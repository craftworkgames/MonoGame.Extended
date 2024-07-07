using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    public class TiledMapAnimatedLayerModelBuilder : TiledMapLayerModelBuilder<TiledMapAnimatedLayerModel>
    {
        public TiledMapAnimatedLayerModelBuilder()
        {
            AnimatedTilesetTiles = new List<TiledMapTilesetAnimatedTile>();
            AnimatedTilesetFlipFlags = new List<TiledMapTileFlipFlags>();
        }

        public List<TiledMapTilesetAnimatedTile> AnimatedTilesetTiles { get; }
        public List<TiledMapTileFlipFlags> AnimatedTilesetFlipFlags { get; }

        protected override void ClearBuffers()
        {
            AnimatedTilesetTiles.Clear();
            AnimatedTilesetFlipFlags.Clear();
        }

        protected override TiledMapAnimatedLayerModel CreateModel(GraphicsDevice graphicsDevice, Texture2D texture)
        {
            return new TiledMapAnimatedLayerModel(graphicsDevice, texture, Vertices.ToArray(), Indices.ToArray(), AnimatedTilesetTiles.ToArray(), AnimatedTilesetFlipFlags.ToArray());
        }
    }
}
