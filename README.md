# ChromeDriverUpdater

[![Build Status (ChromeDriverUpdater)](https://github.com/hyo-seong/chromedriverupdater/workflows/Test/badge.svg)](https://github.com/hyo-seong/chromedriverupdater/actions)
[![License (ChromeDriverUpdater)](https://img.shields.io/github/license/Hyo-Seong/chromedriverupdater)](https://github.com/Hyo-Seong/ChromeDriverUpdater/blob/main/LICENSE.md)
[![NuGet version (ChromeDriverUpdater)](https://img.shields.io/nuget/v/ChromeDriverUpdater.svg)](https://www.nuget.org/packages/ChromeDriverUpdater/)
[![NuGet downloads (ChromeDriverUpdater)](https://img.shields.io/nuget/dt/ChromeDriverUpdater.svg)](https://www.nuget.org/packages/ChromeDriverUpdater/)

[NuGet Package](https://www.nuget.org/packages/ChromeDriverUpdater)

## Available OS
OS | Available
----|----
Windows|O
Mac|X
Linux|O

## Usage

```csharp
try
{
    Updater.Update(@"c:\path\to\chromedriver.exe");
}
catch (UpdateFailException exc)
{
    // ...
}
