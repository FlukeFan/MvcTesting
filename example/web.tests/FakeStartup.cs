using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcTesting.AspNetCore;
using web.Models;
using web.Services;

namespace web.tests
{
    public class FakeStartup : Startup
    {
        public FakeStartup(IConfiguration configuration) : base(configuration) { }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(UserManager<ApplicationUser>), UserManagerSpy.Instance);
            services.AddSingleton(typeof(SignInManager<ApplicationUser>), SignInManagerSpy.Instance);

            services.AddSingleton<IEmailSender>(EmailSenderSpy.Instance);

            services.AddMvc(o => o.Filters.Add<CaptureResultFilter>());
        }
    }
}
