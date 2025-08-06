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
            Texture2DRegion sourceRegion = sourcePatches[i];
            Rectangle destinationRect = _patchCache[i];

            if (clippingRectangle.HasValue)
            {
                sourceRegion = ClipSourceRegion(sourceRegion, destinationRect, clippingRectangle.Value);
                destinationRect = ClipDestinationRectangle(destinationRect, clippingRectangle.Value);
            }
            if (sourceRegion != null && !destinationRect.IsEmpty)
            {
                Draw(spriteBatch, sourceRegion, destinationRect, color);
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
            Draw(
                spriteBatch,
                sprite.TextureRegion,
                position,
                sprite.Color * sprite.Alpha,
                rotation,
                sprite.Origin,
                scale,
                sprite.Effect,
                sprite.Depth
            );
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
        var offset = origin - textureRegion.Offset;
        Vector2 sourceScale = scale;

        // Handle rotated texture regions
        if (textureRegion.IsRotated)
        {
            var rotatedOrigin = new Vector2(origin.Y, -origin.X);
            var rotatedTrimOffset = new Vector2(textureRegion.Offset.Y, -textureRegion.Offset.X);
            var shiftByWidth = new Vector2(textureRegion.Size.Width, 0);
            offset = rotatedTrimOffset - rotatedOrigin + shiftByWidth;

            // Swap scale axes and adjust rotation for rotated regions
            sourceScale = new Vector2(scale.Y, scale.X);
            rotation -= (float)Math.PI / 2;

            switch (effects)
            {
                case SpriteEffects.FlipHorizontally: effects = SpriteEffects.FlipVertically; break;
                case SpriteEffects.FlipVertically: effects = SpriteEffects.FlipHorizontally; break;
                default: break; // nothing to do if flipped in both directions
            }
        }

        if (clippingRectangle.HasValue)
        {
            float scaledOffsetX = (origin.X - textureRegion.Offset.X) * scale.X;
            float scaledOffsetY = (origin.Y - textureRegion.Offset.Y) * scale.Y;

            var x = (int)(position.X - scaledOffsetX);
            var y = (int)(position.Y - scaledOffsetY);

            var width = (int)(textureRegion.Width * sourceScale.X);
            var height = (int)(textureRegion.Height * sourceScale.Y);
            if (textureRegion.IsRotated)
            {
                (width, height) = (height, width);
            }
            var destinationRectangle = new Rectangle(x, y, width, height);

            if (!ClipRectangles(ref sourceRectangle, ref destinationRectangle, clippingRectangle, textureRegion.IsRotated))
            {
                // Clipped rectangle is empty, nothing to draw
                return;
            }
            
            if (textureRegion.IsRotated)
            {
                offset.X -= (y + height - destinationRectangle.Bottom) / sourceScale.X;
                offset.Y += (position.X - (destinationRectangle.X + scaledOffsetX)) / sourceScale.Y;
            }
            else
            {
                offset.X += (position.X - (destinationRectangle.X + scaledOffsetX)) / sourceScale.X;
                offset.Y += (position.Y - (destinationRectangle.Y + scaledOffsetY)) / sourceScale.Y;
            }
        }

        spriteBatch.Draw(textureRegion.Texture, position, sourceRectangle, color, rotation, offset, sourceScale, effects, layerDepth);
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
        float scaleX = (float)destinationRectangle.Width / textureRegion.OriginalSize.Width;
        float scaleY = (float)destinationRectangle.Height / textureRegion.OriginalSize.Height;
        Draw(spriteBatch, textureRegion, new Vector2(destinationRectangle.X, destinationRectangle.Y), color, 0, Vector2.Zero, new Vector2(scaleX, scaleY), SpriteEffects.None, 0, clippingRectangle);
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
    private static bool ClipRectangles(ref Rectangle sourceRectangle, ref Rectangle destinationRectangle, Rectangle? clippingRectangle, bool rotatedSource = false)
    {
        if (!clippingRectangle.HasValue)
            return true;

        var originalDestination = destinationRectangle;
        destinationRectangle = destinationRectangle.Clip(clippingRectangle.Value);

        if (destinationRectangle == Rectangle.Empty)
            return false; // Clipped rectangle is empty, nothing to draw

        int leftDiff = destinationRectangle.Left - originalDestination.Left;
        int topDiff = destinationRectangle.Top - originalDestination.Top;
        int bottomDiff = originalDestination.Bottom - destinationRectangle.Bottom;

        if (rotatedSource)
        {
            var scaleX = (float)sourceRectangle.Height / originalDestination.Width;
            var scaleY = (float)sourceRectangle.Width / originalDestination.Height;

            sourceRectangle.X += (int)(bottomDiff * scaleY);
            sourceRectangle.Y += (int)(leftDiff * scaleX);
            sourceRectangle.Width = (int)(destinationRectangle.Height * scaleY);
            sourceRectangle.Height = (int)(destinationRectangle.Width * scaleX);
        }
        else
        {
            var scaleX = (float)sourceRectangle.Width / originalDestination.Width;
            var scaleY = (float)sourceRectangle.Height / originalDestination.Height;

            sourceRectangle.X += (int)(leftDiff * scaleX);
            sourceRectangle.Y += (int)(topDiff * scaleY);
            sourceRectangle.Width = (int)(destinationRectangle.Width * scaleX);
            sourceRectangle.Height = (int)(destinationRectangle.Height * scaleY);
        }

        return true;
    }

    private static Texture2DRegion ClipSourceRegion(Texture2DRegion sourceRegion, Rectangle destinationRectangle, Rectangle clippingRectangle)
    {
        var left = (float)(clippingRectangle.Left - destinationRectangle.Left);
        var right = (float)(destinationRectangle.Right - clippingRectangle.Right);
        var top = (float)(clippingRectangle.Top - destinationRectangle.Top);
        var bottom = (float)(destinationRectangle.Bottom - clippingRectangle.Bottom);
        var x = left > 0 ? left : 0;
        var y = top > 0 ? top : 0;
        var w = (right > 0 ? right : 0) + x;
        var h = (bottom > 0 ? bottom : 0) + y;

        var scaleX = (float)destinationRectangle.Width / sourceRegion.OriginalSize.Width;
        var scaleY = (float)destinationRectangle.Height / sourceRegion.OriginalSize.Height;
        x /= scaleX;
        y /= scaleY;
        w /= scaleX;
        h /= scaleY;

        return sourceRegion.GetSubregion((int)x, (int)y, (int)(sourceRegion.OriginalSize.Width - w), (int)(sourceRegion.OriginalSize.Height - h));
    }

    private static Rectangle ClipDestinationRectangle(Rectangle destinationRectangle, Rectangle clippingRectangle)
    {
        return destinationRectangle.Clip(clippingRectangle);
    }

    #endregion -------------------------Utilities-----------------------------

}
