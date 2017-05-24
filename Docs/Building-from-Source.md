# Building From Source

Ideally building from source should be as straightforward as possible. Everything should build right out of the repository. However, there are some external factors that can sometimes make things more difficult.

MonoGame.Extended uses a few newish technologies:
 - [MonoGame 3.6](http://www.monogame.net/downloads/).
 - C# 6.0 and Portable Class Library support in your IDE.
 - .NET Framework 4.5 or equivalent (Mono).
 - An up to date NuGet package manager

If you're building on Windows I highly recommend using [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx) because it supports all of the above out of the box.

There's not much more to it than that. Download the source using you're favorite git client and build.

    git clone https://github.com/craftworkgames/MonoGame.Extended.git

On the first build the solution will download several NuGet packages.

Once the code builds, you can play with the demos in the `Demos` folder.
