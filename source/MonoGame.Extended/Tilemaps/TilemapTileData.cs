using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents metadata and properties for a specific tile in a tileset.
/// </summary>
public class TilemapTileData
{
    /// <summary>
    /// Gets the local tile ID within the tileset.
    /// </summary>
    public int LocalId { get; }

    /// <summary>
    /// Gets or sets the class/type identifier for the tile.
    /// </summary>
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the probability of this tile being used in random tile generation.
    /// </summary>
    /// <value>A value between 0.0 and 1.0, where 1.0 is the default probability.</value>
    public float Probability { get; set; }

    /// <summary>
    /// Gets the custom properties of the tile.
    /// </summary>
    public TilemapProperties Properties { get; }

    /// <summary>
    /// Gets or sets the animation data for animated tiles.
    /// </summary>
    public TilemapTileAnimation Animation { get; set; }

    /// <summary>
    /// Gets the collision objects associated with this tile.
    /// </summary>
    public List<TilemapObject> CollisionObjects { get; }

    /// <summary>
    /// Gets or sets a custom image texture for this tile.
    /// </summary>
    /// <remarks>
    /// When set, this texture overrides the tileset's atlas texture for this specific tile.
    /// </remarks>
    public Texture2D CustomImage { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapTileData"/> class.
    /// </summary>
    /// <param name="localId">The local tile ID within the tileset.</param>
    public TilemapTileData(int localId)
    {
        LocalId = localId;
        Properties = new TilemapProperties();
        CollisionObjects = new List<TilemapObject>();
        Probability = 1.0f;
    }
}
