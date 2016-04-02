using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public interface IBatcher<in TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        int MaximumBatchSize { get; }

        void Select(TVertexType[] vertices);
        void Select(TVertexType[] vertices, short[] indices);
        void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, IDrawContext drawContext);
        void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext);
    }
}
