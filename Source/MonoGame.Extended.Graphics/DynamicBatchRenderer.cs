using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Geometry;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Enables groups of draw calls to be batched together by enqueueing the geometry in single vertex buffer and index buffer every frame.
    /// </summary>
    /// <typeparam name="TVertexType">The type of the vertex type.</typeparam>
    /// <typeparam name="TIndexType">The type of the index type.</typeparam>
    /// <typeparam name="TDrawCallData">The type of the draw call data.</typeparam>
    /// <seealso cref="BatchRenderer{TVertexType, TIndexType, TDrawCallData}" />
    public class DynamicBatchRenderer<TVertexType, TIndexType, TDrawCallData> : BatchRenderer<TVertexType, TIndexType, TDrawCallData> where TVertexType : struct, IVertexType where TIndexType : struct where TDrawCallData : struct, IDrawCallData
    {
        private readonly TIndexType[] _sortedIndices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicBatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="defaultEffect">The default effect.</param>
        /// <param name="geometryBuffer">The geometry data buffer.</param>
        /// <param name="maximumDrawCallsCount">
        ///     The maximum number of draw calls that can be deferred. The default value is <code>2024</code>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice"/>, <paramref name="defaultEffect"/>, or <paramref name="geometryBuffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumDrawCallsCount" /> is less than or equal
        ///     <code>0</code>.
        /// </exception>
        public DynamicBatchRenderer(GraphicsDevice graphicsDevice, Effect defaultEffect, GraphicsGeometryBuffer<TVertexType, TIndexType> geometryBuffer, int maximumDrawCallsCount = DefaultMaximumDrawCallsCount) 
            : base(graphicsDevice, defaultEffect, geometryBuffer, maximumDrawCallsCount)
        {
            _sortedIndices = new TIndexType[geometryBuffer.MaximumIndicesCount];
        }

        /// <summary>
        ///     Submits the draw calls to the <see cref="GraphicsDevice" />.
        /// </summary>
        protected override void SubmitDrawCalls()
        {
            int drawCallsCount, indicesCount;
            SortIndices(out drawCallsCount, out indicesCount);
            EnqueuedDrawCallsCount = drawCallsCount;

            GeometryBuffer.VertexBuffer.SetData(GeometryBuffer.Vertices, 0, GeometryBuffer.VerticesCount);
            GeometryBuffer.VerticesCount = 0;

            GeometryBuffer.IndexBuffer.SetData(_sortedIndices, 0, indicesCount);
            GeometryBuffer.IndicesCount = 0;

            base.SubmitDrawCalls();
        }

        private void SortIndices(out int drawCallsCount, out int sortedIndicesCount)
        {
            var currentDrawCall = DrawCalls[0];
            var previousDrawCall = DrawCalls[0];
            var newDrawCallsCount = 1;
            sortedIndicesCount = 0;

            // change the indices used by the draw call to use the sorted indices array so the indices are sequential for the graphics API
            DrawCalls[0].StartIndex = 0;
            DrawCalls[0].BaseVertex = 0;
            // get the number of vertices used for the current draw call, each index represents a vertex
            var drawCallIndicesCount = currentDrawCall.PrimitiveType.GetVerticesCount(currentDrawCall.PrimitiveCount);
            // copy the indices of the current draw call into our sorted indices array so the indices are sequential for the graphics API
            GeometryBuffer.CopyIndicesTo(_sortedIndices, currentDrawCall.StartIndex, 0, drawCallIndicesCount, currentDrawCall.BaseVertex);
            sortedIndicesCount += drawCallIndicesCount;

            // iterate through sorted draw calls checking if any can now be merged to reduce expensive draw calls to the graphics API
            // this might need to be changed for next-gen graphics API (Vulkan, Metal, DirectX 11) where the draw calls are not so expensive
            for (var index = 1; index < EnqueuedDrawCallsCount; index++)
            {
                currentDrawCall = DrawCalls[index];

                // get the number of vertices used for the draw call, each index represents a vertex
                drawCallIndicesCount = currentDrawCall.PrimitiveType.GetVerticesCount(currentDrawCall.PrimitiveCount);

                if (sortedIndicesCount + drawCallIndicesCount > _sortedIndices.Length)
                {
                    base.SubmitDrawCalls();
                    sortedIndicesCount = 0;
                }

                if (TryMergeDrawCall(ref sortedIndicesCount, currentDrawCall, ref previousDrawCall, newDrawCallsCount,
                    drawCallIndicesCount))
                    continue;

                // could not merge draw call

                // change the indices used by the current draw call to use the sorted indices array so the indices are sequential for the graphics API
                currentDrawCall.StartIndex = sortedIndicesCount;

                DrawCalls[newDrawCallsCount++] = currentDrawCall;
                previousDrawCall = currentDrawCall;

                // copy the indices of the command into our sorted indices array so the merged indices are sequential for the graphics API
                GeometryBuffer.CopyIndicesTo(_sortedIndices, currentDrawCall.StartIndex, sortedIndicesCount, drawCallIndicesCount, currentDrawCall.BaseVertex);
                sortedIndicesCount += drawCallIndicesCount;
            }

            drawCallsCount = newDrawCallsCount;
        }

        private bool TryMergeDrawCall(ref int sortedIndicesCount, DrawCall<TDrawCallData> currentDrawCall, ref DrawCall<TDrawCallData> previousDrawCall, int newDrawCallsCount, int drawCallIndicesCount)
        {
            if (currentDrawCall.Key != previousDrawCall.Key ||
                currentDrawCall.PrimitiveType != previousDrawCall.PrimitiveType)
                return false;

            if (currentDrawCall.PrimitiveType == PrimitiveType.TriangleStrip || currentDrawCall.PrimitiveType == PrimitiveType.LineStrip)
            {
                return false;
            }

            // copy the indices of the draw call into our sorted indices array so the merged indices are sequential for the graphics API
            GeometryBuffer.CopyIndicesTo(_sortedIndices, currentDrawCall.StartIndex, sortedIndicesCount, drawCallIndicesCount, currentDrawCall.BaseVertex);
            sortedIndicesCount += drawCallIndicesCount;

            // increase the number of primitives for the previous draw call by the amount of the current draw call
            DrawCalls[newDrawCallsCount - 1].PrimitiveCount =
                previousDrawCall.PrimitiveCount += currentDrawCall.PrimitiveCount;

            return true;
        }
    }
}
