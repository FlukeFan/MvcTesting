using FluentAssertions;
using MvcTesting.Html;
using MvcTesting.Http;
using NUnit.Framework;

namespace MvcTesting.Tests.Html
{
    [TestFixture]
    public class TypedFormExtensionsTests
    {
        [Test]
        public void GetText()
        {
            var html = @"
                <form>
                    <input type='text' name='Name' value='form0' />
                </form>
            ";

            var form = new Response { Text = html }.Form<FormModel>();

            form.GetText(m => m.Name).Should().Be("form0");
        }
    }
}
