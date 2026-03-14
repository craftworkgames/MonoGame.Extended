using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    [Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
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