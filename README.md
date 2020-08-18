![MonoGame.Extended Logo](Logos/logo-banner-800.png)

# MonoGame.Extended

MonoGame.Extended is a set of utilities (in the form of libraries/tools) to [MonoGame](http://www.monogame.net/) that makes it easier to make games. Choose what you want, the rest stays out of your way. It makes MonoGame more awesome.

[![Build, Test, Deploy](https://github.com/craftworkgames/MonoGame.Extended/workflows/Build,%20Test,%20Deploy/badge.svg?branch=develop)](https://github.com/craftworkgames/MonoGame.Extended/actions?query=workflow%3A%22Build%2C+Test%2C+Deploy%22) [![Docs](https://img.shields.io/badge/Docs-latest-brightgreen.svg?style=flat)](http://www.monogameextended.net/)

## Getting started

The libraries and tools are distributed as NuGet packages. The libraries can be installed into your existing MonoGame project with ease using a NuGet UI or CLI.

CLI:

    dotnet add package MonoGame.Extended
    
To get access to the `develop` branch and pull-requests as packages add the private feed: `https://www.myget.org/F/lithiumtoast/api/v3/index.json` and be sure to include pre-releases when searching. 

Once you've installed one or more of the main packages you might also want to install the Content Pipeline extensions.

To do this you'll need to manually reference the `MonoGame.Extended.Content.Pipeline.dll` in the [MonoGame Content Pipeline Tool](http://docs.monogameextended.net/Installation/#referencing-the-content-pipeline-extension) because it doesn't support NuGet directly.

Alternately, if you want to try the latest and greatest you might want to [build from source](http://docs.monogameextended.net/Development/Building-from-Source/).

## Where to next?

- Check out [the demos](https://github.com/craftworkgames/MonoGame.Extended/tree/develop/Source/Demos)
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
