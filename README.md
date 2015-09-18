![MonoGame.Extended Logo](https://raw.githubusercontent.com/craftworkgames/MonoGame.Extended/master/Logos/logo-banner-800.png)

# MonoGame.Extended

[![Build Status](http://build.craftworkgames.com/app/rest/builds/buildType:(id:MonoGameExtended_CI)/statusIcon)](http://build.craftworkgames.com/viewType.html?buildTypeId=MonoGameExtended_CI&guest=1)

MonoGame.Extended is an open source extension library for [MonoGame](http://www.monogame.net/). It is a collection of classes and extension methods to make it easier to make games with MonoGame. It makes MonoGame more awesome.


## Development status

MonoGame.Extended is a work in progress. New classes are being added almost every day and I'm blogging about them on [dylanwilson.net](http://dylanwilson.net/). We have a very early [alpha Nuget package](https://www.nuget.org/packages/MonoGame.Extended/) released but keep in mind the API is likely to change as the project grows.


## How to install

See the [How to install MonoGame.Extended](http://dylanwilson.net/how-to-install-monogame-extended) blog post. Here's the TL;DR version:

The library is distributed as a NuGet package. Add a reference to your project using the following command:

	Install-Package MonoGame.Extended -Pre

The package comes with 2 DLLs:

 - MonoGame.Extended.dll
 - MonoGame.Extedded.Content.Pipeline.dll
 
The `MonoGame.Extended.Content.Pipeline.dll` is intended to be used with the [Pipeline tool](http://www.monogame.net/documentation/?page=Pipeline). To reference the DLL in the Pipeline tool you'll edit your `Content.mgcb` file.

```
#-------------------------------- References --------------------------------#
    
/reference:..\..\packages\MonoGame.Extended.0.2.0.0\lib\MonoGame.Extended.Content.Pipeline.dll
```


## Contributing

If you would like to contribute start with one of the following:

 - Your thoughts on [what should be included in the library](https://github.com/craftworkgames/MonoGame.Extended/issues/2)
 - Join the discussion on one of the [other issues](https://github.com/craftworkgames/MonoGame.Extended/issues)
 - Talk about it on your [blog](http://dylanwilson.net/) or [twitter](https://twitter.com/craftworkgames)
 - Vote on a task in the [Trello board](https://trello.com/b/Xi6Rfqhb)
 - Create [demos and samples](https://github.com/craftworkgames/Samples.Extended) using MonoGame.Extended
 - and of course, you can fork the project to start making changes

 
## Design goals

 - A clean and consistent API familiar to MonoGame developers.
 - It's *not* a game engine, but extends the framework with code to make games.
 - Follows [C# coding guidelines](https://msdn.microsoft.com/en-us/library/ms229002(v=vs.110).aspx).


## License

MonoGame.Extended is released under the [The MIT License (MIT)](https://github.com/craftworkgames/MonoGame.Extended/blob/master/LICENSE).
 
