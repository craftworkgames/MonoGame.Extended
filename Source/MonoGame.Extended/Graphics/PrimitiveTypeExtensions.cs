using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public static class PrimitiveTypeExtensions
    {
        internal static int GetPrimitivesCount(this PrimitiveType primitiveType, int verticesCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineStrip:
                    return verticesCount - 1;
                case PrimitiveType.LineList:
                    return verticesCount/2;
                case PrimitiveType.TriangleStrip:
                    return verticesCount - 2;
                case PrimitiveType.TriangleList:
                    return verticesCount/3;
                default:
                    throw new ArgumentException("Invalid primitive type.");
            }
        }

        internal static int GetVerticesCount(this PrimitiveType primitiveType, int primitivesCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineStrip:
                    return primitivesCount + 1;
                case PrimitiveType.LineList:
                    return primitivesCount*2;
                case PrimitiveType.TriangleStrip:
                    return primitivesCount + 2;
                case PrimitiveType.TriangleList:
                    return primitivesCount*3;
                default:
                    throw new ArgumentException("Invalid primitive type.");
            }
        }
    }
}