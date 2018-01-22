echo off
IF NOT EXIST ".nuget\nuget.exe" (
    MD .nuget
    @powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' -OutFile '.nuget/nuget.exe'"
)
.nuget\nuget.exe update -self

IF NOT EXIST "packages\FAKE" (
    ".nuget\nuget.exe" "install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion" "-Source" "nuget.org"
)

SET BUILDCONFIG="Debug"
IF NOT [%1]==[] (SET BUILDCONFIG="%1")

SET OUTPUTDIR="C:\inetpub\wwwroot\GrandfathersApiary\Website"
IF NOT [%2]==[] (SET OUTPUTDIR="%2")

SET VERSION="1.0.0.0"
IF NOT [%3]==[] (SET VERSION="%3")

"packages\FAKE\tools\Fake.exe" build.fsx "BuildConfiguration=%BUILDCONFIG%" "OutputDir=%OUTPUTDIR%" "InstallWebpack=%INSTLWBPCK%" "Version=%VERSION%"