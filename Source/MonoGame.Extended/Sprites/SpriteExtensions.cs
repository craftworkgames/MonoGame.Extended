﻿using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Sprites
{
    public static class SpriteExtensions
    {
        public static void Draw(this Sprite sprite, SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, sprite);
        }

        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite)
        {
            if (sprite == null) throw new ArgumentNullException(nameof(sprite));

            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;

                spriteBatch.Draw(texture, sprite.WorldPosition, sourceRectangle, sprite.Color*sprite.Alpha, sprite.WorldRotation,
                    sprite.Origin,
                    sprite.WorldScale, sprite.Effect, sprite.Depth);
            }
        }
    }
}