using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Tiled
{
    public class TiledMap
    {
        public TiledMap(GraphicsDevice graphicsDevice, int width, int height, int tileWidth, int tileHeight)
        {
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            _graphicsDevice = graphicsDevice;
            _layers = new List<TiledLayer>();
            _tileSets = new List<TiledTileSet>();
        }

        private readonly List<TiledLayer> _layers;
        private readonly List<TiledTileSet> _tileSets;
        private readonly GraphicsDevice _graphicsDevice;
 
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public Color? BackgroundColor { get; set; }

        public int WidthInPixels
        {
            get { return Width * TileWidth - Width; }       // annoyingly we have to compensate 1 pixel per tile, seems to be a bug in MonoGame?
        }

        public int HeightInPixels
        {
            get { return Height * TileHeight - Height; }
        }

        public TiledTileSet CreateTileSet(Texture2D texture, int firstId, int tileWidth, int tileHeight, int spacing = 2, int margin = 2)
        {
            var tileSet = new TiledTileSet(texture, firstId, tileWidth, tileHeight, spacing, margin);
            _tileSets.Add(tileSet);
            return tileSet;
        }

        public TiledLayer CreateLayer(string name, int width, int height, int[] data)
        {
            var layer = new TiledLayer(this, _graphicsDevice, name, width, height, data);
            _layers.Add(layer);
            return layer;
        }

        public void Draw(Camera2D camera, bool useMapBackgroundColor = false)
        {
            if(useMapBackgroundColor && BackgroundColor.HasValue)
                _graphicsDevice.Clear(BackgroundColor.Value);

            foreach (var layer in _layers)
                layer.Draw(camera);
        }

        public TextureRegion2D GetTileRegion(int id)
        {
            if (id == 0)
                return null;

            var tileset = _tileSets.LastOrDefault(i => i.FirstId <= id);

            if (tileset == null)
                throw new InvalidOperationException(string.Format("No TileSet found for id {0}", id));

            return tileset.GetTileRegion(id);
        }
    }
}
