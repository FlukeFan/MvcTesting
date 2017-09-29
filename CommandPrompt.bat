@CD /D "%~dp0"
@title MvcTesting Command Prompt
@SET PATH=C:\Program Files (x86)\MSBuild\14.0\Bin\;%PATH%
@doskey b=msbuild $* MvcTesting.proj
type readme.md
%comspec%
