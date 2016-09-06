using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     The exception that is thrown when enqueuing vertices or indices into a <see cref="GeometryBuffer{TVertexType, TIndexType}" />
    ///     results in an overflow.
    /// </summary>
    /// <typeparam name="TVertexType">The vertex type.</typeparam>
    /// <typeparam name="TIndexType">The vertex index type.</typeparam>
    /// <seealso cref="Exception" />
    public class GeometryBufferOverflowException<TVertexType, TIndexType> : Exception
        where TVertexType : struct, IVertexType where TIndexType : struct 
    {
        internal GeometryBufferOverflowException(GeometryBuffer<TVertexType, TIndexType> geometryBuffer, int verticesCountToAdd, int indicesCountToAdd)
            : base(message: $"The GeometryBuffer could not enqueue the requested {verticesCountToAdd} vertices or {indicesCountToAdd} indices since it is full full with {geometryBuffer.VertexCount} (out of {geometryBuffer.MaximumVerticesCount}) vertices and {geometryBuffer.IndexCount} (out of {geometryBuffer.MaximumIndicesCount}) indices. Consider increasing the maximum number of vertices or indices when instantiating the GeometryBuffer, flushing the GeometryBuffer if it's dynamic, or using multiple GeometryBuffers to accommodate the requested enqueue.")
        {
        }
    }
}
