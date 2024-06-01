# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [Unreleased]



### Added
- Added unit test for `PolyGon.Contains` to ensure consistency in expected return values for edge touching compared to MonoGame. (@AristurtleDev) [#871](https://github.com/craftworkgames/MonoGame.Extended/pull/871)
- Added unit test for `BitmapFont.MeasureString` to validate that it correctly accounts for trailing whitespace. (@AristurtleDev) [#876](https://github.com/craftworkgames/MonoGame.Extended/pull/876)
- Added unit test for `ContentReaderExtensions.GetRelativeAssetName` to validate that asset name sanitization resolves to correct path. (@AristurtleDev) [#877](https://github.com/craftworkgames/MonoGame.Extended/pull/877)

### Changed
- `Matrix2` was renamed `Matrix3x2` to better reflect what it actually is. (@AristurtleDev) [#870](https://github.com/craftworkgames/MonoGame.Extended/pull/870)
- Renamed `Size2` to `SizeF` to keep naming consistency of types. (@AristurtleDev) [#873](https://github.com/craftworkgames/MonoGame.Extended/pull/873)
- Updated solution (.sln) to Visual Studio 2022 (@nkast) [#880](https://github.com/craftworkgames/MonoGame.Extended/pull/880)
- Updated project types to .Net.Sdk (@nkast) [#880](https://github.com/craftworkgames/MonoGame.Extended/pull/880)
- Use `System.IO.Compression.ZLibStream` (@nkast) [#822](https://github.com/craftworkgames/MonoGame.Extended/pull/882)

### Deprecated

### Removed
- All projects now output build artifacts to a common `.artifacts` directory at the root of the repository. (@AristurtleDev) [#865](https://github.com/craftworkgames/MonoGame.Extended/pull/865)
- Dependency on NewtonSoft.Json was completely removed in favor of using dotnet's `System.Text.Json` (@AristurtleDev) [#869](https://github.com/craftworkgames/MonoGame.Extended/pull/869)
- Removed `Size3`.  It was redundant since  `Microsoft.Xna.Framework.Vector3` exists. (@AristurtleDev) [#872](https://github.com/craftworkgames/MonoGame.Extended/pull/872)
- Removed `Point3`.  It was redundant since `Microsoft.Xna.Framework.Vector3` exists. (@AristurtleDev) [#874](https://github.com/craftworkgames/MonoGame.Extended/pull/874)
- Removed `Point2`.  It was redundant since `Microsoft.Xna.Framework.Vector2` exists. (@AristurtleDev) [#875](https://github.com/craftworkgames/MonoGame.Extended/pull/875)
- Removed unneccessary dependency introduced by previous pull requests (@nkast) [#881](https://github.com/craftworkgames/MonoGame.Extended/pull/881)


### Fixed
- Resolved `Matrix3x2` (formally `Matrix2`) not accounting for negative when calculating scale. (@AristurtleDev) [#870](https://github.com/craftworkgames/MonoGame.Extended/pull/870)
- Implemented standard `IDispose` pattern to resolve for particles (@AristurtleDev) [#879](https://github.com/craftworkgames/MonoGame.Extended/pull/879)

### Security


## [3.9.0-prerelease.4]

> [!WARNING]
> Please note that during this release, the entire project structure was reorganized.  More importantly, build artifacts like dlls and packages created by projects are now output in a `.artifacts` directory at the root of the repository.

### Added
- All projects now output build artifacts to a common `.artifacts` directory at the root of the repository. (@AristurtleDev) [#865](https://github.com/craftworkgames/MonoGame.Extended/pull/865)

### Changed
- Entire project structure was reorganized to resemble the MonoGame project so there is consistency for users who work from source. (@AristurtleDev) [#856](https://github.com/craftworkgames/MonoGame.Extended/pull/858)

### Removed
- Roadmap.md was removed, it was no longer valid. (@AristurtleDev) [#861](https://github.com/craftworkgames/MonoGame.Extended/pull/861)

## [3.9.0-prerelease3]

### Added
- Added `.editorconfig` to project (@LokiMidgard) [#708](https://github.com/craftworkgames/MonoGame.Extended/pull/708)
- Include XML Documentation in NuGet packages (@Apostolique) [#733](https://github.com/craftworkgames/MonoGame.Extended/pull/733)
- Added `ComponentMapper.OnPut` and `ComponentMapper.OnDelete` events. (@GrizzlyEnglish) [#744](https://github.com/craftworkgames/MonoGame.Extended/pull/744)
- Added ability to configure layer depth for auto triggered particle effects. (@codymanix) [$756](https://github.com/craftworkgames/MonoGame.Extended/pull/756)
- Added support for `class` attribute of objects in Tiled. (@carlfriess) [#766](https://github.com/craftworkgames/MonoGame.Extended/pull/776)
- Added `VelocityInterpolator` to allow particles to change velocity as they age. (@slakedclay) [#798](https://github.com/craftworkgames/MonoGame.Extended/pull/798)
- Implemented parallax factor for Tiled maps (@Gandifil) [#801](https://github.com/craftworkgames/MonoGame.Extended/pull/801)
- Added unit tests for `SpriteSheetAnimation`. (@DavidFidge) [#806](https://github.com/craftworkgames/MonoGame.Extended/pull/806)
- Added `MouseStateExtended.IsButtonPressed`, `MouseStateExtended.IsButtonReleased`, `KeyboardStateExtended.IsKeyPressed`, `KeyboardStateExtended.IsKeyReleased` as replacements for deprecated properties. (@LilithSilver) [#815](https://github.com/craftworkgames/MonoGame.Extended/pull/815)
- Adds support of nested class properties in Tiled. (@KatDevsGames) [#817](https://github.com/craftworkgames/MonoGame.Extended/pull/817)
- Add support for class filed on Tiled map, layers, and tilemaps. (@KatDevGames) [#818](https://github.com/craftworkgames/MonoGame.Extended/pull/818)
- Added overload method for `PrimitiveDrawing.DrawSolidCircle` that adds a `fillColor` parameter. (@Asthegor) [#819](https://github.com/craftworkgames/MonoGame.Extended/pull/819)
- Add support for Collection of Images tilest type (@Gandifil) [#829](https://github.com/craftworkgames/MonoGame.Extended/pull/829)
- Added `RectangleF` serializer (@Gandifil) [#833](https://github.com/craftworkgames/MonoGame.Extended/pull/833)
- Added `OrientedRectangle` for improved collision detection. (@toore) [#840](https://github.com/craftworkgames/MonoGame.Extended/pull/840)

### Changed
- `EllipseF` changed form `class` to `struct` and now implements `IEquatable` and `IShapeF`. (@simonantonio) [#718](https://github.com/craftworkgames/MonoGame.Extended/pull/718)
- `TiledMapTileLayer.TryGetTile` returns `false` if the `x` parameter is outside the bounds of the tile. (@LokiMidgard) [755](https://github.com/craftworkgames/MonoGame.Extended/pull/755)
- Updated MonoGame dependency from 3.8.0 to 3.8.1.303. (@lithiumtoast) [#692](https://github.com/craftworkgames/MonoGame.Extended/pull/692)
- Update from `netcore` to `net6` (@Emersont1) [#766](https://github.com/craftworkgames/MonoGame.Extended/pull/776)
- Unit tests swapped to using XUnit. (@toore) [#785](https://github.com/craftworkgames/MonoGame.Extended/pull/785)
- Power of two values added for `MouseButton` enum so that `HasFlag` can be used. (@dezoitodemaio) [#799](https://github.com/craftworkgames/MonoGame.Extended/pull/799)
- Collisions update (@Gandifil) [#824](https://github.com/craftworkgames/MonoGame.Extended/pull/824), [#825](https://github.com/craftworkgames/MonoGame.Extended/pull/825)
- Tween enhancements (@Gandifil) [#835](https://github.com/craftworkgames/MonoGame.Extended/pull/835), [#836](https://github.com/craftworkgames/MonoGame.Extended/pull/836), [#837](https://github.com/craftworkgames/MonoGame.Extended/pull/837)
- Throw `UndefinedLayerException` when providing `null` in `CollisionComponent` constructor due. (@safoster88) [#839](https://github.com/craftworkgames/MonoGame.Extended/pull/839)
- Updated NewtonSoft.Json dependency version (@AristurtleDev) [#854](https://github.com/craftworkgames/MonoGame.Extended/pull/854)

### Deprecated
- Deprecated `MouseStateExtended.WasButtonJustDown`, `MouseStateExtended.WasButtonJustUp`, `KeyboardStateExtended.WasKeyJustDown`, and `KeyboardStateExtended.WasKeyJustUp`. (@LilithSilver) [#815](https://github.com/craftworkgames/MonoGame.Extended/pull/815)

### Removed
- Removed MyGet feed deployment.  All NuGets, including pre-releases, are now pushed to NuGet.org only. (@AristurtleDev) [#856](https://github.com/craftworkgames/MonoGame.Extended/pull/856)

### Fixed
- Resolved `FadeTransition` glitch (@topnik-code) [#699](https://github.com/craftworkgames/MonoGame.Extended/pull/699)
- Resolved issue where relative paths for tileset sources in .tmx caused Tiled map importer to fail. (@merthsoft) [#713](https://github.com/craftworkgames/MonoGame.Extended/pull/713)
- Fixed `ParticleEmitter` always using the same random seed. (@mikeparker) [#717](https://github.com/craftworkgames/MonoGame.Extended/pull/717)
- Resolved half pixel offset bug in `TiledMapLayerModelBuilder`. (@kryzp) [#721]([#721](https://github.com/craftworkgames/MonoGame.Extended/pull/721))
- Fixed issue where `Entity.Detach()` did not invoke `EntityChanged` until next update cycle. (@SjaakAlvarez) [#724](https://github.com/craftworkgames/MonoGame.Extended/pull/724)
- Fixed issue due to `Color.TransparentBlack` being removed from MonoGame 3.8.1.303 (@Pizt0lmnk) [#735](https://github.com/craftworkgames/MonoGame.Extended/pull/735)
- Correctly resolve relative path of texture in `TextureAtlasJsonConverter`. (@merthsoft) [#746](https://github.com/craftworkgames/MonoGame.Extended/pull/746)
- Fixed issue where `KeyboardExtended.GetState()` and `MouseExtended.GetState()` rewrites previous state value. (@KRC2000) [#770](https://github.com/craftworkgames/MonoGame.Extended/pull/770)
- Fixed crash on Android when loading `AnimationSprite`. (@garakutanokiseki) [#782](https://github.com/craftworkgames/MonoGame.Extended/pull/782)
- Fixed issue when transforming a `RectangleF` not calculating bounding rectangle from the rotated original as intended. (@toore) [#787](https://github.com/craftworkgames/MonoGame.Extended/pull/787)
- Effects used for Tiled rendering rebuilt using MonoGame 3.8.1.303. (@mattj1) [#789](https://github.com/craftworkgames/MonoGame.Extended/pull/789)
- Resolved `ComponentManager.Destroy` throwing a `NullReferenceException`. (@Jeff425) [#794](https://github.com/craftworkgames/MonoGame.Extended/pull/794)
- Resolved `CollisionManager` reporting objects as colliding even after object(s) have been moved out of collision bounds (@toore) [#795](https://github.com/craftworkgames/MonoGame.Extended/pull/795)
- Fixed looping animations incorrectly when an update contains large elapsed time in `SpriteSheetAnimation`. (@DavidFidge) [#806](https://github.com/craftworkgames/MonoGame.Extended/pull/806)
- `SpiteSheetAnimation` ping pong waits until frame delay is complete on re-entry of the first frame before firing `Complete` event. @(DavidFidge)
- `SpriteSheetAnimation` ping pong finishes on the first frame instead of potentially on a different frame. (@DavidFidge) [#806](https://github.com/craftworkgames/MonoGame.Extended/pull/806)
- Fix allocations during `foreach` iterations of `Bag<T>`. (@LilithSilver) [#814](https://github.com/craftworkgames/MonoGame.Extended/pull/814)
- Decreased allocation overhead for `KeyboardStateExtended` (@LilithSilver) [#820](https://github.com/craftworkgames/MonoGame.Extended/pull/820)
- Resolve single digit type in HSl Lerp function (@Apllify) [#834](https://github.com/craftworkgames/MonoGame.Extended/pull/834)
- Fixed issue where animated tiles that were flipped were rendering incorrectly (@tigurx) [#846](https://github.com/craftworkgames/MonoGame.Extended/pull/846)
- Resolve infinite recursion glitch for `Shape.Intersects(this IShapeF, IShapeF). (@AristurtleDev) [#852](https://github.com/craftworkgames/MonoGame.Extended/pull/852)
