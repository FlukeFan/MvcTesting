
MvcTesting
==========

A library to test (pre core) Asp.Net Mvc applications from request through to (HTML) response.
Heavily based on http://blog.stevensanderson.com/2009/06/11/integration-testing-your-aspnet-mvc-application/

Building
========

To build, open CommandPrompt.bat, and type 'b'.

Build commands:

b                               : build
b /t:clean                      : clean
b /t:setApiKey /p:apiKey=[key]  : set the api key
b /t:push                       : Push packages to NuGet and publish them (setApiKey before running this)
