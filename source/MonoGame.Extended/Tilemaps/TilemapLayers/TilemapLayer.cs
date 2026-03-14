using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Base class for all tilemap layers.
/// </summary>
public abstract class TilemapLayer
{
    /// <summary>
    /// Gets the name of the layer.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets the class/type identifier for this layer.
    /// </summary>
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the layer is visible.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the opacity of the layer.
    /// </summary>
    /// <value>A value between 0.0 (fully transparent) and 1.0 (fully opaque).</value>
    public float Opacity { get; set; }

    /// <summary>
    /// Gets or sets the tint color applied to the layer.
    /// </summary>
    public Color? TintColor { get; set; }

    /// <summary>
    /// Gets or sets the rendering offset for the layer.
    /// </summary>
    public Vector2 Offset { get; set; }

    /// <summary>
    /// Gets or sets the parallax scrolling factor for this layer, affecting scrolling speed relative to the camera.
    /// </summary>
    /// <value>A factor where (1,1) represents normal scrolling.</value>
    public Vector2 ParallaxFactor { get; set; }

    /// <summary>
    /// Gets the custom properties of the layer.
    /// </summary>
    public TilemapProperties Properties { get; }

    /// <summary>
    /// Gets the bounding rectangle of the layer in world coordinates.
    /// </summary>
    public abstract Rectangle Bounds { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapLayer"/> class.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    protected TilemapLayer(string name)
    {
        Name = name;
        IsVisible = true;
        Opacity = 1.0f;
        ParallaxFactor = Vector2.One;
        Properties = new TilemapProperties();
    }
}
