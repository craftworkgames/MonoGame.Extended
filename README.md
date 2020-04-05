![MonoGame.Extended Logo](https://raw.githubusercontent.com/craftworkgames/MonoGame.Extended/master/Logos/logo-banner-800.png)

# MonoGame.Extended

> :exclamation: I am currently looking for a new owner of the MonoGame.Extended project. If you're interested in taking full ownership and complete creative control over this repository, related websites and the Patreon page please [contact me](mailto:dylan@craftworkgames.com).

> I worked on MonoGame.Extended for nearly 5 years and unfortunately my heart is no longer in it. I've given this decision a lot of thought and I strongly believe handing the project over to someone who will give it the love it deserves will be best for everyone.

> A lot of things have changed in my life over the last half a decade. My family, day job and interests are no longer aligned with this project. I will continue to accept pull requests and do my best to find the right fit for the future of MonoGame.Extended.

MonoGame.Extended is an extension to the [MonoGame](http://www.monogame.net/) framework that makes it easier to make games. It's a collection of packages designed to let you choose what you want without getting in your way.

It makes MonoGame more awesome.

[![Build Status](https://dev.azure.com/dylanwilson/MonoGame.Extended/_apis/build/status/craftworkgames.MonoGame.Extended?branchName=develop)](https://dev.azure.com/dylanwilson/MonoGame.Extended/_build/latest?definitionId=1&branchName=develop) [![Docs](https://img.shields.io/badge/docs-latest-brightgreen.svg?style=flat)](http://docs.monogameextended.net/)

## Getting started

The libraries are distributed as NuGet packages and can be installed into your existing MonoGame project using your favorite NuGet Package Manager in Visual Studio, Xamarin Studio, or MonoDevelop.

For example:

    Install-Package MonoGame.Extended

Once you've installed one or more of the main packages you might also want to install the Content Pipeline extensions.

To do this you'll need to manually reference the `MonoGame.Extended.Content.Pipeline.dll` in the [MonoGame Content Pipeline Tool](http://docs.monogameextended.net/Installation/#referencing-the-content-pipeline-extension) because it doesn't support NuGet directly.

Alternately, if you want to try the latest and greatest you might want to [build from source](http://docs.monogameextended.net/Development/Building-from-Source/).

## Where to next?

- Check out [the demos](https://github.com/craftworkgames/MonoGame.Extended/tree/develop/Source/Demos)
- Join our live [Discord chat](https://discord.gg/xPUEkj9)
- Read the [Documentation](http://docs.monogameextended.net/)
- Submit an [issue on GitHub](https://github.com/craftworkgames/MonoGame.Extended/issues)
- Ask a question on [gamedev stack overflow](http://gamedev.stackexchange.com/questions/tagged/monogame-extended)
- Post on our [MonoGame community forum](http://community.monogame.net/category/extended)
- Follow development [on Patreon](https://www.patreon.com/craftworkgames)

## News

We're in the process of developing MonoGame.Extended 3.6!

There may be some confusion, pain and disruption for a while. Here's what you need to know:

- Everything that used to be in the `develop` branch is now in `master`
- NuGet packages built from `master` have been [published to nuget.org as version 1.1](https://www.nuget.org/packages?q=monogame.extended)
- There's lots of breaking changes happening to create a cleaner more useful API
- From now on we're going to (attempt) to use [Git Flow](https://gitversion.readthedocs.io/en/latest/git-branching-strategies/gitflow/)
- We're now using [cake builds](https://cakebuild.net/) so that you can build everything (including the NuGet packages) locally
- We're migrating everything to [.NET Standard!](https://www.patreon.com/posts/one-library-to-18916187)

## Patreon Supporters

Thanks to all those that support the project on Patreon! You're helping to keep the build server up and running allowing me to devote more of my time to the project.

[![image](https://cloud.githubusercontent.com/assets/3201643/17462536/f5608898-5cf3-11e6-8e81-47d6594a8d9c.png)](https://www.patreon.com/craftworkgames)

## Special thanks

As a reward to some of my patrons I've linked thier websites here:

- [PRT Studios](http://prt-studios.com/)
- [optimuspi](http://www.optimuspi.com/)

If you're not on the list and you should be please let me know!

Also thanks to

- Matthew-Davey for letting us use the [Mercury Particle Engine](https://github.com/Matthew-Davey/mercury-particle-engine).
- John McDonald for [2D XNA Primitives](https://bitbucket.org/C3/2d-xna-primitives/wiki/Home)
- [LibGDX](https://libgdx.badlogicgames.com) for a whole lot of inspiration.
- @prime31 for [Nez](https://github.com/prime31/Nez), which ideas and code bounce back and forth.
- All of our contributors!

## Design goals

- The primary goal is to make it easier to _make games_.
- Choose the features you like and the rest stays out of your way.
- A clean and consistent API familiar to MonoGame developers.
- It's _not_ a game engine, but extends the framework.
- Follow [C# coding guidelines](<https://msdn.microsoft.com/en-us/library/ms229002(v=vs.110).aspx>).

## License

MonoGame.Extended is released under the [The MIT License (MIT)](https://github.com/craftworkgames/MonoGame.Extended/blob/master/LICENSE).
