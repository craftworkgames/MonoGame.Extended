# Building the MonoGame PCL

This guide is about building the MonoGame [Portable Class Library](https://msdn.microsoft.com/en-us/library/gg597391) (PCL) found in the [Dependencies folder](https://github.com/craftworkgames/MonoGame.Extended/tree/develop/Dependencies) of the project.

**Note**: This is NOT a guide about building MonoGame.Extended from source. You won't need this guide unless you're trying to update the dependencies.

One of the challenges with using MonoGame is getting your project set up so that you can separate platform-specific and platform-independent code. One way, which we are using with MonoGame.Extended is to compile against a PCL ([Portable Class Library](https://msdn.microsoft.com/en-us/library/gg597391)). Instead of creating a platform specific binary of MonoGame.Extended for each platform, a cross-platform library is used and the platform specific code is substituted in later. A PCL is created through a certain set of tools such as [Visual Studio](https://www.visualstudio.com) on Windows, [Xamarin Studio](https://xamarin.com/studio) on Windows and Mac, or [MonoDevelop](http://www.monodevelop.com) on Windows, Mac and Linux. Since PCLs are designed to be cross-platform they can be created with one toolchain and used through a different toolchain.

With MonoGame, making a PCL isn't so easy since a large amount of platform-specific code is mixed in with other code, so you can't (at the moment) make a PCL of MonoGame directly. There are a few other issues as well, but I won't go into those. The solution to this is to use a tool to scrape all of the platform-specific implementation details from a specific MonoGame platform library. It doesn't really matter if it's the WindowsGL platform version that is used for this, but it's recommended.

## Install Tools

The first step in generating the PCL is to install the required tools. If you have Visual Studio, Xamarin Studio, you already have the tools. For Linux, you have to install the tools from Mono. To install Mono on a specific Linux distribution follow [their directions on the Mono website](http://www.mono-project.com/docs/getting-started/install/linux/). The important packages to install are `mono-complete`, which installs the complete mono runtime and development tools (including the tools in `mono-devel` if I remember correctly), and also the `referenceassemblies-pcl` package, which installs tools and references required for PCL generation and usage.

## Get MonoGame Source

Next, clone down the copy of [MonoGame from GitHub](https://github.com/mono/MonoGame) that you want to generate a PCL for. Remember that a PCL that is generated will probably only be usable with binaries made from that exact same commit. 

## Build MonoGame

After you have cloned the repository, follow the MonoGame build instructions and use Protobuild to generate the set of `.sln` files and `.csproj` files for your platform. Open up the solution, then compile it.

## "Piranha" Binary into PCL

Finally, you need a copy of @Ark-kun's [Piranha tool, which "chews" binaries to make them into PCLs](https://github.com/Ark-kun/Piranha). Download a copy of his repository from GitHub, then build it. Navigate to where Piranha is built and open up command prompt if you are on Windows (PROTIP: shift-right-click on the Explorer background and select "Open command window here"), or Terminal if you are on Mac or Linux (MAC USERS PROTIP: type "cd" into terminal then drag and drop the folder into the terminal window and hit return). Enter the following command where `PATH_TO_MONOGAME_FRAMEWORK_DLL` is the path to the target `MonoGame.Framework.dll`:

Windows Users:
`Piranha.exe make-portable-skeleton -p ".NETPortable,Version=v4.5,Profile=Profile111" -i "PATH_TO_MONOGAME_FRAMEWORK_DLL" -o MonoGame.Framework.dll`

Mac and Linux Users:
`mono Piranha.exe make-portable-skeleton -p ".NETPortable,Version=v4.5,Profile=Profile111" -i "PATH_TO_MONOGAME_FRAMEWORK_DLL" -o MonoGame.Framework.dll`

Note that the .NET framework portable profile might need to be changed to match your project.

Piranha should of completed successfully and generated a portable `MonoGame.Framework.dll`.
