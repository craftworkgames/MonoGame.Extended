using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Geometry;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Enables groups of draw calls to be batched together by having the geometry in single vertex buffer and index buffer.
    /// </summary>
    /// <typeparam name="TVertexType">The type of vertex.</typeparam>
    /// <typeparam name="TIndexType">The type of vertex index.</typeparam>
    /// <typeparam name="TDrawCallData">The type of the draw call data.</typeparam>
    /// <seealso cref="IDisposable" />
    public abstract class BatchRenderer<TVertexType, TIndexType, TDrawCallData> : QueueRenderer<TDrawCallData>
        where TVertexType : struct, IVertexType
        where TIndexType : struct
        where TDrawCallData : struct, IDrawCallData
    {
        /// <summary>
        ///     Gets the <see cref="GeometryBuffer" /> associated with this
        ///     <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GeometryBuffer" /> associated with this
        ///     <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" />.
        /// </value>
        public GraphicsGeometryBuffer<TVertexType, TIndexType> GeometryBuffer { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="defaultEffect">The default effect.</param>
        /// <param name="geometryBuffer">The geometry data buffer.</param>
        /// <param name="maximumDrawCallsCount">
        ///     The maximum number of draw calls that can be deferred. The default value is <code>2024</code>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice"/>, or <paramref name="defaultEffect"/>, or <paramref name="geometryBuffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumDrawCallsCount" /> is less than or equal
        ///     <code>0</code>.
        /// </exception>
        protected BatchRenderer(GraphicsDevice graphicsDevice, Effect defaultEffect, GraphicsGeometryBuffer<TVertexType, TIndexType> geometryBuffer,
            int maximumDrawCallsCount = DefaultMaximumDrawCallsCount)
            : base(graphicsDevice, defaultEffect, maximumDrawCallsCount)
        {
            if (geometryBuffer == null)
                throw new ArgumentNullException(nameof(geometryBuffer));

            GeometryBuffer = geometryBuffer;
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="diposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool diposing)
        {
            if (diposing)
                GeometryBuffer.Dispose();
        }

        /// <summary>
        ///     Submits the draw calls to the <see cref="GraphicsDevice" />.
        /// </summary>
        protected override void SubmitDrawCalls()
        {
            GraphicsDevice.SetVertexBuffer(GeometryBuffer.VertexBuffer);
            GraphicsDevice.Indices = GeometryBuffer.IndexBuffer;

            base.SubmitDrawCalls();
        }
    }
}