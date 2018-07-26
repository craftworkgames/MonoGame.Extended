using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace JamGame
{
    public class Tileset
    {
        private readonly int _columns;
        private readonly TextureAtlas _atlas;

        public Tileset(Texture2D texture, int tileWidth, int tileHeight)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            _atlas = TextureAtlas.Create("", texture, tileWidth, tileHeight);
            _columns = texture.Width / tileWidth;
        }

        public int TileWidth { get; }
        public int TileHeight { get; }

        public TextureRegion2D GetTile(int index)
        {
            if (index < 0 || index >= _atlas.RegionCount)
                return null;

            return _atlas[index];
        }
        
        public TextureRegion2D GetTileAt(int x, int y)
        {
            if (x >= _columns)
                return null;

            var index = y * _columns + x;

            return GetTile(index);
        }
    }
}