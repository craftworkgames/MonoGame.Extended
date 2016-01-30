![MonoGame.Extended Logo](https://raw.githubusercontent.com/craftworkgames/MonoGame.Extended/master/Logos/logo-banner-800.png)

# MonoGame.Extended
It makes MonoGame more awesome.

[![Join the chat at https://gitter.im/craftworkgames/MonoGame.Extended](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/craftworkgames/MonoGame.Extended?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Build Status](http://build.craftworkgames.com/app/rest/builds/buildType:(id:MonoGameExtended_CI)/statusIcon)](http://build.craftworkgames.com/viewType.html?buildTypeId=MonoGameExtended_CI&guest=1)

MonoGame.Extended is an open source extension library for [MonoGame](http://www.monogame.net/). A collection of classes and extensions to make it easier to make games with MonoGame. 

## Forums

We now have forums! Our forum is part of the [MonoGame community](http://community.monogame.net/category/extended). Please ask any questions or post about problems or bugs that you have found there. Let us know if you're making a game with MonoGame.Extended!

## Development status

MonoGame.Extended is a work in progress. New classes are being added almost every day and I'm blogging about them on [dylanwilson.net](http://dylanwilson.net/). We have a very early [alpha Nuget package](https://www.nuget.org/packages/MonoGame.Extended/) released but keep in mind the API is likely to change as the project grows.

## Usage

MonoGame.Extended is a portable class library designed to build on top of MonoGame. It may work also with XNA/FNA (not confirmed) and it should work on other platforms like Xamarin Android or iOS. If you do use it on another platform please [let us know](http://community.monogame.net/category/extended)!

Samples and examples can be found in the [Samples.Extended](https://github.com/craftworkgames/Samples.Extended) repository; we try to keep these up to date with the current feature list as much as possible. Contributions are welcome.

## How to install

See the [How to install MonoGame.Extended](http://dylanwilson.net/how-to-install-monogame-extended) blog post. Here's the TL;DR version:

The library is distributed as a NuGet package. Add a reference to your project using the following command:

	Install-Package MonoGame.Extended

The package comes with 2 DLLs:

 - MonoGame.Extended.dll
 - MonoGame.Extedded.Content.Pipeline.dll
 
The `MonoGame.Extended.Content.Pipeline.dll` is intended to be used with the [Pipeline tool](http://www.monogame.net/documentation/?page=Pipeline). To reference the DLL in the Pipeline tool you'll  need to edit your `Content.mgcb` file as follows.

```
#-------------------------------- References --------------------------------#
    
/reference:..\..\packages\MonoGame.Extended.0.4.64\lib\MonoGame.Extended.Content.Pipeline.dll
```

**Note**: *Be sure to check that the version number in the path matches the version you're using!* 

## Features

**Features up to v0.2:** ([release notes](http://dylanwilson.net/monogame-extended-v0-3-release-notes))
- **Tile based maps** using [Tiled](http://www.mapeditor.org/) (orthogonal only in v0.3, isometric in master)
- **Bitmap fonts** using [BMFont](http://www.angelcode.com/products/bmfont/)
- **[Sprites](http://dylanwilson.net/sprites-and-spritebatch-extensions-in-monogame-extended)** (with SpriteBatch extensions!)
- **Input listeners** for event driven input handling (Keyboard, Mouse, Touch) (GamePad in master)
- **Texture Atlases** using the JSON format in [TexturePacker](https://www.codeandweb.com/texturepacker)
- **2D Camera** with pan, zoom, and rotation
- **Viewport Adapters** for resolution independent rendering

**Added in v0.3:** ([release notes](http://dylanwilson.net/monogame-extended-v0-3-release-notes))
- **Sprite Animators** using texture atlases
- **Timers** including a continuous clock and a countdown timer w/ event integration
- **FPS Counter** that is handy for debugging
- **Circle** primative to complement Monogame's Rectangle

**Added in v0.4:** ([release notes](http://dylanwilson.net/monogame-extended-v0-3-release-notes))
- **Tiled Map Renderer** - now uses a render target to eliminate tearing
- **Sprite Sheet Animations** - created with the [Astrid Animator](http://dylanwilson.net/introducing-astrid-animator) prototype
- **Simple Collision Detection** - experimental
- **Bug fixes** - with relative assets in the content importers and a few other minor bug fixes


## Contributing

If you would like to contribute start with one of the following:

 - Please post your thoughts on our new [forum](http://community.monogame.net/category/extended)
 - Join the discussion on one of the [issues](https://github.com/craftworkgames/MonoGame.Extended/issues). We often use github issues to discuss new features as well.
 - Talk about it on your [blog](http://dylanwilson.net/) or [twitter](https://twitter.com/craftworkgames)
  - Create [demos and samples](https://github.com/craftworkgames/Samples.Extended) using MonoGame.Extended
 - and of course, you can fork the project to start making changes

 
## Design goals

 - A clean and consistent API familiar to MonoGame developers.
 - It's *not* a game engine, but extends the framework with code to make games.
 - Follows [C# coding guidelines](https://msdn.microsoft.com/en-us/library/ms229002(v=vs.110).aspx).


## License

MonoGame.Extended is released under the [The MIT License (MIT)](https://github.com/craftworkgames/MonoGame.Extended/blob/master/LICENSE).
 
