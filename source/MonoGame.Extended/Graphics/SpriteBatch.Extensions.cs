// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.


using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics;

public static class SpriteBatchExtensions
{
    #region ----------------------------NinePatch-----------------------------
    private static readonly Rectangle[] _patchCache = new Rectangle[9];
    private static Rectangle _rect = default;

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

    #region ----------------------------Texture2D-----------------------------

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

    public static void Draw(this SpriteBatch spriteBatch, Texture2DRegion textureRegion, Vector2 position, Color color, Rectangle? clippingRectangle = null)
    {
        Draw(spriteBatch, textureRegion, position, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0, clippingRectangle);
    }

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
