using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public struct TiledMapTilesetTileAnimationFrame
    {
        public readonly int LocalTileIdentifier;
        public readonly TimeSpan Duration;
        public readonly Vector2[] TextureCoordinates;

        internal TiledMapTilesetTileAnimationFrame(TiledMapTileset tileset, int localTileIdentifier, int durationInMilliseconds)
        {
            LocalTileIdentifier = localTileIdentifier;
            Duration = new TimeSpan(0, 0, 0, 0, durationInMilliseconds);
            TextureCoordinates = new Vector2[4];
            CreateTextureCoordinates(tileset);
        }

        private void CreateTextureCoordinates(TiledMapTileset tileset)
        {
            var sourceRectangle = tileset.GetTileRegion(LocalTileIdentifier);
            var texture = tileset.Texture;
            var texelLeft = (sourceRectangle.X + 0.5f) / texture.Width;
            var texelTop = (sourceRectangle.Y + 0.5f) / texture.Height;
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