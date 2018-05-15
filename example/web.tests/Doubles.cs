using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using web.Models;
using web.Services;

namespace web.tests
{
    public class EmailSenderSpy : IEmailSender
    {
        public static readonly EmailSenderSpy Instance = new EmailSenderSpy();

        public static Tuple<string, string, string> LastSendEmail;

        private EmailSenderSpy() { }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            LastSendEmail = Tuple.Create(email, subject, message);
            return Task.CompletedTask;
        }
    }

    public class UserManagerSpy : UserManager<ApplicationUser>
    {
        public const string EmailToken = "stub_email_token";

        public static readonly UserManagerSpy Instance = new UserManagerSpy();

        public static Tuple<ApplicationUser, string> LastCreate;

        private UserManagerSpy()
            : base(new DummyUserStore(), new DummyOptionsAccessor(), null, null, null, null, null, null, null) { }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            LastCreate = Tuple.Create(user, password);
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return Task.FromResult(EmailToken);
        }

        public class DummyOptionsAccessor : OptionsWrapper<IdentityOptions>
        {
            public DummyOptionsAccessor() : base(new IdentityOptions()) { }
        }

        public class DummyUserStore : IUserStore<ApplicationUser>
        {
            public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken) { throw new System.NotImplementedException(); }
            public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken) { throw new System.NotImplementedException(); }
            public void Dispose() { throw new System.NotImplementedException(); }
            public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken) { throw new System.NotImplementedException(); }
            public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) { throw new System.NotImplementedException(); }
            public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) { throw new System.NotImplementedException(); }
            public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) { throw new System.NotImplementedException(); }
            public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) { throw new System.NotImplementedException(); }
            public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken) { throw new System.NotImplementedException(); }
            public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken) { throw new System.NotImplementedException(); }
            public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken) { throw new System.NotImplementedException(); }
        }
    }

    public class SignInManagerSpy : SignInManager<ApplicationUser>
    {
        public static Tuple<ApplicationUser, bool, string> LastSignIn;

        public static readonly SignInManagerSpy Instance = new SignInManagerSpy(UserManagerSpy.Instance);

        private SignInManagerSpy(UserManagerSpy userManager)
            : base(userManager, new DummyContextAccessor(), new DummyClaimsFactory(), null, null, null) { }

        public override Task SignInAsync(ApplicationUser user, bool isPersistent, string authenticationMethod = null)
        {
            LastSignIn = Tuple.Create(user, isPersistent, authenticationMethod);
            return Task.CompletedTask;
        }

        public class DummyContextAccessor : IHttpContextAccessor
        {
            public HttpContext HttpContext { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        }

        public class DummyClaimsFactory : IUserClaimsPrincipalFactory<ApplicationUser>
        {
            public Task<ClaimsPrincipal> CreateAsync(ApplicationUser user) { throw new System.NotImplementedException(); }
        }
    }
}
