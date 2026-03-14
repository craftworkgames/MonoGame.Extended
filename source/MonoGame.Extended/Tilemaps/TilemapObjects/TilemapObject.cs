using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Base class for all tilemap objects
/// </summary>
public abstract class TilemapObject
{
    /// <summary>
    /// Gets the unique identifier for this object.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets or sets the name of the object.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the class/type identifier for the object.
    /// </summary>
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the position of the object in world coordinates.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the rotation of the object in radians.
    /// </summary>
    public float Rotation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the object is visible.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets the custom properties of the object.
    /// </summary>
    public TilemapProperties Properties { get; }

    /// <summary>
    /// Gets the axis-aligned bounding box that encloses the object.
    /// </summary>
    public abstract BoundingBox2D Bounds { get; }

    protected TilemapObject(int id, Vector2 position)
    {
        Id = id;
        Position = position;
        IsVisible = true;
        Properties = new TilemapProperties();
    }
}
