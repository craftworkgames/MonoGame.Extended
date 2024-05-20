> [!NOTE]
> Hi MonoGame community, MonoGame.Extended is currently being updated to resolve outstanding bug issues, included those surrounding Tiled, and to update for MonoGame 3.8.1.303.  Please bear with me as I work through the backlog.  You can follow the progress in the [v4.0.0 Milestones](https://github.com/craftworkgames/MonoGame.Extended/milestone/8) page.
>
> - AristurtleDev

![MonoGame.Extended Logo](logos/logo-banner-800.png)

# MonoGame.Extended

MonoGame.Extended is a set of utilities (in the form of libraries/tools) to [MonoGame](http://www.monogame.net/) that makes it easier to make games. Choose what you want, the rest stays out of your way. It makes MonoGame more awesome.

[![Build, Test, Deploy](https://github.com/craftworkgames/MonoGame.Extended/workflows/Build,%20Test,%20Deploy/badge.svg?branch=develop)](https://github.com/craftworkgames/MonoGame.Extended/actions?query=workflow%3A%22Build%2C+Test%2C+Deploy%22) [![Docs](https://img.shields.io/badge/Docs-latest-brightgreen.svg?style=flat)](http://www.monogameextended.net/)

## Getting started

Code is distributed as NuGet packages in the form of libraries (`.dll` files). You can easily install the NuGet packages into your existing MonoGame project using the NuGet Package Manager UI in Visual Studio or by using the command line interface (CLI) in a terminal.

**Current Stable Release**
> [!WARNING]
> The current stable release is not compatible with MonoGame 3.8.1.303.

```sh
dotnet add package MonoGame.Extended --version 3.8.0
```

**Current Prerelease**
> [!NOTE]
> Prerelease is based on current `develop` branch snapshot.  There it is not considered stable and may contain bugs
```sh
dotnet add package MonoGame.Extended --version 3.9.0-prerelease.4
```

### Using the Content Pipeline Extensions
To use the content pipeline extensions, you will need to edit your `.mgcb` file to reference the `.dll`. To see an example of how to do this with NuGet see the samples at https://github.com/craftworkgames/MonoGame.Extended-samples. The important pieces are the `NuGet.config` file and the `.mgcb` file.

## Where to next?

- Check out [the samples](https://github.com/craftworkgames/MonoGame.Extended-samples)
- Join our live [Discord](https://discord.gg/xPUEkj9)
- Read the [Documentation](http://www.monogameextended.net/docs)
- Submit an [issue on GitHub](https://github.com/craftworkgames/MonoGame.Extended/issues)
- Ask a question on [gamedev stack overflow](http://gamedev.stackexchange.com/questions/tagged/monogame-extended)
- Post on our [MonoGame community forum](http://community.monogame.net/category/extended)
- Follow development [on Patreon](https://www.patreon.com/craftworkgames)

## News

We're in the process of developing MonoGame.Extended 4.0! Stay tuned.

## Patreon Supporters
The patreon has been removed.  If you would like to support the maintainers of this project, please consider using the GitHub sponsors link for one of the maintainers.

As a special thanks to those that supported this project through Patreon in the past, their websites were linked in this readme and have been preserved below:

- [PRT Studios](http://prt-studios.com/)
- [optimuspi](http://www.optimuspi.com/)


## Special Thanks
- Matthew-Davey for letting us use the [Mercury Particle Engine](https://github.com/Matthew-Davey/mercury-particle-engine).
- John McDonald for [2D XNA Primitives](https://bitbucket.org/C3/2d-xna-primitives/wiki/Home)
- [LibGDX](https://libgdx.badlogicgames.com) for a whole lot of inspiration.
- [@prime31](https://github.com/prime31) for [`Nez`](https://github.com/prime31/Nez). Both `MonoGame.Extended` and `Nez` are in communication with each other to share ideas.
- All of our contributors!

## License

MonoGame.Extended is released under the [The MIT License (MIT)](https://github.com/craftworkgames/MonoGame.Extended/blob/master/LICENSE).
