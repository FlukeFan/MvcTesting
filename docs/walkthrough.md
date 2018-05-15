Walkthrough
===========

This walkthrough will provide instructions to get from a generated ASP.Net Core application to adding your first MvcTesting test against the registration functionality.

You only need .Net Core and a text editor.

Create a directory for your new application in `c:\temp\myapp`:

    cd c:\temp\myapp\
    mkdir web
    mkdir web.tests
    cd web
    dotnet new mvc --auth Individual
    cd ..\web.tests
    dotnet new xunit
    dotnet add reference ..\web\web.csproj
    dotnet add package MvcTesting

And if you want the solution too:

    cd ..
    dotnet new sln
    dotnet sln add web\web.csproj
    dotnet sln add web.tests\web.tests.csproj

Delete the file `UnitTest1.cs`, and add a new file `AccountRegistrationTests.cs` with the following:

```c#
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
```

This is what our first web test will look like.  This relies on a couple of helper classes, so add
the following code to `WebTest.cs`:

```c#
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MvcTesting.AspNetCore;

namespace web.tests
{
    public class WebTest : IDisposable
    {
        private TestServer _testServer;

        public WebTest()
        {
            _testServer = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseContentRoot(@"..\..\..\..\web")
                .UseStartup<FakeStartup>()
                .MvcTestingTestServer();
        }

        public SimulatedHttpClient Client()
        {
            return _testServer.MvcTestingClient();
        }

        public void Dispose()
        {
            using (_testServer) { }
        }
    }
}
```

... and add the following code to `FakeStartup.cs`:

```c#
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
```

Update the ConfigureServices in Startup.cs (in the web project) to make the ConfigureServices method virtual:

```c#
    ...
    // This method gets called by the runtime. Use this method to add services to the container.
    public virtual void ConfigureServices(IServiceCollection services)
    {
        ...

```

In order to test some parts of the AccountController, we have to stub out some dependencies
during the tests.  Add the test doubles for `UserManagerSpy`, `SignInManagerSpy`, and `EmailSenderSpy` copied from the source code
here:  https://github.com/FlukeFan/MvcTesting/blob/master/example/web.tests/Doubles.cs

You should now be able to run your first test:


    cd web.tests
    dotnet test

