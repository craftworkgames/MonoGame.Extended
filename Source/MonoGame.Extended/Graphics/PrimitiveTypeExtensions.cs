using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public static class PrimitiveTypeExtensions
    {
        internal static int GetPrimitiveCount(this PrimitiveType primitiveType, int verticesOrIndicesCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineStrip:
                    return verticesOrIndicesCount - 1;
                case PrimitiveType.LineList:
                    return verticesOrIndicesCount / 2;
                case PrimitiveType.TriangleStrip:
                    return verticesOrIndicesCount - 2;
                case PrimitiveType.TriangleList:
                    return verticesOrIndicesCount / 3;
                default:
                    throw new ArgumentException("Invalid primitive type.");
            }
        }
    }
}
