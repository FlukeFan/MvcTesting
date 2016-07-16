
MvcTesting
==========

A library to test (pre core) Asp.Net Mvc applications from request through to (HTML) response.

Building
========

To build, open CommandPrompt.bat, and type 'b'.

Build commands:

b                               : build
b /t:clean                      : clean
b /t:setApiKey /p:apiKey=[key]  : set the api key
b /t:push                       : Push packages to NuGet and publish them (setApiKey before running this)
