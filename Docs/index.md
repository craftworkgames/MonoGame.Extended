# Welcome

Welcome to the documentation for [MonoGame.Extended](https://github.com/craftworkgames/MonoGame.Extended), a collection of NuGet packages that make [MonoGame](http://www.monogame.net/) more awesome.

We're only just getting started building this documentation. If you're not finding what you're looking for we've got a number of other places to get help.

 - We've got a live chatroom on [gitter](https://gitter.im/craftworkgames/MonoGame.Extended)
 - We've got our own section on the [MonoGame Community Forums](http://community.monogame.net/c/extended)
 - Development is usually discussed in [github issues](https://github.com/craftworkgames/MonoGame.Extended/issues)
 - We've even got a [tag on stackoverflow](http://gamedev.stackexchange.com/questions/tagged/monogame-extended) ;)


# Getting Started

 - [Installation](installation.md)

# About

MonoGame.Extended is an open source extension library for [MonoGame](http://www.monogame.net/). A collection of classes and extensions to make it easier to make games with MonoGame.

The goal of the project is to provide a wide variety of features often needed when making games with MonoGame without being a complete game engine. Most of the features in MonoGame.Extended are isolated into different namespaces. You can pick and choose which bits you want and which bits you don't. It's designed to get you going fast and stay out of your way.

The core library is built as a Portable Class Library (PCL) that contains one code base for all supported platforms. PCL's are fully supported by [Visual Studio 2015 on Windows](https://msdn.microsoft.com/en-us/library/gg597391(v=vs.110).aspx), [Xamarin Studio](https://developer.xamarin.com/guides/cross-platform/application_fundamentals/pcl/introduction_to_portable_class_libraries/) for Android and iOS and have also been reported to work on Linux and Mac. This means that a single self contained library can be used to target a wide variety of platforms.

MonoGame.Extended also provides another DLL that is to be used with the [MonoGame Pipeline tool](http://www.monogame.net/documentation/?page=Pipeline). This provides a set of extra content importers and processors that can be used to load extra content for your game. The content is processed into XNB files just like your textures and sounds and loaded into your games using the standard `Content.Load` method.


# Packages

## Core
The `MonoGame.Extended` core library contains common classes and interfaces the other MonoGame.Extended libraries reference.

By itself, it creates a solid foundation with sprites, [bitmap fonts](MonoGame.Extended/BitmapFonts.md), [collections](MonoGame.Extended/Collections.md), [serialization](MonoGame.Extended/Serialization.md), shapes, texture atlases, viewport adapters, [cameras](MonoGame.Extended/Camera2D.md), timers, math, [object pooling](MonoGame.Extended/Object-Pooling.md), [screens](MonoGame.Extended/Screens.md), and diagnostics.

## Animations
The `MonoGame.Extended.Animations` library contains classes useful for 2D [sprite sheet animations](MonoGame.Extended.Animation/Animated-Sprites.md).

## Collisions
The `MonoGame.Extended.Collisions` library contains a 2D grid based collision system.

## Content.Pipeline
The `MonoGame.Extended.Content.Pipeline` library extends the [MonoGame Content Pipeline tool](http://www.monogame.net/documentation/?page=Pipeline). This adds Animations, BitmapFonts, TextureAtlases, and Tiled maps to the Content Pipeline tool.

## Entities
The `MonoGame.Extended.Entities` library adds an [Entity Component System](https://en.wikipedia.org/wiki/Entity%E2%80%93component%E2%80%93system) (ECS) to MonoGame.

## Graphics
The `MonoGame.Extended.Graphics` library contains extensions useful for generating dynamic geometry and batching draw calls.

## Gui
The `MonoGame.Extended.Gui` library contains a complete GUI system.  It includes Buttons, Text Boxes, Dialogs and many other controls, and is skinnable.

## Input
The `MonoGame.Extended.Input` library contains input listener classes that have events you can use to subscribe to input events, instead of having to poll for input changes.

## NuclexGui
The `MonoGame.Extended.NuclexGui` library contains an implementation of the [Nuclex GUI Framework](https://nuclexframework.codeplex.com/wikipage?title=Nuclex.UserInterface) for XNA, ported to MonoGame.

## Particles
The `MonoGame.Extended.Particles` library contains a high performance Particle System ported from the [Mercury Particle Engine](matthew-davey.github.io/mercury-particle-engine/).

## SceneGraphs
The `MonoGame.Extended.SceneGraphs` library contains a scene graph (tree) system.


## Tiled
The `MonoGame.Extended.Tiled` library loads and renders maps created with the popular [Tiled Map Editor](http://www.mapeditor.org/).

## Tweening
The `MonoGame.Extended.Tweening` library contains class extensions for tween based animations.


