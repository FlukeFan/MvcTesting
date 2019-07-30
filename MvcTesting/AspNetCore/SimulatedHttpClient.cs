using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using MvcTesting.Html;
using MvcTesting.Http;

namespace MvcTesting.AspNetCore
{
    public class SimulatedHttpClient : ISimulatedHttpClient
    {
        private TestServer          _testServer;
        private IList<TestCookie>   _cookies    = new List<TestCookie>();

        public SimulatedHttpClient(TestServer testServer)
        {
            _testServer = testServer;
        }

        public IList<TestCookie> Cookies => _cookies;

        public Task<Response> GetAsync(string url, Action<Request> modifier = null)
        {
            var request = new Request(url, "GET");

            return (this as ISimulatedHttpClient).Process(request, modifier);
        }

        public Task<Response> PostAsync(string url, Action<Request> modifier = null)
        {
            var request = new Request(url, "POST");

            return (this as ISimulatedHttpClient).Process(request, modifier);
        }

        private void ProcessCookies(HttpResponseMessage netResponse)
        {
            foreach (var header in netResponse.Headers)
            {
                switch (header.Key)
                {
                    case "Set-Cookie":
                        ParseCookies(header.Value);
                        break;
                }
            }
        }

        private void ParseCookies(IEnumerable<string> headerValues)
        {
            foreach (var headerValue in headerValues)
            {
                var cookie = TestCookie.Parse(headerValue);
                cookie.Update(_cookies);
            }
        }

        async Task<Response> ISimulatedHttpClient.Process(Request request, Action<Request> modifier)
        {
            modifier?.Invoke(request);

            using (var client = _testServer.CreateClient())
            {
                var method = new HttpMethod(request.Verb);

                using (var netRequest = new HttpRequestMessage(method, request.Url))
                {
                    request.SetContent(netRequest);

                    var cookieHeader = TestCookie.CookieHeader(_cookies);

                    if (!string.IsNullOrWhiteSpace(cookieHeader))
                        netRequest.Headers.Add("Cookie", cookieHeader);

                    using (var netResponse = await client.SendAsync(netRequest))
                    {
                        var text = await netResponse.Content.ReadAsStringAsync();
                        ProcessCookies(netResponse);

                        var response = new Response
                        {
                            Client = this,
                            LastResult = CaptureResultFilter.LastResult?.Result,
                            StatusCode = netResponse.StatusCode,
                            Text = text,
                        };

                        var error = netResponse.StatusCode == HttpStatusCode.InternalServerError
                            || (CaptureResultFilter.LastException != null && !CaptureResultFilter.LastExceptionHandled);

                        if (error)
                            throw new InternalServerErrorException(response, CaptureResultFilter.LastException);

                        if (request.ExptectedResponse.HasValue && request.ExptectedResponse.Value != response.HttpStatusCode)
                            throw new UnexpectedStatusCodeException(response, request.ExptectedResponse.Value);

                        return response;
                    }
                }
            }
        }
    }
}
