using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

#region Root Map

/// <summary>
/// Intermediate tilemap data produced by format parsers.
/// </summary>
public sealed class TilemapData
{
    /// <summary>
    /// Gets or sets the map name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the map width in tiles.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the map height in tiles.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the tile width in pixels.
    /// </summary>
    public int TileWidth { get; set; }

    /// <summary>
    /// Gets or sets the tile height in pixels.
    /// </summary>
    public int TileHeight { get; set; }

    /// <summary>
    /// Gets or sets the map orientation.
    /// </summary>
    public TilemapOrientation Orientation { get; set; }

    /// <summary>
    /// Gets or sets the stagger axis for staggered and hexagonal maps.
    /// </summary>
    public TilemapStaggerAxis StaggerAxis { get; set; }

    /// <summary>
    /// Gets or sets the stagger index for staggered and hexagonal maps.
    /// </summary>
    public TilemapStaggerIndex StaggerIndex { get; set; }

    /// <summary>
    /// Gets or sets the hex side length in pixels for hexagonal maps.
    /// </summary>
    public int HexSideLength { get; set; }

    /// <summary>
    /// Gets or sets the optional background color.
    /// </summary>
    public Color? BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the parallax origin X coordinate.
    /// </summary>
    public float ParallaxOriginX { get; set; }

    /// <summary>
    /// Gets or sets the parallax origin Y coordinate.
    /// </summary>
    public float ParallaxOriginY { get; set; }

    /// <summary>
    /// Gets or sets the X coordinate of this map in world space.
    /// </summary>
    public int WorldX { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of this map in world space.
    /// </summary>
    public int WorldY { get; set; }

    /// <summary>
    /// Gets or sets the depth layer of this map.
    /// Positive values represent layers above the surface; negative values represent
    /// layers below the surface.
    /// </summary>
    public int WorldDepth { get; set; }

    /// <summary>
    /// Gets the map-level custom properties.
    /// </summary>
    public List<TilemapPropertyData> Properties { get; } = new List<TilemapPropertyData>();

    /// <summary>
    /// Gets the tileset entries referenced by this map.
    /// </summary>
    public List<TilemapTilesetEntry> Tilesets { get; } = new List<TilemapTilesetEntry>();

    /// <summary>
    /// Gets the top-level layer list.
    /// </summary>
    public List<TilemapLayerData> Layers { get; } = new List<TilemapLayerData>();
}

#endregion

#region Tilesets

/// <summary>
/// A tileset reference inside a <see cref="TilemapData"/>, either an external file or inline data.
/// </summary>
public sealed class TilemapTilesetEntry
{
    /// <summary>
    /// Gets or sets the first global tile ID assigned to this tileset.
    /// </summary>
    public int FirstGlobalId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this tileset is external.
    /// When <see langword="true"/>, <see cref="ExternalPath"/> is set.
    /// When <see langword="false"/>, <see cref="InlineData"/> is set.
    /// </summary>
    public bool IsExternal { get; set; }

    /// <summary>
    /// Gets or sets the path to the external tileset file (e.g. .tsx).
    /// Only valid when <see cref="IsExternal"/> is <see langword="true"/>.
    /// </summary>
    public string ExternalPath { get; set; }

    /// <summary>
    /// Gets or sets the inline tileset data.
    /// Only valid when <see cref="IsExternal"/> is <see langword="false"/>.
    /// </summary>
    public TilemapTilesetData InlineData { get; set; }
}

/// <summary>
/// Tileset data embedded directly in the map file.
/// </summary>
public sealed class TilemapTilesetData
{
    /// <summary>
    /// Gets or sets the tileset name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the path to the tileset texture image.
    /// </summary>
    public string TexturePath { get; set; }

    /// <summary>
    /// Gets or sets the individual tile width in pixels.
    /// </summary>
    public int TileWidth { get; set; }

    /// <summary>
    /// Gets or sets the individual tile height in pixels.
    /// </summary>
    public int TileHeight { get; set; }

    /// <summary>
    /// Gets or sets the total number of tiles in the tileset.
    /// </summary>
    public int TileCount { get; set; }

    /// <summary>
    /// Gets or sets the number of tile columns in the atlas.
    /// </summary>
    public int Columns { get; set; }

    /// <summary>
    /// Gets or sets the pixel spacing between tiles.
    /// </summary>
    public int Spacing { get; set; }

    /// <summary>
    /// Gets or sets the pixel margin around the tileset edge.
    /// </summary>
    public int Margin { get; set; }

    /// <summary>
    /// Gets or sets the draw-offset X applied when rendering tiles from this tileset.
    /// </summary>
    public float DrawOffsetX { get; set; }

    /// <summary>
    /// Gets or sets the draw-offset Y applied when rendering tiles from this tileset.
    /// </summary>
    public float DrawOffsetY { get; set; }

    /// <summary>
    /// Gets the tileset-level custom properties.
    /// </summary>
    public List<TilemapPropertyData> Properties { get; } = new List<TilemapPropertyData>();

    /// <summary>
    /// Gets the per-tile metadata entries (animations, collision objects, etc.).
    /// </summary>
    public List<TilemapTileEntryData> Tiles { get; } = new List<TilemapTileEntryData>();
}

#endregion

#region Per-Tile Metadata

/// <summary>
/// Metadata for a single tile in a tileset, including animation and collision data.
/// </summary>
public sealed class TilemapTileEntryData
{
    /// <summary>
    /// Gets or sets the local tile ID within the tileset.
    /// </summary>
    public int LocalId { get; set; }

    /// <summary>
    /// Gets or sets the tile class name.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets the random-placement probability (1.0 = default).
    /// </summary>
    public float Probability { get; set; } = 1.0f;

    /// <summary>
    /// Gets the per-tile custom properties.
    /// </summary>
    public List<TilemapPropertyData> Properties { get; } = new List<TilemapPropertyData>();

    /// <summary>
    /// Gets or sets the animation, or <see langword="null"/> if the tile is not animated.
    /// </summary>
    public TilemapAnimationData Animation { get; set; }

    /// <summary>
    /// Gets the collision objects defined for this tile.
    /// </summary>
    public List<TilemapObjectData> CollisionObjects { get; } = new List<TilemapObjectData>();

    /// <summary>
    /// Gets or sets the path to the image for this tile.
    /// </summary>
    /// <remarks>
    /// Only populated for image collection tilesets where each tile has its own source image.
    /// Empty for standard atlas-based tilesets.
    /// </remarks>
    public string ImagePath { get; set; } = string.Empty;
}

/// <summary>
/// Animation data for an animated tileset tile.
/// </summary>
public sealed class TilemapAnimationData
{
    /// <summary>
    /// Gets the ordered list of animation frames.
    /// </summary>
    public List<TilemapAnimationFrameData> Frames { get; } = new List<TilemapAnimationFrameData>();
}

/// <summary>
/// A single frame in a tile animation.
/// </summary>
public sealed class TilemapAnimationFrameData
{
    /// <summary>
    /// Gets or sets the local tile ID of the frame image.
    /// </summary>
    public int TileId { get; set; }

    /// <summary>
    /// Gets or sets the frame duration in seconds.
    /// </summary>
    public float Duration { get; set; }
}

#endregion

#region Decoded Tiles

/// <summary>
/// A decoded non-empty tile entry produced during tile layer processing.
/// </summary>
public sealed class TilemapDecodedTile
{
    /// <summary>
    /// Gets the tile X coordinate within the layer grid.
    /// </summary>
    public ushort X { get; }

    /// <summary>
    /// Gets the tile Y coordinate within the layer grid.
    /// </summary>
    public ushort Y { get; }

    /// <summary>
    /// Gets the global tile ID.
    /// </summary>
    public int GlobalId { get; }

    /// <summary>
    /// Gets the flip transformation flags for this tile.
    /// </summary>
    public TilemapTileFlipFlags FlipFlags { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapDecodedTile"/> class.
    /// </summary>
    /// <param name="x">The tile X coordinate.</param>
    /// <param name="y">The tile Y coordinate.</param>
    /// <param name="globalId">The global tile ID.</param>
    /// <param name="flipFlags">The flip transformation flags.</param>
    public TilemapDecodedTile(ushort x, ushort y, int globalId, TilemapTileFlipFlags flipFlags)
    {
        X = x;
        Y = y;
        GlobalId = globalId;
        FlipFlags = flipFlags;
    }
}

#endregion

#region Properties

/// <summary>
/// A single typed custom property entry.
/// </summary>
public sealed class TilemapPropertyData
{
    /// <summary>
    /// Gets or sets the property key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the property value type.
    /// </summary>
    public TilemapPropertyType Type { get; set; }

    /// <summary>
    /// Gets or sets the string value (used when Type is String or File).
    /// </summary>
    public string StringValue { get; set; }

    /// <summary>
    /// Gets or sets the integer value (used when Type is Int or Object).
    /// </summary>
    public int IntValue { get; set; }

    /// <summary>
    /// Gets or sets the float value (used when Type is Float).
    /// </summary>
    public float FloatValue { get; set; }

    /// <summary>
    /// Gets or sets the boolean value (used when Type is Bool).
    /// </summary>
    public bool BoolValue { get; set; }

    /// <summary>
    /// Gets or sets the color value (used when Type is Color).
    /// </summary>
    public Color ColorValue { get; set; }
}

#endregion

#region Layers

/// <summary>
/// Base class for all layer data entries.
/// </summary>
public abstract class TilemapLayerData
{
    /// <summary>
    /// Gets or sets the layer name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the layer type/class name.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets whether the layer is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the layer opacity (0.0 to 1.0).
    /// </summary>
    public float Opacity { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the optional tint color.
    /// </summary>
    public Color? TintColor { get; set; }

    /// <summary>
    /// Gets or sets the X draw offset in pixels.
    /// </summary>
    public float OffsetX { get; set; }

    /// <summary>
    /// Gets or sets the Y draw offset in pixels.
    /// </summary>
    public float OffsetY { get; set; }

    /// <summary>
    /// Gets or sets the parallax scroll factor X (1.0 = moves with camera).
    /// </summary>
    public float ParallaxX { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the parallax scroll factor Y (1.0 = moves with camera).
    /// </summary>
    public float ParallaxY { get; set; } = 1.0f;

    /// <summary>
    /// Gets the layer custom properties.
    /// </summary>
    public List<TilemapPropertyData> Properties { get; } = new List<TilemapPropertyData>();
}

/// <summary>
/// Data for a tile layer.
/// </summary>
public sealed class TilemapTileLayerData : TilemapLayerData
{
    /// <summary>
    /// Gets or sets the layer width in tiles.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the layer height in tiles.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets the non-empty decoded tile entries for this layer.
    /// </summary>
    public List<TilemapDecodedTile> Tiles { get; } = new List<TilemapDecodedTile>();
}

/// <summary>
/// Data for an object layer.
/// </summary>
public sealed class TilemapObjectLayerData : TilemapLayerData
{
    /// <summary>
    /// Gets or sets the draw order for objects in this layer.
    /// </summary>
    public TilemapObjectDrawOrder DrawOrder { get; set; }

    /// <summary>
    /// Gets the objects in this layer.
    /// </summary>
    public List<TilemapObjectData> Objects { get; } = new List<TilemapObjectData>();
}

/// <summary>
/// Data for an image layer.
/// </summary>
public sealed class TilemapImageLayerData : TilemapLayerData
{
    /// <summary>
    /// Gets or sets the path to the layer image texture.
    /// </summary>
    public string TexturePath { get; set; }

    /// <summary>
    /// Gets or sets whether the image repeats horizontally.
    /// </summary>
    public bool RepeatX { get; set; }

    /// <summary>
    /// Gets or sets whether the image repeats vertically.
    /// </summary>
    public bool RepeatY { get; set; }
}

/// <summary>
/// Data for a group layer containing child layers.
/// </summary>
public sealed class TilemapGroupLayerData : TilemapLayerData
{
    /// <summary>
    /// Gets the child layers.
    /// </summary>
    public List<TilemapLayerData> Layers { get; } = new List<TilemapLayerData>();
}

/// <summary>
/// Data for a layer with no visual tiles. Format-specific data is stored
/// in <see cref="TilemapLayerData.Properties"/>.
/// </summary>
public sealed class TilemapDataLayerData : TilemapLayerData
{
    /// <summary>
    /// Gets or sets the layer width in tiles.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the layer height in tiles.
    /// </summary>
    public int Height { get; set; }
}

#endregion

#region Objects

/// <summary>
/// Base class for all tilemap object data entries.
/// </summary>
public abstract class TilemapObjectData
{
    /// <summary>
    /// Gets or sets the object ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the object name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the object class/type.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets the object X position in pixels.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Gets or sets the object Y position in pixels.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Gets or sets the object rotation in radians.
    /// </summary>
    public float Rotation { get; set; }

    /// <summary>
    /// Gets or sets whether the object is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets the object custom properties.
    /// </summary>
    public List<TilemapPropertyData> Properties { get; } = new List<TilemapPropertyData>();
}

/// <summary>
/// Data for a rectangle object.
/// </summary>
public sealed class TilemapRectangleObjectData : TilemapObjectData
{
    /// <summary>
    /// Gets or sets the rectangle width in pixels.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the rectangle height in pixels.
    /// </summary>
    public float Height { get; set; }
}

/// <summary>
/// Data for an ellipse object.
/// </summary>
public sealed class TilemapEllipseObjectData : TilemapObjectData
{
    /// <summary>
    /// Gets or sets the ellipse bounding width in pixels.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the ellipse bounding height in pixels.
    /// </summary>
    public float Height { get; set; }
}

/// <summary>
/// Data for a point object.
/// </summary>
public sealed class TilemapPointObjectData : TilemapObjectData
{
}

/// <summary>
/// Data for a polygon object.
/// </summary>
public sealed class TilemapPolygonObjectData : TilemapObjectData
{
    /// <summary>
    /// Gets the polygon vertex points relative to the object origin.
    /// </summary>
    public List<Vector2> Points { get; } = new List<Vector2>();
}

/// <summary>
/// Data for a polyline object.
/// </summary>
public sealed class TilemapPolylineObjectData : TilemapObjectData
{
    /// <summary>
    /// Gets the polyline vertex points relative to the object origin.
    /// </summary>
    public List<Vector2> Points { get; } = new List<Vector2>();
}

/// <summary>
/// Data for a tile object.
/// </summary>
public sealed class TilemapTileObjectData : TilemapObjectData
{
    /// <summary>
    /// Gets or sets the global tile ID.
    /// </summary>
    public int GlobalId { get; set; }

    /// <summary>
    /// Gets or sets the flip flags.
    /// </summary>
    public TilemapTileFlipFlags FlipFlags { get; set; }

    /// <summary>
    /// Gets or sets the tile object width in pixels.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the tile object height in pixels.
    /// </summary>
    public float Height { get; set; }
}

/// <summary>
/// Data for a text object.
/// </summary>
public sealed class TilemapTextObjectData : TilemapObjectData
{
    /// <summary>
    /// Gets or sets the text bounding width in pixels.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the text bounding height in pixels.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the font family name.
    /// </summary>
    public string FontFamily { get; set; }

    /// <summary>
    /// Gets or sets the font size in pixels.
    /// </summary>
    public int PixelSize { get; set; } = 16;

    /// <summary>
    /// Gets or sets whether word-wrap is enabled.
    /// </summary>
    public bool WordWrap { get; set; }

    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Gets or sets whether the text is bold.
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Gets or sets whether the text is italic.
    /// </summary>
    public bool Italic { get; set; }

    /// <summary>
    /// Gets or sets whether the text has an underline.
    /// </summary>
    public bool Underline { get; set; }

    /// <summary>
    /// Gets or sets whether the text has strikethrough.
    /// </summary>
    public bool Strikethrough { get; set; }

    /// <summary>
    /// Gets or sets the horizontal text alignment.
    /// </summary>
    public TilemapTextObjectHorizontalAlignment HorizontalAlign { get; set; }

    /// <summary>
    /// Gets or sets the vertical text alignment.
    /// </summary>
    public TilemapTextObjectVerticalAlignment VerticalAlign { get; set; }
}

#endregion
