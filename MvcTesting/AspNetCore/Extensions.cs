using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace MvcTesting.AspNetCore
{
    public static class Extensions
    {
        public static TestServer MvcTestingTestServer(this IWebHostBuilder webHostBuilder)
        {
            return new TestServer(webHostBuilder);
        }

        public static SimulatedHttpClient MvcTestingClient(this TestServer testServer)
        {
            return new SimulatedHttpClient(testServer);
        }
    }
}
