using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Geometry
{
    public abstract class GraphicsGeometryBuilder<TVertexType, TIndexType>
        where TVertexType : struct, IVertexType
        where TIndexType : struct
    {
        public PrimitiveType PrimitiveType { get; }
        public int PrimitivesCount { get; }
        public TVertexType[] Vertices { get; }
        public TIndexType[] Indices { get; }

        protected GraphicsGeometryBuilder(PrimitiveType primitiveType, int verticesCount, int indicesCount)
        {
            PrimitiveType = primitiveType;
            PrimitivesCount = primitiveType.GetPrimitivesCount(indicesCount);

            if (verticesCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(verticesCount));
            }

            if (indicesCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(indicesCount));
            }

            Vertices = new TVertexType[verticesCount];
            Indices = new TIndexType[indicesCount];
        }
    }
}
