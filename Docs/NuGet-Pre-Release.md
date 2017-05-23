# Pre-Release NuGet packages

You can get access to the pre-release NuGet packages by adding the [Craftwork Games Build Server](http://build.craftworkgames.com/guestAuth/app/nuget/v1/FeedService.svc/) as a NuGet package source in Visual Studio.

![Craftwork Games Build Server NuGet Package Source](https://dl.dropboxusercontent.com/u/82020056/craftwork-games-build-server-nuget-package-source.png)

## Instructions

 1. In Visual Studio go to Tools => NuGet Package Manager => Package Manager Settings => Package Sources
 2. Add a new package source using the following url `http://build.craftworkgames.com/guestAuth/app/nuget/v1/FeedService.svc/`
 3. When you're adding the package, remember to tick the "Include prerelease" checkbox

![Include prerelease](https://dl.dropboxusercontent.com/u/82020056/install-monogame-extended-nuget-package-include-prerelease.png)

