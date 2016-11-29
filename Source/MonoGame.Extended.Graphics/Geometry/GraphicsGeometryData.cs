using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Geometry
{
    public class GraphicsGeometryData<TVertexType, TIndexType>
        where TVertexType : struct, IVertexType where TIndexType : struct
    {
        public TVertexType[] Vertices { get; }
        public int VerticesCount { get; set; }
        public int MaximumVerticesCount { get; }
        public TIndexType[] Indices { get; }
        public int IndicesCount { get; set; }
        public int MaximumIndicesCount { get; }

        public GraphicsGeometryData(int maximumVertices)
        {
            if (maximumVertices < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumVertices));
            }

            Vertices = new TVertexType[maximumVertices];
            Indices = null;
            MaximumVerticesCount = maximumVertices;
            MaximumIndicesCount = 0;
        }

        public GraphicsGeometryData(int maximumVertices, int maximumIndices)
        {
            if (maximumVertices < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumVertices));
            }

            if (maximumIndices < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumIndices));
            }

            Vertices = new TVertexType[maximumVertices];
            Indices = new TIndexType[maximumIndices];
            MaximumVerticesCount = maximumVertices;
            MaximumIndicesCount = maximumIndices;
        }
    }
}
