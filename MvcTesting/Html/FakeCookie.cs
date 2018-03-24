using System.Collections.Generic;
using System.Linq;

namespace MvcTesting.Html
{
    public class FakeCookie
    {
        public string Name;
        public string Value;

        public static FakeCookie Parse(string headerValue)
        {
            var parts = ParseParts(headerValue);
            return new FakeCookie(parts);
        }

        public FakeCookie() { }

        public FakeCookie(IDictionary<string, string> values)
        {
            Name = values["Name"];
            Value = values["Value"];
        }

        public void Update(IList<FakeCookie> cookies)
        {
            cookies.Add(this);
        }

        private static IDictionary<string, string> ParseParts(string headerValue)
        {
            var result = new Dictionary<string, string>();

            var parts = headerValue.Split(';')
                .Select(p => p?.TrimStart())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            var firstPart = parts[0].Split('=');
            result.Add("Name", firstPart[0]);
            result.Add("Value", firstPart[1]);

            return result;
        }
    }
}
