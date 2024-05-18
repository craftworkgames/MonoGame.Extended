using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Extended.Tiled.Renderers
{
    public sealed class TiledMapAnimatedLayerModel : TiledMapLayerModel
    {
        public TiledMapAnimatedLayerModel(GraphicsDevice graphicsDevice, Texture2D texture, VertexPositionTexture[] vertices, ushort[] indices, TiledMapTilesetAnimatedTile[] animatedTilesetTiles, TiledMapTileFlipFlags[] animatedTilesetTileFlipFlags)
            : base(graphicsDevice, texture, vertices, indices)
        {
            Vertices = vertices;
            AnimatedTilesetTiles = animatedTilesetTiles;
            _animatedTilesetFlipFlags = animatedTilesetTileFlipFlags;
        }

        public VertexPositionTexture[] Vertices { get; }
        public TiledMapTilesetAnimatedTile[] AnimatedTilesetTiles { get; }
        private readonly TiledMapTileFlipFlags[] _animatedTilesetFlipFlags;

        public ReadOnlySpan<TiledMapTileFlipFlags> AnimatedTilesetFlipFlags => _animatedTilesetFlipFlags;

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
