using System;
using MvcTesting.Http;

namespace MvcTesting.Html
{
    public interface ISimulatedHttpClient
    {
        Response Process(Request request, Action<Request> modifier);
    }
}
