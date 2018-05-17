using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [Fact]
        public async Task WhenDetailsEntered_DisplaysExternalForm()
        {
            var form = await _webTest.Client()
                .GetAsync("/Account/Register")
                .Form<RegisterViewModel>();

            var response = await form
                .SetText(m => m.Email, "unit.test@unit.test")
                .SetText(m => m.Password, "Un!tTestPassw0rd")
                .SetText(m => m.ConfirmPassword, "Un!tTestPassw0rd")
                .Submit();

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

            var result = response.ActionResultOf<RedirectToActionResult>();
            Assert.Equal("Home", result.ControllerName);
            Assert.Equal("Index", result.ActionName);

            var actualCreatedUser = UserManagerSpy.LastCreate.Item1;
            var actualPassword = UserManagerSpy.LastCreate.Item2;

            Assert.Equal("unit.test@unit.test", actualCreatedUser.UserName);
            Assert.Equal("unit.test@unit.test", actualCreatedUser.Email);
            Assert.Equal("Un!tTestPassw0rd", actualPassword);

            var actualEmail = EmailSenderSpy.LastSendEmail.Item1;
            var actualSubject = EmailSenderSpy.LastSendEmail.Item2;
            var actualMessage = EmailSenderSpy.LastSendEmail.Item3;

            Assert.Equal("unit.test@unit.test", actualEmail);
            Assert.Equal("Confirm your email", actualSubject);
            Assert.Contains(UserManagerSpy.EmailToken, actualMessage);

            var actualSignInUser = SignInManagerSpy.LastSignIn.Item1;
            var actualPersistent = SignInManagerSpy.LastSignIn.Item2;

            Assert.Same(actualCreatedUser, actualSignInUser);
            Assert.False(actualPersistent);
        }
    }
}
