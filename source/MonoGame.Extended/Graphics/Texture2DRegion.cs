// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Represents a region of a texture.
/// </summary>
public class Texture2DRegion
{
    /// <summary>
    /// Gets the name assigned to this texture region when it was created.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the texture associated with this texture region.
    /// </summary>
    public Texture2D Texture { get; }

    /// <summary>
    /// Gets the top-left x-coordinate of the texture region within the texture.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Gets the top-left y-coordinate of the texture region within the texture.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Gets the width, in pixels, of the texture region.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height, in pixels,  of the texture region.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the size of the texture region.
    /// </summary>
    public Size Size { get; }

    /// <summary>
    /// Gets or sets the user-defined data associated with this texture region.
    /// </summary>
    public object Tag { get; set; }

    /// <summary>
    /// Gets the bounds of the texture region within the texture.
    /// </summary>
    public Rectangle Bounds { get; }

    /// <summary>
    /// Gets the top UV coordinate of the texture region.
    /// </summary>
    public float TopUV { get; }

    /// <summary>
    /// Gets the right UV coordinate of the texture region.
    /// </summary>
    public float RightUV { get; }

    /// <summary>
    /// Gets the bottom UV coordinate of the texture region.
    /// </summary>
    public float BottomUV { get; }

    /// <summary>
    /// Gets the left UV coordinate of the texture region.
    /// </summary>
    public float LeftUV { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DRegion"/> class representing the entire texture.
    /// </summary>
    /// <param name="texture">The texture to create the region from.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="texture"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if <paramref name="texture"/> has been disposed prior.
    /// </exception>
    public Texture2DRegion(Texture2D texture)
        : this(texture, 0, 0, texture.Width, texture.Height, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DRegion"/> class representing the entire texture with the
    /// specified name.
    /// </summary>
    /// <param name="texture">The texture to create the region from.</param>
    /// <param name="name">The name of the texture region.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="texture"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if <paramref name="texture"/> has been disposed prior.
    /// </exception>
    public Texture2DRegion(Texture2D texture, string name)
        : this(texture, 0, 0, texture.Bounds.Width, texture.Bounds.Height, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DRegion"/> class with the specified region of the texture.
    /// </summary>
    /// <param name="texture">The texture to create the region from.</param>
    /// <param name="region">The region of the texture to use.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="texture"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if <paramref name="texture"/> has been disposed prior.
    /// </exception>
    public Texture2DRegion(Texture2D texture, Rectangle region)
        : this(texture, region.X, region.Y, region.Width, region.Height, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DRegion"/> class with the specified region of the texture.
    /// </summary>
    /// <param name="texture">The texture to create the region from.</param>
    /// <param name="x">The top-left x-coordinate of the region within the texture.</param>
    /// <param name="y">The top-left y-coordinate of the region within the texture.</param>
    /// <param name="width">The width, in pixels, of the region.</param>
    /// <param name="height">The height, in pixels, of the region.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="texture"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if <paramref name="texture"/> has been disposed prior.
    /// </exception>
    public Texture2DRegion(Texture2D texture, int x, int y, int width, int height)
        : this(texture, x, y, width, height, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DRegion"/> class with the specified region of the texture and
    /// name.
    /// </summary>
    /// <param name="texture">The texture to create the region from.</param>
    /// <param name="region">The region of the texture to use.</param>
    /// <param name="name">The name of the texture region.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="texture"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if <paramref name="texture"/> has been disposed prior.
    /// </exception>
    public Texture2DRegion(Texture2D texture, Rectangle region, string name)
        : this(texture, region.X, region.Y, region.Width, region.Height, name) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DRegion"/> class with the specified region of the texture and
    /// name.
    /// </summary>
    /// <param name="texture">The texture to create the region from.</param>
    /// <param name="x">The top-left x-coordinate of the region within the texture.</param>
    /// <param name="y">The top-left y-coordinate of the region within the texture.</param>
    /// <param name="width">The width, in pixels, of the region.</param>
    /// <param name="height">The height, in pixels, of the region.</param>
    /// <param name="name">The name of the texture region.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="texture"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if <paramref name="texture"/> has been disposed prior.
    /// </exception>
    public Texture2DRegion(Texture2D texture,int x, int y, int width, int height, string name)
    {
        ArgumentNullException.ThrowIfNull(texture);
        if (texture.IsDisposed)
        {
            throw new ObjectDisposedException(nameof(texture));
        }

        if (string.IsNullOrEmpty(name))
        {
            name = $"{texture.Name}({x}, {y}, {width}, {height})";
        }

        Name = name;
        Texture = texture;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Bounds = new Rectangle(x, y, width, height);
        Size = new Size(width, height);
        TopUV = Bounds.Top / (float)texture.Height;
        RightUV = Bounds.Right / (float)texture.Width;
        BottomUV = Bounds.Bottom / (float)texture.Height;
        LeftUV = Bounds.Left / (float)texture.Width;
    }

    //  Used for unit tests only
    internal Texture2DRegion(string name, Rectangle bounds) : this(name, bounds.X, bounds.Y, bounds.Width, bounds.Height) { }

    //  Used for unit tests only
    internal Texture2DRegion(string name, int x, int y, int width, int height)
    {
        Name = name;
        Texture = null;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Bounds = new Rectangle(x, y, width, height);
        Size = new Size(width, height);
        TopUV = Bounds.Top / 1.0f;
        RightUV = Bounds.Right / 1.0f;
        BottomUV = Bounds.Bottom / 1.0f;
        LeftUV = Bounds.Left / 1.0f;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Name ?? string.Empty} {Bounds}";
    }
}
