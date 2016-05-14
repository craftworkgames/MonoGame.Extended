using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public class PrimitiveMesh<TVertexType>
        where TVertexType : struct, IVertexType
    {
        // ReSharper disable InconsistentNaming
        internal readonly TVertexType[] _vertices;
        internal readonly short[] _indices;
        // ReSharper restore InconsistentNaming

        public IReadOnlyCollection<TVertexType> Vertices
        {
            get { return _vertices; }
        }

        public IReadOnlyCollection<short> Indices
        {
            get { return _indices; }
        }

        public PrimitiveType PrimitiveType { get; }

        public PrimitiveMesh(PrimitiveType primitiveType, TVertexType[] vertices, short[] indices = null)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            PrimitiveType = primitiveType;
            _vertices = vertices;
            _indices = indices;
        }
    }
}
