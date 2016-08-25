using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public static class PrimitiveTypeExtensions
    {
        /// <summary>
        /// Gets the primitive count for a specified number of vertices.
        /// </summary>
        /// <param name="primitiveType">Type of the primitive.</param>
        /// <param name="verticesCount">The number of vertices.</param>
        /// <returns>The primitive count.</returns>
        /// <exception cref="ArgumentException">Invalid primitive type.</exception>
        internal static int GetPrimitiveCount(this PrimitiveType primitiveType, int verticesCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineStrip:
                    return verticesCount - 1;
                case PrimitiveType.LineList:
                    return verticesCount / 2;
                case PrimitiveType.TriangleStrip:
                    return verticesCount - 2;
                case PrimitiveType.TriangleList:
                    return verticesCount / 3;
                default:
                    throw new ArgumentException("Invalid primitive type.");
            }
        }
    }
}
