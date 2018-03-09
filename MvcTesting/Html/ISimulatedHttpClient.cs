using System;
using System.Threading.Tasks;
using MvcTesting.Http;

namespace MvcTesting.Html
{
    public interface ISimulatedHttpClient
    {
        Task<Response> Process(Request request, Action<Request> modifier);
    }
}
