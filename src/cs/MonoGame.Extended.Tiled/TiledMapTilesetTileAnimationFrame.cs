using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public struct TiledMapTilesetTileAnimationFrame
    {
        public readonly int LocalTileIdentifier;
        public readonly TimeSpan Duration;
        public readonly Vector2[] TextureCoordinates;
        private readonly Dictionary<TiledMapTileFlipFlags, Vector2[]> _flipDictionary = new Dictionary<TiledMapTileFlipFlags, Vector2[]>();

        internal TiledMapTilesetTileAnimationFrame(TiledMapTileset tileset, int localTileIdentifier, int durationInMilliseconds)
        {
            LocalTileIdentifier = localTileIdentifier;
            Duration = new TimeSpan(0, 0, 0, 0, durationInMilliseconds);
            TextureCoordinates = new Vector2[4];
            CreateTextureCoordinates(tileset);
        }

        public Vector2[] GetTextureCoordinates(TiledMapTileFlipFlags flipFlags)
        {
            if (!_flipDictionary.TryGetValue(flipFlags, out Vector2[] flippedTextureCoordiantes))
            {
                return TextureCoordinates;
            }
            else
            {
                return flippedTextureCoordiantes;
            }
        }

        public void CreateTextureRotations(TiledMapTileset tileset, TiledMapTileFlipFlags flipFlags)
        {
            if (!_flipDictionary.ContainsKey(flipFlags))
            {
                if (flipFlags == TiledMapTileFlipFlags.None)
                {
                    _flipDictionary.Add(flipFlags, TextureCoordinates);
                }
                else
                {
                    _flipDictionary.Add(flipFlags, TransformTextureCoordinates(tileset, flipFlags));
                }
            }
        }

        public Vector2[] TransformTextureCoordinates(TiledMapTileset tileset, TiledMapTileFlipFlags flipFlags)
        {
            var sourceRectangle = tileset.GetTileRegion(LocalTileIdentifier);
            var texture = tileset.Texture;
            var texelLeft = (float)sourceRectangle.X / texture.Width;
            var texelTop = (float)sourceRectangle.Y / texture.Height;
            var texelRight = (sourceRectangle.X + sourceRectangle.Width) / (float)texture.Width;
            var texelBottom = (sourceRectangle.Y + sourceRectangle.Height) / (float)texture.Height;

            var flipDiagonally = (flipFlags & TiledMapTileFlipFlags.FlipDiagonally) != 0;
            var flipHorizontally = (flipFlags & TiledMapTileFlipFlags.FlipHorizontally) != 0;
            var flipVertically = (flipFlags & TiledMapTileFlipFlags.FlipVertically) != 0;
            var transform = new Vector2[4];

            transform[0].X = texelLeft;
            transform[0].Y = texelTop;

            transform[1].X = texelRight;
            transform[1].Y = texelTop;

            transform[2].X = texelLeft;
            transform[2].Y = texelBottom;

            transform[3].X = texelRight;
            transform[3].Y = texelBottom;

            if (flipDiagonally)
            {
                FloatHelper.Swap(ref transform[1].X, ref transform[2].X);
                FloatHelper.Swap(ref transform[1].Y, ref transform[2].Y);
            }

            if (flipHorizontally)
            {
                if (flipDiagonally)
                {
                    FloatHelper.Swap(ref transform[0].Y, ref transform[1].Y);
                    FloatHelper.Swap(ref transform[2].Y, ref transform[3].Y);
                }
                else
                {
                    FloatHelper.Swap(ref transform[0].X, ref transform[1].X);
                    FloatHelper.Swap(ref transform[2].X, ref transform[3].X);
                }
            }

            if (flipVertically)
            {
                if (flipDiagonally)
                {
                    FloatHelper.Swap(ref transform[0].X, ref transform[2].X);
                    FloatHelper.Swap(ref transform[1].X, ref transform[3].X);
                }
                else
                {
                    FloatHelper.Swap(ref transform[0].Y, ref transform[2].Y);
                    FloatHelper.Swap(ref transform[1].Y, ref transform[3].Y);
                }
            }

            transform[0] = transform[0];
            transform[1] = transform[1];
            transform[2] = transform[2];
            transform[3] = transform[3];

            return transform;
        }

        private void CreateTextureCoordinates(TiledMapTileset tileset)
        {
            var sourceRectangle = tileset.GetTileRegion(LocalTileIdentifier);
            var texture = tileset.Texture;
            var texelLeft = (float)sourceRectangle.X / texture.Width;
            var texelTop = (float)sourceRectangle.Y / texture.Height;
            var texelRight = (sourceRectangle.X + sourceRectangle.Width) / (float)texture.Width;
            var texelBottom = (sourceRectangle.Y + sourceRectangle.Height) / (float)texture.Height;

            TextureCoordinates[0].X = texelLeft;
            TextureCoordinates[0].Y = texelTop;

            TextureCoordinates[1].X = texelRight;
            TextureCoordinates[1].Y = texelTop;

            TextureCoordinates[2].X = texelLeft;
            TextureCoordinates[2].Y = texelBottom;

            TextureCoordinates[3].X = texelRight;
            TextureCoordinates[3].Y = texelBottom;
        }

        public override string ToString()
        {
            return $"{LocalTileIdentifier}:{Duration}";
        }
    }
}
