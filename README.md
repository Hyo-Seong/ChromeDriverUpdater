# ChromeDriverUpdater
[![NuGet version (ChromeDriverUpdater)](https://img.shields.io/nuget/v/ChromeDriverUpdater.svg?style=flat-square)](https://www.nuget.org/packages/ChromeDriverUpdater/)
[![NuGet downloads (ChromeDriverUpdater)](https://img.shields.io/nuget/dt/ChromeDriverUpdater.svg?style=flat-square)](https://www.nuget.org/packages/ChromeDriverUpdater/)

[NuGet Package](https://www.nuget.org/packages/ChromeDriverUpdater)

##Usage

```csharp
try
{
    Updater.Update(@"c:\path\to\chromedriver.exe");
}
catch (UpdateFailException exc)
{
    // ...
}