using System;
using AngleSharp.Html.Parser;

namespace MvcTesting.Http
{
    public class InternalServerErrorException : Exception
    {
        public static Func<string, string> FormatException = FormatExceptionResponse;

        public InternalServerErrorException(Response response, Exception innerException) : base(FormatException(response.Text), innerException)
        {
            Response = response;
        }

        public Response Response { get; protected set; }

        public static string FormatExceptionResponse(string responseText)
        {
            try
            {
                var parser = new HtmlParser();
                var doc = parser.ParseDocument(responseText);
                var body = doc.Body;

                var raw = doc.QuerySelector("div.rawExceptionDetails pre");

                if (raw == null)
                    return responseText; // we can't do any better than the raw response

                return raw.TextContent;
            }
            catch (Exception e)
            {
                return e.ToString() + "\n\n" + responseText;
            }
        }
    }
}
