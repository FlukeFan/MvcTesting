Walkthrough
===========

This walkthrough will provide instructions to get from a generated ASP.Net Core application to adding your first MvcTesting test against the registration functionality.

You only need .Net Core and a text editor.

Create a directory for your new application in `c:\temp\myapp:

    cd c:\temp\myapp\
    mkdir web
    mkdir web.tests
    cd web
    dotnet new mvc --auth Individual
    cd ..\web.tests
    dotnet new xunit
    dotnet add package MvcTesting

Ad if you want the solution too:

    cd ..
    dotnet new sln
    dotnet sln add web\web.csproj
    dotnet sln add web.tests\web.tests.csproj

