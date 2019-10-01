:: using .Net SDK 3.0.100
mkdir web
cd web
dotnet new mvc --auth Individual
cd ..
mkdir web.tests
cd web.tests
dotnet new xunit
