using System.Threading.Tasks;
using MvcTesting.Html;
using web.Models.AccountViewModels;
using Xunit;

namespace web.tests
{
    public class AccountRegistrationTests : IClassFixture<WebTest>
    {
        private WebTest _webTest;

        public AccountRegistrationTests(WebTest webTest)
        {
            _webTest = webTest;
        }

        [Fact]
        public async Task WhenRegistering_DisplaysForm()
        {
            var form = await _webTest.Client()
                .GetAsync("/Account/Register")
                .Form<RegisterViewModel>();

            Assert.Equal("", form.GetText(f => f.Email));
        }
    }
}
