// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Represents a drawable 2D texture region with additional properties for rendering, such as position, scale,
/// rotation, and color.
/// </summary>
public class Sprite : IColorable
{
    private Texture2DRegion _textureRegion;

    /// <summary>
    /// Gets or sets a value indicating whether this sprite is visible.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the color mask used when rendering this sprite.
    /// </summary>
    /// <remarks>
    /// The color mask is applied to the sprite's texture by multiplying each pixel's color value with the specified
    /// color.  For example, setting the color to `Color.Red` will make the sprite appear as a red tint over its
    /// texture.
    /// </remarks>
    public Color Color { get; set; }

    /// <summary>
    /// Gets or sets the alpha transparency value used when rendering this sprite.
    /// </summary>
    /// <remarks>
    /// The alpha value should be between 0 (fully transparent) and 1 (fully opaque).
    /// </remarks>
    public float Alpha { get; set; }

    /// <summary>
    /// Gets or sets the layer depth used when rendering this sprite.
    /// </summary>
    /// <remarks>
    /// Sprites with higher depth values are rendered on top of those with lower depth values.
    /// </remarks>
    public float Depth { get; set; }

    /// <summary>
    /// Gets or sets the sprite effects to apply when rendering this sprite.
    /// </summary>
    /// <remarks>
    /// This specifies the desired horizontal and/or vertical flip effect of the sprite.
    /// </remarks>
    public SpriteEffects Effect { get; set; }

    /// <summary>
    /// Gets or Sets an object that contains user defined data about this sprite.
    /// </summary>
    public object Tag { get; set; }

    /// <summary>
    /// Gets the size of the sprite.
    /// </summary>
    public Point Size => TextureRegion.OriginalSize;

    /// <summary>
    /// Gets or sets the origin of this sprite.
    /// </summary>
    /// <remarks>
    /// The origin is relative to the bounds of the texture region and represents the point around which the sprite is
    /// rotated and scaled.
    /// </remarks>
    public Vector2 Origin { get; set; }

    /// <summary>
    /// Gets or sets the normalized origin of this sprite relative to its texture region.
    /// </summary>
    /// <remarks>
    /// The normalized origin represents the origin as a fraction of the texture region's UV coordinates,
    /// where (0, 0) is the top-left corner and (1, 1) is the bottom-right corner.
    /// </remarks>
    public Vector2 OriginNormalized
    {
        get { return new Vector2(Origin.X / TextureRegion.OriginalSize.Width, Origin.Y / TextureRegion.OriginalSize.Height); }
        set { Origin = new Vector2(value.X * TextureRegion.OriginalSize.Width, value.Y * TextureRegion.OriginalSize.Height); }
    }

    /// <summary>
    /// Gets or sets the source texture region of this sprite.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when setting to a null texture region.</exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the source texture of the assigned <see cref="Texture2DRegion"/> has been disposed of when setting.
    /// </exception>
    public Texture2DRegion TextureRegion
    {
        get => _textureRegion;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.Texture.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(value), $"The source {nameof(Texture2D)} of the {nameof(TextureRegion)} was disposed prior to setting this property.");
            }
            _textureRegion = value;

            if (value.OriginNormalized.HasValue)
            {
                OriginNormalized = value.OriginNormalized.Value;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class with the specified texture.
    /// </summary>
    /// <param name="texture">The source texture of the sprite.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="texture"/> parameter is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the <paramref name="texture"/> parameter was disposed of prior.
    /// </exception>
    public Sprite(Texture2D texture)
        : this(new Texture2DRegion(texture))
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class with the specified texture region.
    /// The sprite represents a renderable 2D image defined by the given texture region.
    /// </summary>
    /// <param name="textureRegion">The source texture region of the sprite.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="textureRegion"/> parameter is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the source texture of the <paramref name="textureRegion"/> parameter has been disposed of.
    /// </exception>
    public Sprite(Texture2DRegion textureRegion)
    {
        ArgumentNullException.ThrowIfNull(textureRegion);
        if (textureRegion.Texture.IsDisposed)
        {
            throw new ObjectDisposedException(nameof(textureRegion), $"The source {nameof(Texture2D)} of the {nameof(textureRegion)} was disposed prior.");
        }

        _textureRegion = textureRegion;

        Alpha = 1.0f;
        Color = Color.White;
        IsVisible = true;
        Effect = SpriteEffects.None;
        OriginNormalized = textureRegion.OriginNormalized ?? Vector2.Zero;
        Depth = 0.0f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class by copying property values from an existing sprite.
    /// </summary>
    /// <remarks>
    /// Creates a shallow copy where the new sprite shares the same <see cref="TextureRegion"/> reference.
    /// The <see cref="Tag"/> reference is copied but the object itself is not cloned.
    /// </remarks>
    /// <param name="source">The sprite to copy from.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the source texture of the <see cref="TextureRegion"/> of the provided source sprite has been disposed of.
    /// </exception>
    public Sprite(Sprite source)
    {
        ArgumentNullException.ThrowIfNull(source);
        ObjectDisposedException.ThrowIf(source.TextureRegion.Texture.IsDisposed, source.TextureRegion.Texture);

        _textureRegion = source._textureRegion;
        Alpha = source.Alpha;
        Color = source.Color;
        IsVisible = source.IsVisible;
        Effect = source.Effect;
        Depth = source.Depth;
        Origin = source.Origin;
        Tag = source.Tag;
    }

    /// <summary>
    /// Gets the bounding rectangle of the sprite in world/screen coordinates.
    /// </summary>
    /// <param name="transform">The transformation of the sprite.</param>
    /// <returns>The bounding rectangle of the sprite in world/screen coordinates.</returns>
    public RectangleF GetBoundingRectangle(Transform2 transform)
    {
        return GetBoundingRectangle(transform.Position, transform.Rotation, transform.Scale);
    }

    /// <summary>
    /// Gets the bounding rectangle of the sprite in world/screen coordinates.
    /// </summary>
    /// <param name="position">The xy-coordinate position of the sprite in world/screen coordinates.</param>
    /// <param name="rotation">The rotation, in radians, of the sprite.</param>
    /// <param name="scale">The scale of the sprite.</param>
    /// <returns>The bounding rectangle of the sprite in world/screen coordinates.</returns>
    public RectangleF GetBoundingRectangle(Vector2 position, float rotation, Vector2 scale)
    {
        var corners = GetCorners(position, rotation, scale);
        var min = new Vector2(corners.Min(i => i.X), corners.Min(i => i.Y));
        var max = new Vector2(corners.Max(i => i.X), corners.Max(i => i.Y));
        return new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
    }

    /// <summary>
    /// Gets the corner points of the sprite in world/screen coordinates.
    /// </summary>
    /// <param name="position">The xy-coordinate position of the sprite in world/screen coordinates.</param>
    /// <param name="rotation">The rotation, in radians, of the sprite.</param>
    /// <param name="scale">The scale of the sprite.</param>
    /// <returns>The corner points of the sprite in world/screen coordinates.</returns>
    public Vector2[] GetCorners(Vector2 position, float rotation, Vector2 scale)
    {
        var min = -Origin;
        var max = min + new Vector2(TextureRegion.OriginalSize.Width, TextureRegion.OriginalSize.Height);
        var offset = position;

        if (scale != Vector2.One)
        {
            min *= scale;
            max = max * scale;
        }

        var corners = new Vector2[4];
        corners[0] = min;
        corners[1] = new Vector2(max.X, min.Y);
        corners[2] = max;
        corners[3] = new Vector2(min.X, max.Y);

        if (rotation != 0)
        {
            var matrix = Matrix.CreateRotationZ(rotation);

            for (var i = 0; i < 4; i++)
            {
                corners[i] = Vector2.Transform(corners[i], matrix);
            }
        }

        for (var i = 0; i < 4; i++)
        {
            corners[i] += offset;
        }

        return corners;
    }

    /// <summary>
    /// Creates a shallow copy of this sprite with the same texture region and property values.
    /// </summary>
    /// <remarks>
    /// The returned sprite shares the same <see cref="TextureRegion"/> reference.
    /// The <see cref="Tag"/> reference is copied but the object itself is not cloned.
    /// </remarks>
    /// <returns>A new <see cref="Sprite"/> instance with copied property values.</returns>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the source texture of the <see cref="TextureRegion"/> of this sprite has been disposed of.
    /// </exception>
    public Sprite Clone()
    {
        return new Sprite(this);
    }
}
