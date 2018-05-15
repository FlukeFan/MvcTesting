using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MvcTesting.AspNetCore;

namespace web.tests
{
    public class WebTest : IDisposable
    {
        private TestServer _testServer;

        public WebTest()
        {
            _testServer = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseContentRoot(@"..\..\..\..\web")
                .UseStartup<FakeStartup>()
                .MvcTestingTestServer();
        }

        public SimulatedHttpClient Client()
        {
            return _testServer.MvcTestingClient();
        }

        public void Dispose()
        {
            using (_testServer) { }
        }
    }
}
