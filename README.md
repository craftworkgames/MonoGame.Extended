![MonoGame.Extended Logo](https://raw.githubusercontent.com/craftworkgames/MonoGame.Extended/master/Logos/logo-banner-800.png)

# MonoGame.Extended

[![Build Status](http://build.craftworkgames.com/app/rest/builds/buildType:(id:MonoGameExtended_CI)/statusIcon)](http://build.craftworkgames.com/viewType.html?buildTypeId=MonoGameExtended_CI&guest=1)

MonoGame.Extended is an open source extension library for [MonoGame](http://www.monogame.net/). It is a collection of classes and extension methods to make it easier to make games with MonoGame. It makes MonoGame more awesome.


## Development status

MonoGame.Extended is a work in progress. New classes are being added almost every day and I'm blogging about them on [dylanwilson.net](http://dylanwilson.net/). We have a very early [alpha Nuget package](https://www.nuget.org/packages/MonoGame.Extended/) released but keep in mind the API is likely to change as the project grows.

## Usage

MonoGame.Extended is designed to build on top of MonoGame, but may work with XNA/FNA (for now this is not confirmed, but it would most likely require a custom build). Samples and examples of uses for each feature can be found in the [Samples.Extended](https://github.com/craftworkgames/Samples.Extended) repository; we try to keep these up to date with the current feature list.

## Features

**Features up to v0.2:** (More informatino can be found at [the v0.2 release notes](http://dylanwilson.net/monogame-extended-v0-3-release-notes))
- **Tile based maps** using [Tiled](http://www.mapeditor.org/) (orthogonal only in v0.3, isometric in master)
- **Bitmap fonts** using [BMFont](http://www.angelcode.com/products/bmfont/)
- **[Sprites](http://dylanwilson.net/sprites-and-spritebatch-extensions-in-monogame-extended)** (with SpriteBatch extensions!)
- **Input listeners** for event driven input handling (Keyboard, Mouse, Touch)
- **Texture Atlases** using the JSON format in [TexturePacker](https://www.codeandweb.com/texturepacker)
- **2D Camera** with pan, zoom, and rotation
- **Viewport Adapters** for resolution independent rendering

**Added in v0.3:** (More information can be found in [the v0.3 release notes](http://dylanwilson.net/monogame-extended-v0-3-release-notes))
- **Sprite Animators** using texture atlases
- **Timers** including a continuous clock and a countdown timer w/ event integration
- **FPS Counter** that is handy for debugging
- **Circle** primative to complement Monogame's Rectangle

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
 
