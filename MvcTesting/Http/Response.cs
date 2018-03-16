using System;
using System.Net;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.Mvc;
using MvcTesting.Html;

namespace MvcTesting.Http
{
    public class Response
    {
        public static Func<HtmlParser> NewParser = () => new HtmlParser();

        private Lazy<DocumentWrapper> _documentWrapper;

        public Response()
        {
            _documentWrapper = new Lazy<DocumentWrapper>(() =>
            {
                var parser = NewParser();
                var doc = parser.Parse(Text);
                return new DocumentWrapper(doc);
            });
        }

        public ISimulatedHttpClient Client;
        public HttpStatusCode       StatusCode;
        public string               StatusDescription;
        public string               Text;

        public IActionResult        LastResult;

        public HttpStatusCode       HttpStatusCode  { get { return (HttpStatusCode)StatusCode; } }
        public DocumentWrapper      Doc             { get { return _documentWrapper.Value; } }

        public TypedForm<T> Form<T>()                   { return Doc.Form<T>().SetClient(Client); }
        public TypedForm<T> Form<T>(int index)          { return Doc.Form<T>(index).SetClient(Client); }
        public TypedForm<T> Form<T>(string cssSelector) { return Doc.Form<T>(cssSelector).SetClient(Client); }

        public object ActionResult()
        {
            if (LastResult == null)
                throw new Exception("Expected ActionResult, but got no result from global filter CaptureResultFilter");

            return LastResult;
        }

        public T ActionResultOf<T>() where T : class
        {
            var actionResult = ActionResult();

            var result = (actionResult as T);

            if (result == null)
                throw new Exception(string.Format("Expected {0}, but got {1}", typeof(T).Name, actionResult.GetType().Name));

            return result;
        }
    }
}
