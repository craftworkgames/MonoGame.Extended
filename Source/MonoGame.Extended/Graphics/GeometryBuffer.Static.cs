using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public class StaticGeometryBuffer<TVertexType> : GeometryBuffer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GeometryBuffer{TVertexType}" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="maximumVerticesCount">The maximum number of vertices.</param>
        /// <param name="maximumIndicesCount">The maximum number of indices.</param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumVerticesCount" /> is <code>0</code>, or, <paramref name="maximumVerticesCount" /> is
        ///     <code>0</code>.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         For best performance, use <see cref="DynamicGeometryBuffer{TVertexType}" /> for geometry which changes
        ///         frame-to-frame and <see cref="StaticGeometryBuffer{TVertexType}" /> for geoemtry which does not change
        ///         frame-to-frame, or changes infrequently between frames.
        ///     </para>
        ///     <para>
        ///         Memory will be allocated for the vertex and index array buffers in proportion to
        ///         <paramref name="maximumVerticesCount" /> and <paramref name="maximumIndicesCount" /> respectively.
        ///     </para>
        /// </remarks>
        public StaticGeometryBuffer(GraphicsDevice graphicsDevice, ushort maximumVerticesCount, ushort maximumIndicesCount)
            : base(graphicsDevice, GeometryBufferType.Static, maximumVerticesCount, maximumIndicesCount)
        {
        }
    }
}
