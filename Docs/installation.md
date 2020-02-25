# Installation

MonoGame.Extended is a collection of [.NET Standard libraries](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) designed to be referenced from any MonoGame project to add common extensions and classes for making your games more awesome.

The libraries are distributed as [NuGet packages](https://www.nuget.org/packages?q=MonoGame.Extended) and can be installed using the NuGet Package Manager in [Visual Studio](https://www.visualstudio.com), [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/), or [MonoDevelop](http://www.monodevelop.com). They can also be installed by running the following command (or the equivalent command for the package you want to install) in the [Package Manager Console](http://docs.nuget.org/consume/package-manager-console).

	Install-Package MonoGame.Extended

Note that [MonoGame 3.6 will need to be installed](http://www.monogame.net/downloads/) to properly use MonoGame.Extended in your game project.

## Referencing the Content Pipeline extension
	
To get the full experience you'll also want to install the Content Pipeline extension:

    Install-Package MonoGame.Extended.Content.Pipeline

This package is not included in MonoGame.Extended and must be installed separately. It won't add any references to your project. Instead it will download a DLL that's intended to be referenced from the [MonoGame Content Pipeline tool](http://www.monogame.net/documentation/?page=Pipeline).

You'll need to manually add the reference to your content file (usually `Content.mgcb`) using one of the following methods.

### Using the MonoGame Pipeline GUI

To add the reference using the Pipeline GUI tool follow these steps:

 1. Click on the **Content** node in the root of the tree.
 2. In the properties window, modify the **References** property.
 3. Find and add the `MonoGame.Extended.Content.Pipeline.dll`. It's usually located in the **packages** folder of your solution.  The default location of the **packages** folder in .NET Core is:  C:\Users\[User]\.nuget\packages.

![MonoGame Pipeline Add References](https://dl.dropboxusercontent.com/u/82020056/MonoGame.Extended/how-to-add-content-pipeline-reference.png)
 
### Using a text editor

An alternative way to add the reference is by manually editing the `Content.mgcb` file in a text editor or Visual Studio. Look for the references section and update it like this:

```
#-------------------------------- References --------------------------------#

/reference:..\..\packages\MonoGame.Extended.Content.Pipeline.0.6.372\tools\MonoGame.Extended.Content.Pipeline.dll
```

**Remember:** The `MonoGame.Extended.dll` and the `MonoGame.Extended.Content.Pipeline.dll` come as a pair. Always make sure the version referenced by your game matches the version referenced by the Pipeline tool.

That's it! Once you've referenced the library you can start using it to make your games even more awesome.

