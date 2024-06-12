// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Provides extension methods for the <see cref="TextureRegion"/> class.
/// </summary>
public static class TextureRegionExtensions
{
    /// <summary>
    /// Gets a subregion of the specified texture region using the provided rectangle.
    /// </summary>
    /// <param name="textureRegion">The texture region to get the subregion from.</param>
    /// <param name="region">The rectangle defining the subregion.</param>
    /// <returns>A new <see cref="TextureRegion"/> representing the specified subregion.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    public static TextureRegion GetSubregion(this TextureRegion textureRegion, Rectangle region) =>
        textureRegion.GetSubregion(null, region.X, region.Y, region.Width, region.Height);

    /// <summary>
    /// Gets a subregion of the specified texture region using the provided rectangle and name.
    /// </summary>
    /// <param name="textureRegion">The texture region to get the subregion from.</param>
    /// <param name="name">The name of the new subregion.</param>
    /// <param name="region">The rectangle defining the subregion.</param>
    /// <returns>A new <see cref="TextureRegion"/> representing the specified subregion.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    public static TextureRegion GetSubregion(this TextureRegion textureRegion, string name, Rectangle region) =>
        textureRegion.GetSubregion(name, region.X, region.Y, region.Width, region.Height);

    /// <summary>
    /// Gets a subregion of the specified texture region using the provided coordinates and dimensions.
    /// </summary>
    /// <param name="textureRegion">The texture region to get the subregion from.</param>
    /// <param name="x">The top-left x-coordinate of the subregion within the texture region.</param>
    /// <param name="y">The top-left y-coordinate of the subregion within the texture region.</param>
    /// <param name="width">The width, in pixels, of the subregion.</param>
    /// <param name="height">The height, in pixels, of the subregion.</param>
    /// <returns>A new <see cref="TextureRegion"/> representing the specified subregion.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    public static TextureRegion GetSubregion(this TextureRegion textureRegion, int x, int y, int width, int height) =>
        textureRegion.GetSubregion(null, x, y, width, height);

    /// <summary>
    /// Gets a subregion of the specified texture region using the provided name, coordinates, and dimensions.
    /// </summary>
    /// <param name="textureRegion">The texture region to get the subregion from.</param>
    /// <param name="name">The name of the new subregion.</param>
    /// <param name="x">The top-left x-coordinate of the subregion within the texture region.</param>
    /// <param name="y">The top-left y-coordinate of the subregion within the texture region.</param>
    /// <param name="width">The width, in pixels, of the subregion.</param>
    /// <param name="height">The height, in pixels, of the subregion.</param>
    /// <returns>A new <see cref="TextureRegion"/> representing the specified subregion.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    public static TextureRegion GetSubregion(this TextureRegion textureRegion, string name, int x, int y, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(textureRegion);

        if (string.IsNullOrEmpty(name))
        {
            name = $"{textureRegion.Texture.Name}({x}, {y}, {width}, {height})";
        }

        Rectangle region = textureRegion.Bounds.GetRelativeRectangle(x, y, width, height);
        return new TextureRegion(textureRegion.Texture, name, region);
    }
}
