using System.IO;
using System.Net;
using System.Text;
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
                .UseEnvironment("Development")
                .UseContentRoot(@"..\..\..\..\MvcTesting.StubApp")
                .UseStartup<FakeStartup>()
                .MvcTestingTestServer();
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {
            using (_testServer) { }
        }

        [SetUp]
        public void SetUp()
        {
            if (File.Exists("stub_test.txt"))
                File.Delete("stub_test.txt");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists("stub_test.txt"))
                File.Delete("stub_test.txt");
        }

        [Test]
        public async Task ViewRequest_GET()
        {
            var client = _testServer.MvcTestingClient();

            var response = await client.GetAsync("/Stub/ViewRequest");

            response.Text.Should().Contain("Method=GET");
        }

        [Test]
        public async Task ViewRequest_GET_Query()
        {
            var client = _testServer.MvcTestingClient();

            var response = await client.GetAsync("/Stub/ViewRequest?var1=val1");

            response.Text.Should().Contain("var1=val1");
        }

        [Test]
        public async Task ViewRequest_POST()
        {
            var client = _testServer.MvcTestingClient();

            var response = await client.PostAsync("/Stub/ViewRequest", r => r.SetExpectedResponse(HttpStatusCode.OK));

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

            await form.Submit(r => r.SetExpectedResponse(HttpStatusCode.OK));
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
        public async Task FileUpload()
        {
            var client = _testServer.MvcTestingClient();

            var page = await client.GetAsync("/FileUpload");
            var form = page.Form<FakeFileModel>();

            await form
                .SetFile(m => m.File, "stub_test.txt", ASCIIEncoding.ASCII.GetBytes("some content"))
                .Submit();

            File.Exists("stub_test.txt").Should().BeTrue();
            File.ReadAllText("stub_test.txt").Should().Be("some content");
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

        [Test]
        public void CatchesInternalErrors()
        {
            var client = _testServer.MvcTestingClient();

            var e = Assert.ThrowsAsync<InternalServerErrorException>(async () =>
            {
                await client.GetAsync("/Stub/Throw");
            });

            e.Message.Should().Contain("thrown from stub");
            e.Message.Should().NotContain("&#x", "html entities/tags should have been stripped out of the response");
        }

        [Test]
        public async Task DefaultGet_ReturnsResponse()
        {
            var client = _testServer.MvcTestingClient();

            var response = await client.GetAsync($"/Stub/Code/{(int)HttpStatusCode.OK}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Text.Should().Be("200");
        }

        [Test]
        public void NonDefaultGet_Throws()
        {
            var client = _testServer.MvcTestingClient();

            var e = Assert.ThrowsAsync<UnexpectedStatusCodeException>(async () =>
            {
                await client.GetAsync($"/Stub/Code/{(int)HttpStatusCode.PartialContent}");
            });
        }

        [Test]
        public async Task NonDefaultGet_CanModify()
        {
            var client = _testServer.MvcTestingClient();

            var response = await client.GetAsync($"/Stub/Code/{(int)HttpStatusCode.PartialContent}", r => r.SetExpectedResponse(HttpStatusCode.PartialContent));

            response.StatusCode.Should().Be(HttpStatusCode.PartialContent);
            response.Text.Should().Be("206");
        }

        [Test]
        public async Task DefaultPost_ReturnsResponse()
        {
            var client = _testServer.MvcTestingClient();

            var response = await client.PostAsync($"/Stub/Code/{(int)HttpStatusCode.Redirect}");

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Text.Should().Be("302");
        }

        [Test]
        public void NonDefaultPost_Throws()
        {
            var client = _testServer.MvcTestingClient();

            var e = Assert.ThrowsAsync<UnexpectedStatusCodeException>(async () =>
            {
                await client.PostAsync($"/Stub/Code/{(int)HttpStatusCode.PartialContent}");
            });
        }

        [Test]
        public async Task NonDefaultPost_CanModify()
        {
            var client = _testServer.MvcTestingClient();

            var response = await client.PostAsync($"/Stub/Code/{(int)HttpStatusCode.PartialContent}", r => r.SetExpectedResponse(HttpStatusCode.PartialContent));

            response.StatusCode.Should().Be(HttpStatusCode.PartialContent);
            response.Text.Should().Be("206");
        }

        private class FakeFileModel
        {
            public byte[] File { get; set; }
        }
    }
}
