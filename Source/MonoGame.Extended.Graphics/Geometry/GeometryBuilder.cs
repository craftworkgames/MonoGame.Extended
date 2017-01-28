using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Geometry
{
    public abstract class GeometryBuilder<TVertexType, TIndexType>
        where TVertexType : struct, IVertexType
        where TIndexType : struct
    {
        public PrimitiveType PrimitiveType { get; protected set; }
        public int VertexCount { get; protected set; }
        public int IndexCount { get; protected set; }
        public int PrimitivesCount { get; protected set; }

        public TVertexType[] Vertices { get; }
        public TIndexType[] Indices { get; }

        protected GeometryBuilder(int maximumVerticesCount, int maximumIndicesCount)
        {
            Vertices = new TVertexType[maximumVerticesCount];
            Indices = new TIndexType[maximumIndicesCount];
        }
    }
}
