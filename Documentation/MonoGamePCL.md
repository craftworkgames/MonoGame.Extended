One of the challenges with using MonoGame is getting your project set up so that you can seperate platform-specific and platform-independent code. One way, which we are using with MonoGame.Extended since all of our code is completely cross-platform, is to compile against a PCL binary rather than a tradtional binary, then substituting the traditional, platform-specific version of MonoGame later. A PCL, or Portable Class Library, is compiled through a certain set of tools provided by Microsoft (nominally for Visual Studio, but available for other IDEs such as Xamarin on Windows, and for other platforms through Mono).

With Monogame, making a PCL isn't so easy since a large amount of platform-specific code is mixed in with other code, so you can't (at the moment) make a PCL of Monogame directly. There are a few other issues as well, but I won't go into those. The solution to this is to use a tool to scrape all of the platform-specific implementation details from, in this case, MonoGame.WindowsGL. Simon Jackson (aka @DDReaper) originally chose the WindowsGL version of MonoGame because it is the most tested version at this moment, among other reasons.

The first step in generating a PCL is to install the required tools. They should come by default with Visual Studio, but on Linux you have to install them from Mono. Follow [their directions on the Mono website](http://www.mono-project.com/docs/getting-started/install/linux/).

The important packages to install are `mono-complete`, which installs the complete mono runtime and development tools (including the tools in `mono-devel` if I remember correctly), and also the `referenceassemblies-pcl` package, which installs tools and references required for PCL generation and usage.

Next, clone down the copy of Monogame that you want to generate a PCL for from github. Remember that a PCL that is generated will probably only be usable with binaries made from that exact same commit. 

After you have cloned the repository, follow the Monogame build instructions and use Protobuild to generate the Windows set of .sln files and .csproj files. Open up the solution, then compile it, specifically the WindowsGL binary.

Finally, you need a copy of @Ark-kun's [Piranha tool, which "chews" binaries to make them into PCLs](https://github.com/Ark-kun/Piranha). Download a copy of his repository from Github, then build it. Take the built executable and drop it and assorted files into a folder inside of your copy of the Monogame source. This should be located at `ThirdParty/Piranha`, and an example can be seen [in my fork of Monogame in the PCL_v3.4 branch](https://github.com/WardBenjamin/MonoGame/tree/PCL_v3.4/ThirdParty/Piranha).

Next, add either the [GenerateMonoGamePCL.cmd](https://raw.githubusercontent.com/WardBenjamin/MonoGame/PCL_v3.4/ThirdParty/Piranha/GenerateMonoGamePCL.cmd) or [GenerateMonoGamePCL.sh](https://raw.githubusercontent.com/WardBenjamin/MonoGame/PCL_v3.4/ThirdParty/Piranha/GenerateMonoGamePCL.sh) script to the same folder that you added the Piranha binary to. You can also run the script inside of those files directly from command line.

The last step is to run this script. After it has completed, you should have a working PCL binary. I tested the v3.4 version that I made against the WindowsGL and Linux versions of Monogame.

If you want to download an example, or to quickly generate a Monogame 3.4 PCL, I have a branch set up for that on Github, in my fork of Monogame. All of my code should be able to be found in [my PCL_v3.4 branch](https://github.com/WardBenjamin/MonoGame/tree/PCL_v3.4).
