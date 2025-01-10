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
        get { return new Vector2(Origin.X / TextureRegion.Width, Origin.Y / TextureRegion.Height); }
        set { Origin = new Vector2(value.X * TextureRegion.Width, value.Y * TextureRegion.Height); }
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
        OriginNormalized = Vector2.Zero;
        Depth = 0.0f;
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
        var max = min + new Vector2(TextureRegion.Width, TextureRegion.Height);
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
}
