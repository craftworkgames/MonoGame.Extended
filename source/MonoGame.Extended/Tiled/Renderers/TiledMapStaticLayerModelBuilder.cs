using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    public class TiledMapStaticLayerModelBuilder : TiledMapLayerModelBuilder<TiledMapStaticLayerModel>
    {
        protected override void ClearBuffers()
        {
        }

        protected override TiledMapStaticLayerModel CreateModel(GraphicsDevice graphicsDevice, Texture2D texture)
        {
            return new TiledMapStaticLayerModel(graphicsDevice, texture, Vertices.ToArray(), Indices.ToArray());
        }
    }
}