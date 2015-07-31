using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.TileMaps
{
    public class TileMap
    {
        public TileMap(GraphicsDevice graphicsDevice, int width, int height, int tileWidth, int tileHeight)
        {
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            _graphicsDevice = graphicsDevice;
            _layers = new List<TileLayer>();
            _tileSets = new List<TileSet>();
        }

        private readonly List<TileLayer> _layers;
        private readonly List<TileSet> _tileSets;
        private readonly GraphicsDevice _graphicsDevice;
 
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public int WidthInPixels
        {
            get { return Width * TileWidth; }
        }

        public int HeightInPixels
        {
            get { return Height * TileHeight; }
        }

        public TileSet CreateTileSet(Texture2D texture, int firstId, int tileWidth, int tileHeight, int spacing = 2, int margin = 2)
        {
            var tileSet = new TileSet(texture, firstId, tileWidth, tileHeight, spacing, margin);
            _tileSets.Add(tileSet);
            return tileSet;
        }

        public TileLayer CreateLayer(string name, int width, int height, int[] data)
        {
            var layer = new TileLayer(this, _graphicsDevice, name, width, height, data);
            _layers.Add(layer);
            return layer;
        }

        public void Draw(Camera2D camera)
        {
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
