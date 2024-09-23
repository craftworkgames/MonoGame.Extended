// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Provides extension methods for the <see cref="Texture2DRegion"/> class.
/// </summary>
public static class Texture2DRegionExtensions
{
    /// <summary>
    /// Gets a subregion of the specified texture region using the provided rectangle.
    /// </summary>
    /// <param name="textureRegion">The texture region to get the subregion from.</param>
    /// <param name="region">The rectangle defining the subregion.</param>
    /// <returns>A new <see cref="Texture2DRegion"/> representing the specified subregion.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if source texture of the <paramref name="textureRegion"/> has been disposed prior.
    /// </exception>
    public static Texture2DRegion GetSubregion(this Texture2DRegion textureRegion, Rectangle region) =>
        textureRegion.GetSubregion(region.X, region.Y, region.Width, region.Height, null);

    /// <summary>
    /// Gets a subregion of the specified texture region using the provided rectangle and name.
    /// </summary>
    /// <param name="textureRegion">The texture region to get the subregion from.</param>
    /// <param name="region">The rectangle defining the subregion.</param>
    /// <param name="name">The name of the new subregion.</param>
    /// <returns>A new <see cref="Texture2DRegion"/> representing the specified subregion.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if source texture of the <paramref name="textureRegion"/> has been disposed prior.
    /// </exception>
    public static Texture2DRegion GetSubregion(this Texture2DRegion textureRegion, string name, Rectangle region) =>
        textureRegion.GetSubregion(region.X, region.Y, region.Width, region.Height, name);

    /// <summary>
    /// Gets a subregion of the specified texture region using the provided coordinates and dimensions.
    /// </summary>
    /// <param name="textureRegion">The texture region to get the subregion from.</param>
    /// <param name="x">The top-left x-coordinate of the subregion within the texture region.</param>
    /// <param name="y">The top-left y-coordinate of the subregion within the texture region.</param>
    /// <param name="width">The width, in pixels, of the subregion.</param>
    /// <param name="height">The height, in pixels, of the subregion.</param>
    /// <returns>A new <see cref="Texture2DRegion"/> representing the specified subregion.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if source texture of the <paramref name="textureRegion"/> has been disposed prior.
    /// </exception>
    public static Texture2DRegion GetSubregion(this Texture2DRegion textureRegion, int x, int y, int width, int height) =>
        textureRegion.GetSubregion(x, y, width, height, null);

    /// <summary>
    /// Gets a subregion of the specified texture region using the provided name, coordinates, and dimensions.
    /// </summary>
    /// <param name="textureRegion">The texture region to get the subregion from.</param>
    /// <param name="x">The top-left x-coordinate of the subregion within the texture region.</param>
    /// <param name="y">The top-left y-coordinate of the subregion within the texture region.</param>
    /// <param name="width">The width, in pixels, of the subregion.</param>
    /// <param name="height">The height, in pixels, of the subregion.</param>
    /// <param name="name">The name of the new subregion.</param>
    /// <returns>A new <see cref="Texture2DRegion"/> representing the specified subregion.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if source texture of the <paramref name="textureRegion"/> has been disposed prior.
    /// </exception>
    public static Texture2DRegion GetSubregion(this Texture2DRegion textureRegion, int x, int y, int width, int height, string name)
    {
        ArgumentNullException.ThrowIfNull(textureRegion);

        if (string.IsNullOrEmpty(name))
        {
            name = $"{textureRegion.Texture.Name}({x}, {y}, {width}, {height})";
        }

        Rectangle region = textureRegion.Bounds.GetRelativeRectangle(x, y, width, height);
        return new Texture2DRegion(textureRegion.Texture, region, name);
    }

    /// <summary>
    /// Creates a nine-patch from the specified texture region with the specified padding.
    /// </summary>
    /// <param name="textureRegion">The texture region to create the nine-patch from.</param>
    /// <param name="padding">The padding to apply to each side of the nine-patch.</param>
    /// <returns>A new <see cref="NinePatch"/> representing the created nine-patch.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if source texture of the <paramref name="textureRegion"/> has been disposed prior.
    /// </exception>
    public static NinePatch CreateNinePatch(this Texture2DRegion textureRegion, Thickness padding) =>
        textureRegion.CreateNinePatch(padding.Left, padding.Top, padding.Right, padding.Bottom);

    /// <summary>
    /// Creates a nine-patch from the specified texture region with uniform padding.
    /// </summary>
    /// <param name="textureRegion">The texture region to create the nine-patch from.</param>
    /// <param name="padding">The padding to apply uniformly to all sides of the nine-patch.</param>
    /// <returns>A new <see cref="NinePatch"/> representing the created nine-patch.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if source texture of the <paramref name="textureRegion"/> has been disposed prior.
    /// </exception>
    public static NinePatch CreateNinePatch(this Texture2DRegion textureRegion, int padding) =>
        textureRegion.CreateNinePatch(padding, padding, padding, padding);

    /// <summary>
    /// Creates a nine-patch from the specified texture region with non-uniform padding.
    /// </summary>
    /// <param name="textureRegion">The texture region to create the nine-patch from.</param>
    /// <param name="leftPadding">The padding on the left side of the nine-patch.</param>
    /// <param name="topPadding">The padding on the top side of the nine-patch.</param>
    /// <param name="rightPadding">The padding on the right side of the nine-patch.</param>
    /// <param name="bottomPadding">The padding on the bottom side of the nine-patch.</param>
    /// <returns>A new <see cref="NinePatch"/> representing the created nine-patch.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegion"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if source texture of the <paramref name="textureRegion"/> has been disposed prior.
    /// </exception>
    public static NinePatch CreateNinePatch(this Texture2DRegion textureRegion, int leftPadding, int topPadding, int rightPadding, int bottomPadding)
    {
        Texture2DRegion[] patches = new Texture2DRegion[9];

        int middleWidth = textureRegion.Width - leftPadding - rightPadding;
        int middleHeight = textureRegion.Height - topPadding - bottomPadding;
        int rightX = textureRegion.Width - rightPadding;
        int bottomY = textureRegion.Height - bottomPadding;

        patches[NinePatch.TopLeft] = textureRegion.GetSubregion(0, 0, leftPadding, topPadding);
        patches[NinePatch.TopMiddle] = textureRegion.GetSubregion(leftPadding, 0, middleWidth, topPadding);
        patches[NinePatch.TopRight] = textureRegion.GetSubregion(rightX, 0, rightPadding, topPadding);

        patches[NinePatch.MiddleLeft] = textureRegion.GetSubregion(0, topPadding, leftPadding, middleHeight);
        patches[NinePatch.Middle] = textureRegion.GetSubregion(leftPadding, topPadding, middleWidth, middleHeight);
        patches[NinePatch.MiddleRight] = textureRegion.GetSubregion(rightX, topPadding, rightPadding, middleHeight);

        patches[NinePatch.BottomLeft] = textureRegion.GetSubregion(0, bottomY, leftPadding, bottomPadding);
        patches[NinePatch.BottomMiddle] = textureRegion.GetSubregion(leftPadding, bottomY, middleWidth, bottomPadding);
        patches[NinePatch.BottomRight] = textureRegion.GetSubregion(rightX, bottomY, rightPadding, bottomPadding);

        return new NinePatch(patches);
    }
}
