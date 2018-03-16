using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

                using (var netRequest = new HttpRequestMessage(method, request.Url))
                {
                    if (request.Verb == "POST" && request.FormValues != null)
                    {
                        var sb = new StringBuilder();

                        foreach (var formValue in request.FormValues)
                            sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(formValue.Name), HttpUtility.UrlEncode(formValue.Value));

                        var encodedFormValues = sb.ToString();
                        var formBytes = Encoding.UTF8.GetBytes(encodedFormValues);
                        netRequest.Content = new ByteArrayContent(formBytes);

                        foreach (string name in request.Headers)
                            netRequest.Content.Headers.Add(name, request.Headers[name]);
                    }

                    using (var netResponse = await client.SendAsync(netRequest))
                    {
                        var text = await netResponse.Content.ReadAsStringAsync();

                        var response = new Response
                        {
                            Client = this,
                            LastResult = CaptureResultFilter.LastResult?.Result,
                            StatusCode = netResponse.StatusCode,
                            Text = text,
                        };

                        return response;
                    }
                }
            }
        }
    }
}
