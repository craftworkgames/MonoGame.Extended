using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    public sealed class TiledMapStaticLayerModel : TiledMapLayerModel
    {
        public TiledMapStaticLayerModel(GraphicsDevice graphicsDevice, Texture2D texture, VertexPositionTexture[] vertices, ushort[] indices)
            : base(graphicsDevice, texture, vertices, indices)
        {
        }

        protected override VertexBuffer CreateVertexBuffer(GraphicsDevice graphicsDevice, int vertexCount)
        {
            return new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
        }

        protected override IndexBuffer CreateIndexBuffer(GraphicsDevice graphicsDevice, int indexCount)
        {
            return new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indexCount, BufferUsage.WriteOnly); ;
        }
    }
}