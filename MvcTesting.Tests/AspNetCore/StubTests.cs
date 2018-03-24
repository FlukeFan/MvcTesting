using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using MvcTesting.AspNetCore;
using MvcTesting.Html;
using MvcTesting.Http;
using MvcTesting.StubApp.Controllers;
using MvcTesting.StubApp.Views.Stub;
using NUnit.Framework;

namespace MvcTesting.Tests.AspNetCore
{
    [TestFixture]
    public class StubTests
    {
        private TestServer _testServer;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            _testServer = new WebHostBuilder()
                .UseContentRoot(@"..\..\..\..\MvcTesting.StubApp")
                .UseStartup<FakeStartup>()
                .MvcTestingTestServer();
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {
            using (_testServer) { }
        }

        [Test]
        public async Task ViewRequest_GET()
        {
            var client = _testServer.MvcTestingClient();

            var response = await client.GetAsync("/Stub/ViewRequest");

            response.Text.Should().Contain("Method=GET");
        }

        [Test]
        public async Task ViewRequest_POST()
        {
            var client = _testServer.MvcTestingClient();

            var response = await client.PostAsync("/Stub/ViewRequest");

            response.Text.Should().Contain("Method=POST");
        }

        [Test]
        public async Task LastResult()
        {
            var client = _testServer.MvcTestingClient();

            var response = await client.GetAsync("/Stub/ViewRequest");

            var lastResult = response.LastResult;

            lastResult.Should().NotBeNull("Expected last MVC result to be captured");
            lastResult.Should().BeOfType<ViewResult>();
            var model = (ViewRequestModel)(lastResult as ViewResult).Model as ViewRequestModel;
            model.Method.Should().Be("GET");
        }

        [Test]
        public async Task SimpleFormGet()
        {
            var client = _testServer.MvcTestingClient();

            var page = await client.GetAsync("/Stub/SimpleForm");
            var form = page.Form<SimpleFormModel>();

            form.GetSingle("Text").Value.Should().Be("existing");

            await form.Submit();
        }

        [Test]
        public async Task SimpleFormSubmit()
        {
            var client = _testServer.MvcTestingClient();

            var page = await client.GetAsync("/Stub/SimpleForm");
            var form = page.Form<SimpleFormModel>();

            var response = await form
                .SetText(m => m.Text, "success")
                .Submit();

            response.HttpStatusCode.Should().Be(HttpStatusCode.Redirect);
            var result = response.ActionResultOf<RedirectResult>();
            result.Url.Should().Be("~/Stub/Success");
        }

        [Test]
        public async Task Cookies()
        {
            var client = _testServer.MvcTestingClient();
            client.Cookies.Count.Should().Be(0);

            await client.GetAsync("/Stub/SetCookies");

            client.Cookies.ShouldBeEquivalentTo(new[]
            {
                new TestCookie { Name = "a", Value = "2" },
                new TestCookie { Name = "b", Value = "3" },
            });

            client.Cookies.RemoveAt(0);
            client.Cookies.Add(new TestCookie { Name = "c", Value = "4" });

            var response = await client.GetAsync("/Stub/GetCookies");

            response.Text.Should().Be("b=3;c=4");
        }
    }
}
