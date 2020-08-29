![MonoGame.Extended Logo](Logos/logo-banner-800.png)

# MonoGame.Extended

MonoGame.Extended is a set of utilities (in the form of libraries/tools) to [MonoGame](http://www.monogame.net/) that makes it easier to make games. Choose what you want, the rest stays out of your way. It makes MonoGame more awesome.

[![Build, Test, Deploy](https://github.com/craftworkgames/MonoGame.Extended/workflows/Build,%20Test,%20Deploy/badge.svg?branch=develop)](https://github.com/craftworkgames/MonoGame.Extended/actions?query=workflow%3A%22Build%2C+Test%2C+Deploy%22) [![Docs](https://img.shields.io/badge/Docs-latest-brightgreen.svg?style=flat)](http://www.monogameextended.net/)

## Getting started

Code is distributed as NuGet packages in the form of libraries (`.dll` files). You can easily install the NuGet packages into your existing MonoGame project using the NuGet Package Manager UI in Visual Studio or by using the command line interface (CLI) in a terminal.

> :wrench: CLI 
    `dotnet add package MonoGame.Extended`
    
> :hammer: To use the content pipeline extensions, you will need to edit your `.mgcb` file to reference the `.dll`. To see an example of how to do this with NuGet see the samples at https://github.com/craftworkgames/MonoGame.Extended-samples. The important pieces are the `NuGet.config` file and the `.mgcb` file.

> :eyes: To access pre-releases including rolling builds of the repository after every commit or PR commit, you can see add the NuGet feed `https://www.myget.org/F/lithiumtoast/api/v3/index.json`. See https://github.com/craftworkgames/MonoGame.Extended-samples with the `NuGet.config` file for an example of how to setup the feed.

## Where to next?

- Check out [the samples](https://github.com/craftworkgames/MonoGame.Extended-samples)
- Join our live [Discord](https://discord.gg/xPUEkj9)
- Read the [Documentation](http://www.monogameextended.net/docs)
- Submit an [issue on GitHub](https://github.com/craftworkgames/MonoGame.Extended/issues)
- Ask a question on [gamedev stack overflow](http://gamedev.stackexchange.com/questions/tagged/monogame-extended)
- Post on our [MonoGame community forum](http://community.monogame.net/category/extended)
- Follow development [on Patreon](https://www.patreon.com/craftworkgames)

## News

We're in the process of developing MonoGame.Extended 3.8! Stay tuned.

## Patreon Supporters

Thanks to all those that support the project on Patreon! You're helping to keep the project alive.
[![image](https://cloud.githubusercontent.com/assets/3201643/17462536/f5608898-5cf3-11e6-8e81-47d6594a8d9c.png)](https://www.patreon.com/craftworkgames)

## Special thanks

As a reward to some of my patrons I've linked thier websites here:

- [PRT Studios](http://prt-studios.com/)
- [optimuspi](http://www.optimuspi.com/)

If you're not on the list and you should be please let me know!

Also thanks to:

- Matthew-Davey for letting us use the [Mercury Particle Engine](https://github.com/Matthew-Davey/mercury-particle-engine).
- John McDonald for [2D XNA Primitives](https://bitbucket.org/C3/2d-xna-primitives/wiki/Home)
- [LibGDX](https://libgdx.badlogicgames.com) for a whole lot of inspiration.
- [@prime31](https://github.com/prime31) for [`Nez`](https://github.com/prime31/Nez). Both `MonoGame.Extended` and `Nez` are in communication with each other to share ideas.
- All of our contributors!

## License

MonoGame.Extended is released under the [The MIT License (MIT)](https://github.com/craftworkgames/MonoGame.Extended/blob/master/LICENSE).
