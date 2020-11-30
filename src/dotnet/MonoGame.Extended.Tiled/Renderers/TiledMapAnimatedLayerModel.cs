using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    public sealed class TiledMapAnimatedLayerModel : TiledMapLayerModel
    {
        public TiledMapAnimatedLayerModel(GraphicsDevice graphicsDevice, Texture2D texture, VertexPositionTexture[] vertices, ushort[] indices, TiledMapTilesetAnimatedTile[] animatedTilesetTiles) 
            : base(graphicsDevice, texture, vertices, indices)
        {
            Vertices = vertices;
            AnimatedTilesetTiles = animatedTilesetTiles;
        }

        public VertexPositionTexture[] Vertices { get; }
        public TiledMapTilesetAnimatedTile[] AnimatedTilesetTiles { get; }

        protected override VertexBuffer CreateVertexBuffer(GraphicsDevice graphicsDevice, int vertexCount)
        {
            return new DynamicVertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
        }

        protected override IndexBuffer CreateIndexBuffer(GraphicsDevice graphicsDevice, int indexCount)
        {
            return new DynamicIndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indexCount, BufferUsage.WriteOnly); ;
        }
    }
}