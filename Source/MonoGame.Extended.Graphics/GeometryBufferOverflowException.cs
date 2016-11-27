using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     The exception that is thrown when enqueuing vertices or indices into a <see cref="GeometryBuffer{TVertexType}" />
    ///     results in an overflow.
    /// </summary>
    /// <typeparam name="TVertexType">The vertex type.</typeparam>
    /// <seealso cref="Exception" />
    public class GeometryBufferOverflowException<TVertexType> : Exception
        where TVertexType : struct, IVertexType
    {
        internal GeometryBufferOverflowException(GeometryBuffer<TVertexType> geometryBuffer, int verticesCountToAdd,
            int indicesCountToAdd)
            : base(
                $"The GeometryBuffer could not enqueue the requested {verticesCountToAdd} vertices or {indicesCountToAdd} indices since it is full full with {geometryBuffer._vertexCount} (out of {geometryBuffer.MaximumVerticesCount}) vertices and {geometryBuffer._indexCount} (out of {geometryBuffer.MaximumIndicesCount}) indices. Consider increasing the maximum number of vertices or indices when instantiating the GeometryBuffer, flushing the GeometryBuffer if it's dynamic, or using multiple GeometryBuffers to accommodate the requested enqueue."
            )
        {
        }
    }
}