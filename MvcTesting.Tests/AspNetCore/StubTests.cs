using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using MvcTesting.AspNetCore;
using NUnit.Framework;

namespace MvcTesting.Tests.AspNetCore
{
    [TestFixture]
    public class StubTests
    {
        [Test]
        public async Task ViewRequest_GET()
        {
            var server = new WebHostBuilder()
                .UseContentRoot(@"..\..\..\..\MvcTesting.StubApp")
                .UseStartup<StubApp.Startup>()
                .MvcTestingTestServer();

            var client = server.MvcTestingClient();

            var response = await client.GetAsync("/Stub/ViewRequest");

            response.Text.Should().Contain("Method=GET");
        }
    }
}
