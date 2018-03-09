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

        public async Task<Response> GetAsync(string url)
        {
            using (var client = _testServer.CreateClient())
            {
                var response = await client.GetAsync(url);

                var text = await response.Content.ReadAsStringAsync();

                return new Response
                {
                    LastResult = CaptureResultFilter.LastResult.Result,
                    Text = text,
                };
            }
        }

        public async Task<Response> PostAsync(string url)
        {
            using (var client = _testServer.CreateClient())
            {
                var response = await client.PostAsync(url, null);

                var text = await response.Content.ReadAsStringAsync();

                return new Response
                {
                    LastResult = CaptureResultFilter.LastResult.Result,
                    Text = text,
                };
            }
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
