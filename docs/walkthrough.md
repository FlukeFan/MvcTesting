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

MvcTesting is a thin wrapper around the `Microsoft.AspNetCore.TestHost.TestServer`.  In order to get this to work,
open the `web.tests\web.tests.csproj` and add the following section at the end.  A working example can be found
here:  <https://github.com/FlukeFan/MvcTesting/blob/master/example/web.tests/web.tests.csproj>

```xml
  <Target Name="CopyDepsFiles" AfterTargets="Build" Condition="'$(TargetFramework)'!=''">
    <ItemGroup>
      <DepsFilePaths Include="$([System.IO.Path]::ChangeExtension('%(_ResolvedProjectReferencePaths.FullPath)', '.deps.json'))" />
    </ItemGroup>
    <Copy SourceFiles="%(DepsFilePaths.FullPath)" DestinationFolder="$(OutputPath)" Condition="Exists('%(DepsFilePaths.FullPath)')" />
  </Target>
```

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
here:  <https://github.com/FlukeFan/MvcTesting/blob/master/example/web.tests/Doubles.cs>

You should now be able to run your first test:


    cd web.tests
    dotnet test

This test calls through the complete MVC stack to get the razor view, then scrapes the HTML result's form elements into a strongly typed model.  However, there is little or no logic in this controller action, so next we'll add a mor ambitious test.  Add the following to `AccountRegistrationTests.cs`:

```c#
using System.Net;
using Microsoft.AspNetCore.Mvc;

...

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
```

Run the tests again and you should now have two passing tests:

    cd web.tests
    dotnet test

To breakdown what's actually happening in each part of the test:


```c#
            var form = await _webTest.Client()
                .GetAsync("/Account/Register")
                .Form<RegisterViewModel>();
```

The above code 'scrapes' the HTML response into a strongly typed model.  This model
wraps a collection of key-value pairs that can be exmained to see their contents.
In addition, these values can be used to form an HTTP POST equivalent to
the POST request a real browser would send when the submit button is pressed.

```c#
            var response = await form
                .SetText(m => m.Email, "unit.test@unit.test")
                .SetText(m => m.Password, "Un!tTestPassw0rd")
                .SetText(m => m.ConfirmPassword, "Un!tTestPassw0rd")
                .Submit();
```

The above code sets the form's values and posts the form back to the web-server
as a POST request.

```c#
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

            var result = response.ActionResultOf<RedirectToActionResult>();
            Assert.Equal("Home", result.ControllerName);
            Assert.Equal("Index", result.ActionName);
```

The above code checks the response is of the correct type.  Since we added the
`CaptureResultFilter` in the `FakeStartup`, we can also examine the action result of
the request.

```c#
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
```

Finally, because we setup `UserManagerSpy`, `SignInManagerSpy`, and `EmailSenderSpy`
in `FakeStartup`, we can also verify the injected dependencies were used correctly.
