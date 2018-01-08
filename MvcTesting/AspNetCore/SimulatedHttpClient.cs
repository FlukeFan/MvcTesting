using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using MvcTesting.Html;
using MvcTesting.Http;

namespace MvcTesting.AspNetCore
{
    public class SimulatedHttpClient : ISimulatedHttpClient
    {
        private TestServer _testServer;

        public SimulatedHttpClient(TestServer testServer)
        {
            _testServer = testServer;
        }

        public async Task<Response> GetAsync(string url)
        {
            var response = await _testServer.CreateClient().GetAsync(url);

            var text = await response.Content.ReadAsStringAsync();

            return new Response
            {
                Text = text,
            };
        }

        Response ISimulatedHttpClient.Process(Request request, Action<Request> modifier)
        {
            throw new NotImplementedException();
        }
    }
}
