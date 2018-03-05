using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MvcTesting.AspNetCore;
using MvcTesting.StubApp;

namespace MvcTesting.Tests.AspNetCore
{
    public class FakeStartup : Startup
    {
        public FakeStartup(IConfiguration configuration) : base(configuration) { }

        protected override void SetupAction(MvcOptions options)
        {
            base.SetupAction(options);
            options.Filters.Add<CaptureResultFilter>();
        }
    }
}
