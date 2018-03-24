using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcTesting.Html;
using MvcTesting.Http;

namespace MvcTesting.Tests.Html
{
    public class FakeClient : ISimulatedHttpClient
    {
        public Request Request;

        public IList<TestCookie> Cookies => throw new NotImplementedException("cookies not required yet");

        public Task<Response> Process(Request request, Action<Request> modifier)
        {
            Request = request;
            return Task.FromResult<Response>(null);
        }

        public static async Task<Request> Do(string html, Action<TypedForm<FormModel>, FakeClient> submit)
        {
            var form = new Response { Text = html }.Form<FormModel>();
            var client = new FakeClient();
            form.SetClient(client);

            submit(form, client);

            return await Task.FromResult(client.Request);
        }
    }
}
