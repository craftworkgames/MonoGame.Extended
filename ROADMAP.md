# Roadmap of MonoGame.Extended

## [Katabasis]([Katabasis](https://github.com/craftworkgames/katabasis))

### Highlights

* MonoGame.Extended is the bridge for MonoGame and [Katabasis](https://github.com/craftworkgames/katabasis), a fork of FNA.
* Date: Summer, 2021
* Runtime: .NET 5 Desktop (Windows, macOS, Linux)

### Forking MonoGame

#### Why

For building extensions on top of MonoGame, MonoGame itself can be limiting and frustrating. Some frustration is that some "core" types are at odds with extensions. Take the `Rectangle` type in the `Microsoft.Xna.Framework` namespace as an example. `RectangleF` was introduced to be an equivalent to `Rectangle` but for floats (`float`) instead of 32-bit signed integers (`int`). This raises a couple questions however... Should there be 16-bit integer version? What about signed vs unsigned integers? Assume the answer is yes to having these types in the spirit of creating extensions... Then what would that type be called exactly? It would be much easier if the extensions library owned the `Rectangle` type instead so not only the naming is consistent, but also the fields, properties, and methods are consistent aswell! The example is small, but the idea of this frustration with the core types bleeds to many larger problems. In order to solve the very concrete example mentioned above, to solve the larger problems with creating extensions, and to increase the quality of MonoGame.Extended, *FNA* is forked.

#### XNA

MonoGame/FNA has heritage being XNA. It's my opinion that XNA on it's own has some poor choices in design for *game-dev*. To add insult to injury, XNA is a DirectX 9 API. While older graphics APIs are ubiquitous (e.g. OpenGL), the world is moving on! With newer APIs how modern real-time rendering can technically be solved is very different, challenging, and in some cases more complex. (If you are interested in learning more about the newer modern APIs [check out this blog post comparing them](https://alain.xyz/blog/comparison-of-modern-graphics-apis) by Alain Galvan which is updated frequently.) This difference however opens the door to possible innovations. If MonoGame is ever to reach for these newer graphics APIs, it's my opinion that the very first step is that XNA has to go. What does this mean? It means that `IUpdateable` axed! `Rectangle` gone! `DrawableGameComponent` deleted! `BoundingSphere` poof! It's my opinion that if you *really* needed these types you would, and should, just implement them yourself. Or, get this, copy paste the code from the Extended library!

To make this a possibility for version 4.0, MonoGame.Extended forks FNA. Why FNA? Because MonoGame is evolving, but perhaps not in the direction that makes it easier to write extensions on top of. MonoGame in my opinion is adding a bunch of additions that go beyond XNA and bleed into the domain of extensions. FNA is focused on being a drop in replacement for XNA 4.0 with minimal or carefully considered additions to the APIs. This makes it a good, solid, choice to rip out the gut bad stuff of XNA and leave the good stuff.

### Deliverable

Version 4.0 will be focused on desktop only. Porting games to other devices is challenging. In my opinion, development of a game prototype should start on desktop, the ubiquitous dev-environment. Once you have something going you should *then* think about developing for mobile, console, etc. .NET 5 will be iteresting to see in action however as Microsoft has intended [.NET 5 to be the next .NET Core that will unify desktop, mobile, browser, consoles, and other platforms](https://devblogs.microsoft.com/dotnet/introducing-net-5/). 

Of course, then there is the MonoGame.Extended itself. For version 4.0, minimal changes the MonoGame.Extended code will be made. For requested new features, version 4.x will be the window of opportunity.

