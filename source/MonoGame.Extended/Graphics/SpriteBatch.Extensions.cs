// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics;

public static class SpriteBatchExtensions
{
    #region ----------------------------NinePatch-----------------------------
    private static readonly Rectangle[] _patchCache = new Rectangle[9];

    public static void DrawNinePatch(this SpriteBatch spriteBatch, NinePatch ninePatchRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
    {
        var destinationPatches = ninePatchRegion.CreatePatches(destinationRectangle);
        var sourcePatches = ninePatchRegion.SourcePatches;

        for (var i = 0; i < sourcePatches.Length; i++)
        {
            var sourcePatch = sourcePatches[i];
            var destinationPatch = destinationPatches[i];

            if (clippingRectangle.HasValue)
            {
                sourcePatch = ClipSourceRectangle(sourcePatch, destinationPatch, clippingRectangle.Value);
                destinationPatch = ClipDestinationRectangle(destinationPatch, clippingRectangle.Value);
                Draw(spriteBatch, ninePatchRegion.Texture, sourcePatch, destinationPatch, color, clippingRectangle);
            }
            else
            {
                if (destinationPatch.Width > 0 && destinationPatch.Height > 0)
                    spriteBatch.Draw(ninePatchRegion.Texture, sourceRectangle: sourcePatch, destinationRectangle: destinationPatch, color: color);
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

    public static void DrawTextureRegion(this SpriteBatch spriteBatch, TextureRegion textureRegion, Vector2 position, Color color, Rectangle? clippingRectangle = null)
    {
        DrawTextureRegion(spriteBatch, textureRegion, position, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0, clippingRectangle);
    }

    public static void DrawTextureRegion(this SpriteBatch spriteBatch, TextureRegion textureRegion, Vector2 position, Color color,
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

    public static void DrawTextureRegion(this SpriteBatch spriteBatch, TextureRegion textureRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
    {
        Draw(spriteBatch, textureRegion.Texture, textureRegion.Bounds, destinationRectangle, color, clippingRectangle);
    }



    #endregion -------------------------TextureRegion-----------------------------

    #region ----------------------------Utilities-----------------------------
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

    #endregion -------------------------Utilities-----------------------------

}
