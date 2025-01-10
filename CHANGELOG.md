# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [Unreleased]
> [!NOTE]
> Unreleased changes exist in the current `develop` branch but have not been pushed as either a stable or prerelease NuGet package.
>

## [4.0.4]
## Fixed
- Resolved issue where the `Sprite.Origin` property was incorrectly being set to the center origin of the sprite in the constructor. [@AristurtleDev](https://github.com/AristurtleDev) [#938](https://github.com/craftworkgames/MonoGame.Extended/pull/969)

## [4.0.3]
## Fixed
- Resoled issue where `Matrix3x2.Decompose` returned incorrect values for transformation [@Std-Enigma](https://github.com/Std-Enigma) [#941](https://github.com/craftworkgames/MonoGame.Extended/pull/941)
- Resolved bug in NinePatch where padding calculations were incorrect [@Dwergi](https://github.com/Dwergi) [#945](https://github.com/craftworkgames/MonoGame.Extended/pull/945)
- Resoled bug in `Texture2DExtensions.CreateNinePatch` where source rectangles were calculated that overlapped. [@greenstack](https://github.com/greenstack) [#948](https://github.com/craftworkgames/MonoGame.Extended/pull/948)

## Changed
- `BitmapFont` uses `TitleContainer` to load stream of file [@Dwergi](https://github.com/Dwergi) [#946](https://github.com/craftworkgames/MonoGame.Extended/pull/946)
- Revert UV code for `Sprite.OriginNormalize`, resolving incorrect calculations [@kaltinril](https://github.com/kaltinril) [#951](https://github.com/craftworkgames/MonoGame.Extended/pull/951)


## [4.0.2]
### Fixed
- Resolved issue when reading .particle files for a ParticleEffect that cause a recursion loop bug creating a stack overflow exception [@AristurtleDev](https://github.com/AristurtleDev) [#938](https://github.com/craftworkgames/MonoGame.Extended/pull/938)

## [4.0.1]
- `VortexModifier` now properly makes use of the `MaxSpeed` property. [@AristurtleDev](https://github.com/AristurtleDev)    [#921](https://github.com/craftworkgames/MonoGame.Extended/pull/921)
- `rayNearDistance` and `rayFarDistance` are now properly swapped in `PrimitivesHelper.IntersectsSlab()`. [@AristurtleDev](https://github.com/AristurtleDev) [#922](https://github.com/craftworkgames/MonoGame.Extended/pull/922)
- Resolved issue where an `ArgumentNullException` was thrown when loading BitmapFonts due to the `bmfFile.Path` value not being set. [@AristurtleDev](https://github.com/AristurtleDev) [#924](https://github.com/craftworkgames/MonoGame.Extended/pull/924)
- Resolved issue for FNA in `ColorExtensions.FromArgb` due to FNA `Color` struct not having a constructor that accepts a uint packed value. [@ValorZard](https://github.com/ValorZard) [#925](https://github.com/craftworkgames/MonoGame.Extended/pull/925)
- Updated for MonoGame 3.8.2.1105 release [@AristurtleDev](https://github.com/AristurtleDev) [#926](https://github.com/craftworkgames/MonoGame.Extended/pull/926)
  - MonoGame References updated to 3.8.2.1105
  - MonoGame.Extended target framework updated to net8.0
- `Vector2Extensions.Rotate` has been marked deprecated for MonoGame targets only (KNI and FNA still use this). With the release of MonoGame 3.8.2.1105, MonoGame now has a built in method for rotating a `Vector2`. [@AristurtleDev](https://github.com/AristurtleDev) [#926](https://github.com/craftworkgames/MonoGame.Extended/pull/926)
-

## [4.0.0]
### Added
- Added unit test for `PolyGon.Contains` to ensure consistency in expected return values for edge touching compared to MonoGame. [@AristurtleDev](https://github.com/AristurtleDev) [#871](https://github.com/craftworkgames/MonoGame.Extended/pull/871)
- Added unit test for `BitmapFont.MeasureString` to validate that it correctly accounts for trailing whitespace. [@AristurtleDev](https://github.com/AristurtleDev) [#876](https://github.com/craftworkgames/MonoGame.Extended/pull/876)
- Added unit test for `ContentReaderExtensions.GetRelativeAssetName` to validate that asset name sanitization resolves to correct path. [@AristurtleDev](https://github.com/AristurtleDev) [#877](https://github.com/craftworkgames/MonoGame.Extended/pull/877)
- `MonoGame.Extended.Content.Pipeline.targets` was added to the **MonoGame.Extended.Content.Pipeline** nuget.  This target contains a task that will automatically copy the required dll references for the MGCB Editor to the `/Content/references` directory inside the project root for easier access. [@AristurtleDev](https://github.com/AristurtleDev) [#855] (https://github.com/craftworkgames/MonoGame.Extended/pull/885)
- Updated `ComponentManager` to implement `IEnumerable<ComponentMapper>` [@mbowersjr](https://github.com/mbowersjr) [#888](https://github.com/craftworkgames/MonoGame.Extended/pull/888)

### Changed
- `Matrix2` was renamed `Matrix3x2` to better reflect what it actually is. [@AristurtleDev](https://github.com/AristurtleDev) [#870](https://github.com/craftworkgames/MonoGame.Extended/pull/870)
- Renamed `Size2` to `SizeF` to keep naming consistency of types. [@AristurtleDev](https://github.com/AristurtleDev) [#873](https://github.com/craftworkgames/MonoGame.Extended/pull/873)
- Updated solution (.sln) to Visual Studio 2022 [@nkast](https://github.com/nkast) [#880](https://github.com/craftworkgames/MonoGame.Extended/pull/880)
- Updated project types to .Net.Sdk [@nkast](https://github.com/nkast) [#880](https://github.com/craftworkgames/MonoGame.Extended/pull/880)
- Use `System.IO.Compression.ZLibStream` [@nkast](https://github.com/nkast) [#822](https://github.com/craftworkgames/MonoGame.Extended/pull/882)
- `BitmapFont` refactored. Now supports all three BMFont export types (XML, Text, and Binary). [@AristurtleDev](https://github.com/AristurtleDev) [#877](https://github.com/craftworkgames/MonoGame.Extended/pull/887)
- `TextureRegion2D` and `NinePatchRegion2D` moved to `MonoGame.Exteneded.Graphics` namespace.  [@AristurtleDev](https://github.com/AristurtleDev)
- `TextureREgion2D` and `NinePatchRegion2D` renamed to `TextureRegion` and `NinePatchRegion`. [@AristurtleDev](https://github.com/AristurtleDev)

### Removed
- All projects now output build artifacts to a common `.artifacts` directory at the root of the repository. [@AristurtleDev](https://github.com/AristurtleDev) [#865](https://github.com/craftworkgames/MonoGame.Extended/pull/865)
- Dependency on NewtonSoft.Json was completely removed in favor of using dotnet's `System.Text.Json` [@AristurtleDev](https://github.com/AristurtleDev) [#869](https://github.com/craftworkgames/MonoGame.Extended/pull/869)
- Removed `Size3`.  It was redundant since  `Microsoft.Xna.Framework.Vector3` exists. [@AristurtleDev](https://github.com/AristurtleDev) [#872](https://github.com/craftworkgames/MonoGame.Extended/pull/872)
- Removed `Point3`.  It was redundant since `Microsoft.Xna.Framework.Vector3` exists. [@AristurtleDev](https://github.com/AristurtleDev) [#874](https://github.com/craftworkgames/MonoGame.Extended/pull/874)
- Removed `Point2`.  It was redundant since `Microsoft.Xna.Framework.Vector2` exists. [@AristurtleDev](https://github.com/AristurtleDev) [#875](https://github.com/craftworkgames/MonoGame.Extended/pull/875)
- Removed unnecessary dependency introduced by previous pull requests [@nkast](https://github.com/nkast) [#881](https://github.com/craftworkgames/MonoGame.Extended/pull/881)


### Fixed
- Resolved `Matrix3x2` (formally `Matrix2`) not accounting for negative when calculating scale. [@AristurtleDev](https://github.com/AristurtleDev) [#870](https://github.com/craftworkgames/MonoGame.Extended/pull/870)
- Implemented standard `IDispose` pattern to resolve for particles [@AristurtleDev](https://github.com/AristurtleDev) [#879](https://github.com/craftworkgames/MonoGame.Extended/pull/879)

## [3.9.0-prerelease.4]

> [!WARNING]
> Please note that during this release, the entire project structure was reorganized.  More importantly, build artifacts like dlls and packages created by projects are now output in a `.artifacts` directory at the root of the repository.

### Added
- All projects now output build artifacts to a common `.artifacts` directory at the root of the repository. [@AristurtleDev](https://github.com/AristurtleDev) [#865](https://github.com/craftworkgames/MonoGame.Extended/pull/865)

### Changed
- Entire project structure was reorganized to resemble the MonoGame project so there is consistency for users who work from source. [@AristurtleDev](https://github.com/AristurtleDev) [#856](https://github.com/craftworkgames/MonoGame.Extended/pull/858)

### Removed
- Roadmap.md was removed, it was no longer valid. [@AristurtleDev](https://github.com/AristurtleDev) [#861](https://github.com/craftworkgames/MonoGame.Extended/pull/861)

## [3.9.0-prerelease3]

### Added
- Added `.editorconfig` to project [@LokiMidgard](https://github.com/LokiMidgard) [#708](https://github.com/craftworkgames/MonoGame.Extended/pull/708)
- Include XML Documentation in NuGet packages [@Apostolique](https://github.com/Apostolique) [#733](https://github.com/craftworkgames/MonoGame.Extended/pull/733)
- Added `ComponentMapper.OnPut` and `ComponentMapper.OnDelete` events. [@GrizzlyEnglish](https://github.com/GrizzlyEnglish) [#744](https://github.com/craftworkgames/MonoGame.Extended/pull/744)
- Added ability to configure layer depth for auto triggered particle effects. [@codymanix](https://github.com/codymanix) [$756](https://github.com/craftworkgames/MonoGame.Extended/pull/756)
- Added support for `class` attribute of objects in Tiled. [@carlfriess](https://github.com/carlfriess) [#766](https://github.com/craftworkgames/MonoGame.Extended/pull/776)
- Added `VelocityInterpolator` to allow particles to change velocity as they age. [@slakedclay](https://github.com/slakedclay) [#798](https://github.com/craftworkgames/MonoGame.Extended/pull/798)
- Implemented parallax factor for Tiled maps [@Gandifil](https://github.com/Gandifil) [#801](https://github.com/craftworkgames/MonoGame.Extended/pull/801)
- Added unit tests for `SpriteSheetAnimation`. [@DavidFidge](https://github.com/DavidFidge) [#806](https://github.com/craftworkgames/MonoGame.Extended/pull/806)
- Added `MouseStateExtended.IsButtonPressed`, `MouseStateExtended.IsButtonReleased`, `KeyboardStateExtended.IsKeyPressed`, `KeyboardStateExtended.IsKeyReleased` as replacements for deprecated properties. [@LilithSilver](https://github.com/LilithSilver) [#815](https://github.com/craftworkgames/MonoGame.Extended/pull/815)
- Adds support of nested class properties in Tiled. [@KatDevsGames](https://github.com/KatDevsGames) [#817](https://github.com/craftworkgames/MonoGame.Extended/pull/817)
- Add support for class filed on Tiled map, layers, and tilemaps. [@KatDevsGames](https://github.com/KatDevsGames) [#818](https://github.com/craftworkgames/MonoGame.Extended/pull/818)
- Added overload method for `PrimitiveDrawing.DrawSolidCircle` that adds a `fillColor` parameter. [@Asthegor](https://github.com/Asthegor) [#819](https://github.com/craftworkgames/MonoGame.Extended/pull/819)
- Add support for Collection of Images tilest type [@Gandifil](https://github.com/Gandifil) [#829](https://github.com/craftworkgames/MonoGame.Extended/pull/829)
- Added `RectangleF` serializer [@Gandifil](https://github.com/Gandifil) [#833](https://github.com/craftworkgames/MonoGame.Extended/pull/833)
- Added `OrientedRectangle` for improved collision detection. [@toore](https://github.com/toore) [#840](https://github.com/craftworkgames/MonoGame.Extended/pull/840)

### Changed
- `EllipseF` changed form `class` to `struct` and now implements `IEquatable` and `IShapeF`. [@simonantonio](https://github.com/simonantonio) [#718](https://github.com/craftworkgames/MonoGame.Extended/pull/718)
- `TiledMapTileLayer.TryGetTile` returns `false` if the `x` parameter is outside the bounds of the tile. [@LokiMidgard](https://github.com/LokiMidgard) [755](https://github.com/craftworkgames/MonoGame.Extended/pull/755)
- Updated MonoGame dependency from 3.8.0 to 3.8.1.303. [@lithiumtoast](https://github.com/lithiumtoast) [#692](https://github.com/craftworkgames/MonoGame.Extended/pull/692)
- Update from `netcore` to `net6` [@Emersont1](https://github.com/Emersont1) [#766](https://github.com/craftworkgames/MonoGame.Extended/pull/776)
- Unit tests swapped to using XUnit. [@toore](https://github.com/toore) [#785](https://github.com/craftworkgames/MonoGame.Extended/pull/785)
- Power of two values added for `MouseButton` enum so that `HasFlag` can be used. [@dezoitodemaio](https://github.com/dezoitodemaio) [#799](https://github.com/craftworkgames/MonoGame.Extended/pull/799)
- Collisions update [@Gandifil](https://github.com/Gandifil) [#824](https://github.com/craftworkgames/MonoGame.Extended/pull/824), [#825](https://github.com/craftworkgames/MonoGame.Extended/pull/825)
- Tween enhancements [@Gandifil](https://github.com/Gandifil) [#835](https://github.com/craftworkgames/MonoGame.Extended/pull/835), [#836](https://github.com/craftworkgames/MonoGame.Extended/pull/836), [#837](https://github.com/craftworkgames/MonoGame.Extended/pull/837)
- Throw `UndefinedLayerException` when providing `null` in `CollisionComponent` constructor due. [@safoster88](https://github.com/safoster88) [#839](https://github.com/craftworkgames/MonoGame.Extended/pull/839)
- Updated NewtonSoft.Json dependency version [@AristurtleDev](https://github.com/AristurtleDev) [#854](https://github.com/craftworkgames/MonoGame.Extended/pull/854)

### Deprecated
- Deprecated `MouseStateExtended.WasButtonJustDown`, `MouseStateExtended.WasButtonJustUp`, `KeyboardStateExtended.WasKeyJustDown`, and `KeyboardStateExtended.WasKeyJustUp`. [@LilithSilver](https://github.com/LilithSilver) [#815](https://github.com/craftworkgames/MonoGame.Extended/pull/815)

### Removed
- Removed MyGet feed deployment.  All NuGets, including pre-releases, are now pushed to NuGet.org only. [@AristurtleDev](https://github.com/AristurtleDev) [#856](https://github.com/craftworkgames/MonoGame.Extended/pull/856)

### Fixed
- Resolved `FadeTransition` glitch [@topnik-code](https://github.com/topnik-code) [#699](https://github.com/craftworkgames/MonoGame.Extended/pull/699)
- Resolved issue where relative paths for tileset sources in .tmx caused Tiled map importer to fail. [@merthsoft](https://github.com/merthsoft) [#713](https://github.com/craftworkgames/MonoGame.Extended/pull/713)
- Fixed `ParticleEmitter` always using the same random seed. [@mikeparker](https://github.com/mikeparker) [#717](https://github.com/craftworkgames/MonoGame.Extended/pull/717)
- Resolved half pixel offset bug in `TiledMapLayerModelBuilder`. [@kryzp](https://github.com/kryzp) [#721]([#721](https://github.com/craftworkgames/MonoGame.Extended/pull/721))
- Fixed issue where `Entity.Detach()` did not invoke `EntityChanged` until next update cycle. [@SjaakAlvarez](https://github.com/SjaakAlvarez) [#724](https://github.com/craftworkgames/MonoGame.Extended/pull/724)
- Fixed issue due to `Color.TransparentBlack` being removed from MonoGame 3.8.1.303 [@Pizt0lmnk](https://github.com/Pizt0lmnk) [#735](https://github.com/craftworkgames/MonoGame.Extended/pull/735)
- Correctly resolve relative path of texture in `TextureAtlasJsonConverter`. [@merthsoft](https://github.com/merthsoft) [#746](https://github.com/craftworkgames/MonoGame.Extended/pull/746)
- Fixed issue where `KeyboardExtended.GetState()` and `MouseExtended.GetState()` rewrites previous state value. [@KRC2000](https://github.com/KRC2000) [#770](https://github.com/craftworkgames/MonoGame.Extended/pull/770)
- Fixed crash on Android when loading `AnimationSprite`. [@garakutanokiseki](https://github.com/garakutanokiseki) [#782](https://github.com/craftworkgames/MonoGame.Extended/pull/782)
- Fixed issue when transforming a `RectangleF` not calculating bounding rectangle from the rotated original as intended. [@toore](https://github.com/toore) [#787](https://github.com/craftworkgames/MonoGame.Extended/pull/787)
- Effects used for Tiled rendering rebuilt using MonoGame 3.8.1.303. [@mattj1](https://github.com/mattj1) [#789](https://github.com/craftworkgames/MonoGame.Extended/pull/789)
- Resolved `ComponentManager.Destroy` throwing a `NullReferenceException`. [@Jeff425](https://github.com/Jeff425) [#794](https://github.com/craftworkgames/MonoGame.Extended/pull/794)
- Resolved `CollisionManager` reporting objects as colliding even after object(s) have been moved out of collision bounds [@toore](https://github.com/toore) [#795](https://github.com/craftworkgames/MonoGame.Extended/pull/795)
- Fixed looping animations incorrectly when an update contains large elapsed time in `SpriteSheetAnimation`. [@DavidFidge](https://github.com/DavidFidge) [#806](https://github.com/craftworkgames/MonoGame.Extended/pull/806)
- `SpiteSheetAnimation` ping pong waits until frame delay is complete on re-entry of the first frame before firing `Complete` event. @(DavidFidge)
- `SpriteSheetAnimation` ping pong finishes on the first frame instead of potentially on a different frame. [@DavidFidge](https://github.com/DavidFidge) [#806](https://github.com/craftworkgames/MonoGame.Extended/pull/806)
- Fix allocations during `foreach` iterations of `Bag<T>`. [@LilithSilver](https://github.com/LilithSilver) [#814](https://github.com/craftworkgames/MonoGame.Extended/pull/814)
- Decreased allocation overhead for `KeyboardStateExtended` [@LilithSilver](https://github.com/LilithSilver) [#820](https://github.com/craftworkgames/MonoGame.Extended/pull/820)
- Resolve single digit type in HSl Lerp function [@Apllify](https://github.com/Apllify) [#834](https://github.com/craftworkgames/MonoGame.Extended/pull/834)
- Fixed issue where animated tiles that were flipped were rendering incorrectly [@tigurx](https://github.com/tigurx) [#846](https://github.com/craftworkgames/MonoGame.Extended/pull/846)
- Resolve infinite recursion glitch for `Shape.Intersects(this IShapeF, IShapeF). [@AristurtleDev](https://github.com/AristurtleDev) [#852](https://github.com/craftworkgames/MonoGame.Extended/pull/852)
