// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Provides extension methods for the <see cref="SpriteBatch"/> class.
/// </summary>
public static class SpriteBatchExtensions
{
    #region ----------------------------NinePatch-----------------------------
    private static readonly Rectangle[] _patchCache = new Rectangle[9];
    private static Rectangle _rect = default;

    /// <summary>
    /// Draws a nine-patch region to the sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="ninePatchRegion">The nine-patch region.</param>
    /// <param name="destinationRectangle">The destination rectangle.</param>
    /// <param name="color">The color to tint the nine-patch region.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, NinePatch ninePatchRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
    {
        CreateDestinationPatches(ninePatchRegion, destinationRectangle);
        ReadOnlySpan<Texture2DRegion> sourcePatches = ninePatchRegion.Patches;

        for (int i = 0; i < sourcePatches.Length; i++)
        {
            Rectangle source = sourcePatches[i].Bounds;
            Rectangle destination = _patchCache[i];

            if (clippingRectangle.HasValue)
            {
                source = ClipSourceRectangle(source, destination, clippingRectangle.Value);
                destination = ClipDestinationRectangle(destination, clippingRectangle.Value);
                Draw(spriteBatch, sourcePatches[i].Texture, source, destination, color, clippingRectangle);
            }
            else
            {
                if (destination.Width > 0 && destination.Height > 0)
                {
                    spriteBatch.Draw(sourcePatches[i].Texture, destination, source, color);
                }
            }
        }
    }

    #endregion -------------------------NinePatch-----------------------------

    #region ----------------------------Sprite-----------------------------
    /// <summary>
    /// Draws a sprite to the sprite batch.
    /// </summary>
    /// <param name="sprite">The sprite to draw.</param>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="position">The position to draw the sprite.</param>
    /// <param name="rotation">The rotation of the sprite.</param>
    /// <param name="scale">The scale of the sprite.</param>
    public static void Draw(this Sprite sprite, SpriteBatch spriteBatch, Vector2 position, float rotation, Vector2 scale)
    {
        Draw(spriteBatch, sprite, position, rotation, scale);
    }

    /// <summary>
    /// Draws a sprite to the sprite batch with a transform.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="sprite">The sprite to draw.</param>
    /// <param name="transform">The transform to apply to the sprite.</param>
    public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Transform2 transform)
    {
        Draw(spriteBatch, sprite, transform.Position, transform.Rotation, transform.Scale);
    }

    /// <summary>
    /// Draws a sprite to the sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="sprite">The sprite to draw.</param>
    /// <param name="position">The position to draw the sprite.</param>
    /// <param name="rotation">The rotation of the sprite.</param>
    public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Vector2 position, float rotation = 0)
    {
        Draw(spriteBatch, sprite, position, rotation, Vector2.One);
    }

    /// <summary>
    /// Draws a sprite to the sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="sprite">The sprite to draw.</param>
    /// <param name="position">The position to draw the sprite.</param>
    /// <param name="rotation">The rotation of the sprite.</param>
    /// <param name="scale">The scale of the sprite.</param>
    public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Vector2 position, float rotation, Vector2 scale)
    {
        if (sprite == null) throw new ArgumentNullException(nameof(sprite));

        if (sprite.IsVisible)
        {
            var texture = sprite.TextureRegion.Texture;
            var sourceRectangle = sprite.TextureRegion.Bounds;
            spriteBatch.Draw(texture, position, sourceRectangle, sprite.Color * sprite.Alpha, rotation, sprite.Origin, scale, sprite.Effect, sprite.Depth);
        }
    }
    #endregion -------------------------Sprite-----------------------------

    #region ----------------------------Texture2D-----------------------------

    /// <summary>
    /// Draws a texture to the sprite batch with optional clipping.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="texture">The texture to draw.</param>
    /// <param name="sourceRectangle">The source rectangle.</param>
    /// <param name="destinationRectangle">The destination rectangle.</param>
    /// <param name="color">The color to tint the texture.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle)
    {
        if (!ClipRectangles(ref sourceRectangle, ref destinationRectangle, clippingRectangle))
            return;

        if (destinationRectangle.Width > 0 && destinationRectangle.Height > 0)
        {
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }
    }

    #endregion -------------------------Texture2D-----------------------------

    #region ----------------------------TextureRegion-----------------------------

    /// <summary>
    /// Draws a texture region to the sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="textureRegion">The texture region to draw.</param>
    /// <param name="position">The position to draw the texture region.</param>
    /// <param name="color">The color to tint the texture region.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, Texture2DRegion textureRegion, Vector2 position, Color color, Rectangle? clippingRectangle = null)
    {
        Draw(spriteBatch, textureRegion, position, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0, clippingRectangle);
    }

    /// <summary>
    /// Draws a texture region to the sprite batch with specified parameters.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="textureRegion">The texture region to draw.</param>
    /// <param name="position">The position to draw the texture region.</param>
    /// <param name="color">The color to tint the texture region.</param>
    /// <param name="rotation">The rotation of the texture region.</param>
    /// <param name="origin">The origin of the texture region.</param>
    /// <param name="scale">The scale of the texture region.</param>
    /// <param name="effects">The sprite effects to apply.</param>
    /// <param name="layerDepth">The layer depth.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, Texture2DRegion textureRegion, Vector2 position, Color color,
    float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Rectangle? clippingRectangle = null)
    {
        var sourceRectangle = textureRegion.Bounds;

        if (clippingRectangle.HasValue)
        {
            var x = (int)(position.X - origin.X);
            var y = (int)(position.Y - origin.Y);
            var width = (int)(textureRegion.Width * scale.X);
            var height = (int)(textureRegion.Height * scale.Y);
            var destinationRectangle = new Rectangle(x, y, width, height);

            if (!ClipRectangles(ref sourceRectangle, ref destinationRectangle, clippingRectangle))
            {
                // Clipped rectangle is empty, nothing to draw
                return;
            }

            position.X = destinationRectangle.X + origin.X;
            position.Y = destinationRectangle.Y + origin.Y;
        }

        spriteBatch.Draw(textureRegion.Texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
    }

    /// <summary>
    /// Draws a texture region to the sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="textureRegion">The texture region to draw.</param>
    /// <param name="destinationRectangle">The destination rectangle.</param>
    /// <param name="color">The color to tint the texture region.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, Texture2DRegion textureRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
    {
        Draw(spriteBatch, textureRegion.Texture, textureRegion.Bounds, destinationRectangle, color, clippingRectangle);
    }

    #endregion -------------------------TextureRegion-----------------------------

    #region ----------------------------Utilities-----------------------------
    private static void CreateDestinationPatches(NinePatch ninePatch, Rectangle destinationRect)
    {
        destinationRect.Deconstruct(out int x, out int y, out int width, out int height);
        ninePatch.Padding.Deconstruct(out int topPadding, out int rightPadding, out int bottomPadding, out int leftPadding);

        int midWidth = width - leftPadding - rightPadding;
        int midHeight = height - topPadding - bottomPadding;
        int top = y + topPadding;
        int right = x + width - rightPadding;
        int bottom = y + height - bottomPadding;
        int left = x + leftPadding;

        _patchCache[NinePatch.TopLeft] = new Rectangle(x, y, leftPadding, topPadding);
        _patchCache[NinePatch.TopMiddle] = new Rectangle(left, y, midWidth, topPadding);
        _patchCache[NinePatch.TopRight] = new Rectangle(right, y, rightPadding, topPadding);
        _patchCache[NinePatch.MiddleLeft] = new Rectangle(x, top, leftPadding, midHeight);
        _patchCache[NinePatch.Middle] = new Rectangle(left, top, midWidth, midHeight);
        _patchCache[NinePatch.MiddleRight] = new Rectangle(right, top, rightPadding, midHeight);
        _patchCache[NinePatch.BottomLeft] = new Rectangle(x, bottom, leftPadding, bottomPadding);
        _patchCache[NinePatch.BottomMiddle] = new Rectangle(left, bottom, midWidth, bottomPadding);
        _patchCache[NinePatch.BottomRight] = new Rectangle(right, bottom, rightPadding, bottomPadding);
    }
    private static bool ClipRectangles(ref Rectangle sourceRectangle, ref Rectangle destinationRectangle, Rectangle? clippingRectangle)
    {
        if (!clippingRectangle.HasValue)
            return true;

        var originalDestination = destinationRectangle;
        destinationRectangle = destinationRectangle.Clip(clippingRectangle.Value);

        if (destinationRectangle == Rectangle.Empty)
            return false; // Clipped rectangle is empty, nothing to draw

        var scaleX = (float)sourceRectangle.Width / originalDestination.Width;
        var scaleY = (float)sourceRectangle.Height / originalDestination.Height;

        int leftDiff = destinationRectangle.Left - originalDestination.Left;
        int topDiff = destinationRectangle.Top - originalDestination.Top;

        sourceRectangle.X += (int)(leftDiff * scaleX);
        sourceRectangle.Y += (int)(topDiff * scaleY);
        sourceRectangle.Width = (int)(destinationRectangle.Width * scaleX);
        sourceRectangle.Height = (int)(destinationRectangle.Height * scaleY);

        return true;
    }

    private static Rectangle ClipSourceRectangle(Rectangle sourceRectangle, Rectangle destinationRectangle, Rectangle clippingRectangle)
    {
        var left = (float)(clippingRectangle.Left - destinationRectangle.Left);
        var right = (float)(destinationRectangle.Right - clippingRectangle.Right);
        var top = (float)(clippingRectangle.Top - destinationRectangle.Top);
        var bottom = (float)(destinationRectangle.Bottom - clippingRectangle.Bottom);
        var x = left > 0 ? left : 0;
        var y = top > 0 ? top : 0;
        var w = (right > 0 ? right : 0) + x;
        var h = (bottom > 0 ? bottom : 0) + y;

        var scaleX = (float)destinationRectangle.Width / sourceRectangle.Width;
        var scaleY = (float)destinationRectangle.Height / sourceRectangle.Height;
        x /= scaleX;
        y /= scaleY;
        w /= scaleX;
        h /= scaleY;

        return new Rectangle((int)(sourceRectangle.X + x), (int)(sourceRectangle.Y + y), (int)(sourceRectangle.Width - w), (int)(sourceRectangle.Height - h));
    }

    private static Rectangle ClipDestinationRectangle(Rectangle destinationRectangle, Rectangle clippingRectangle)
    {
        var left = clippingRectangle.Left < destinationRectangle.Left ? destinationRectangle.Left : clippingRectangle.Left;
        var top = clippingRectangle.Top < destinationRectangle.Top ? destinationRectangle.Top : clippingRectangle.Top;
        var bottom = clippingRectangle.Bottom < destinationRectangle.Bottom ? clippingRectangle.Bottom : destinationRectangle.Bottom;
        var right = clippingRectangle.Right < destinationRectangle.Right ? clippingRectangle.Right : destinationRectangle.Right;
        return new Rectangle(left, top, right - left, bottom - top);
    }

    #endregion -------------------------Utilities-----------------------------

}
