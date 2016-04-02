using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public interface IBatchQueuer<in TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        void Begin();
        void End();
        void Queue(PrimitiveType type, TVertexType[] vertices, int startVertex, int vertexCount, IDrawContext drawContext);
        void Queue(PrimitiveType type, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, IDrawContext drawContext);
    }
}
