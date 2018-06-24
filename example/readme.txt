:: using .Net SDK 2.1.200
mkdir web
cd web
dotnet new mvc --auth Individual
cd ..
mkdir web.tests
cd web.tests
dotnet new xunit
