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

            _spriteBatch = new SpriteBatch(graphicsDevice);
            _layers = new List<TileLayer>();
            _tileSets = new List<TileSet>();
        }

        private readonly SpriteBatch _spriteBatch;
        private readonly List<TileLayer> _layers;
        private readonly List<TileSet> _tileSets;
 
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public TileSet CreateTileSet(Texture2D texture, int firstId, int tileWidth, int tileHeight, int spacing = 2, int margin = 2)
        {
            var tileSet = new TileSet(texture, firstId, tileWidth, tileHeight, spacing, margin);
            _tileSets.Add(tileSet);
            return tileSet;
        }

        public TileLayer CreateLayer(string name, int width, int height, int[] data)
        {
            var layer = new TileLayer(this, name, width, height, data);
            _layers.Add(layer);
            return layer;
        }

        public void Draw(Camera2D camera)
        {
            foreach (var layer in _layers)
            {
                _spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
                layer.Draw(_spriteBatch);
                _spriteBatch.End();
            }
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
