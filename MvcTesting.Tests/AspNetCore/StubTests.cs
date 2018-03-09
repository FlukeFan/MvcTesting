using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using MvcTesting.AspNetCore;
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
        }
    }
}
