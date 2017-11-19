using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    public class TiledMapAnimatedLayerModelBuilder : TiledMapLayerModelBuilder<TiledMapAnimatedLayerModel>
    {
        public TiledMapAnimatedLayerModelBuilder()
        {
            AnimatedTilesetTiles = new List<TiledMapTilesetAnimatedTile>();
        }

        public List<TiledMapTilesetAnimatedTile> AnimatedTilesetTiles { get; }

        protected override void ClearBuffers()
        {
            AnimatedTilesetTiles.Clear();
        }

        protected override TiledMapAnimatedLayerModel CreateModel(GraphicsDevice graphicsDevice, Texture2D texture)
        {
            return new TiledMapAnimatedLayerModel(graphicsDevice, texture, Vertices.ToArray(), Indices.ToArray(), AnimatedTilesetTiles.ToArray());
        }
    }
}