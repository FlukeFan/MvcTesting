using System;
using System.Net.Http;
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

        public Task<Response> GetAsync(string url)
        {
            var request = new Request(url, "GET");

            return (this as ISimulatedHttpClient).Process(request, null);
        }

        public Task<Response> PostAsync(string url)
        {
            var request = new Request(url, "POST");

            return (this as ISimulatedHttpClient).Process(request, null);
        }

        async Task<Response> ISimulatedHttpClient.Process(Request request, Action<Request> modifier)
        {
            using (var client = _testServer.CreateClient())
            {
                var method = new HttpMethod(request.Verb);
                var netRequest = new HttpRequestMessage(method, request.Url);

                var netResponse = await client.SendAsync(netRequest);
                var text = await netResponse.Content.ReadAsStringAsync();

                var response = new Response
                {
                    LastResult = CaptureResultFilter.LastResult.Result,
                    Text = text,
                };

                return response;
            }
        }
    }
}
