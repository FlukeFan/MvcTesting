using System.Collections.Generic;
using System.Linq;

namespace MvcTesting.Http
{
    public class TestCookie
    {
        public string Name;
        public string Value;

        public static TestCookie Parse(string headerValue)
        {
            var parts = ParseParts(headerValue);
            return new TestCookie(parts);
        }

        public static string CookieHeader(IList<TestCookie> cookies)
        {
            var cookieValues = cookies
                .Select(c => $"{c.Name}={c.Value}");

            var header = string.Join("; ", cookieValues);
            return header;
        }

        public TestCookie() { }

        public TestCookie(IDictionary<string, string> values)
        {
            Name = values["Name"];
            Value = values["Value"];
        }

        public void Update(IList<TestCookie> cookies)
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
