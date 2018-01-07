using System;
using System.Net;
using AngleSharp.Parser.Html;
using MvcTesting.Html;

namespace MvcTesting.Http
{
    [Serializable]
    public class Response
    {
        public static Func<HtmlParser> NewParser = () => new HtmlParser();

        [NonSerialized]
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

        public int              StatusCode;
        public string           StatusDescription;
        public string           Text;

        [NonSerialized]
        public object LastResult;

        public HttpStatusCode   HttpStatusCode  { get { return (HttpStatusCode)StatusCode; } }
        public DocumentWrapper  Doc             { get { return _documentWrapper.Value; } }

        public TypedForm<T> Form<T>()                   { return Doc.Form<T>(); }
        public TypedForm<T> Form<T>(int index)          { return Doc.Form<T>(index); }
        public TypedForm<T> Form<T>(string cssSelector) { return Doc.Form<T>(cssSelector); }

        public object ActionResult()
        {
            if (LastResult == null)
                throw new Exception("Expected ActionResult, but got no result from global filter CaptureResultFilter");

            return "fake it - just to get to compile";// LastResult.Result;
        }

        public T ActionResultOf<T>() where T : class
        {
            var actionResult = ActionResult();

            if (actionResult == null)
                throw new Exception("Expected ActionResult, but got <null>");

            var result = (actionResult as T);

            if (result == null)
                throw new Exception(string.Format("Expected {0}, but got {1}", typeof(T).Name, actionResult.GetType().Name));

            return result;
        }
    }
}
