using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public interface IBatcher<in TVertexType>
        where TVertexType : struct, IVertexType
    {
        int MaximumBatchSize { get; }

        void Select(TVertexType[] vertices, int vertexCount);
        void Select(TVertexType[] vertices, int vertexCount, short[] indices, int indexCount);
        void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, IDrawContext context);
        void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext context);
    }
}
