using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Enables a group of dynamic geometry to be batched together using the same settings to be sent to the graphics
    ///     processing unit (GPU) for rendering.
    /// </summary>
    /// <typeparam name="TVertexType">The type of the primitives to be batched.</typeparam>
    /// <typeparam name="TBatchItemData">The type of user data associated with each draw call.</typeparam>
    /// <remarks>
    ///     <para>
    ///         <see cref="PrimitiveBatch{TVertexType, TBatchItemData}" /> is a helper for easily and efficiently drawing
    ///         dynamically generated geometry such as lines and triangles which change frame-to-frame. Dynamic submission is a
    ///         highly effective pattern for drawing procedural geometry and convenient for debug rendering. It is however not
    ///         as efficient at drawing geometry which does not change every frame. Such geometry should use a static
    ///         <see cref="VertexBuffer" /> and possibly a <see cref="IndexBuffer" /> for rendering instead of
    ///         <see cref="PrimitiveBatch{TVertexType, TBatchItemData}" />.
    ///     </para>
    /// </remarks>
    public class PrimitiveBatch<TVertexType, TBatchItemData> : IDisposable
        where TVertexType : struct, IVertexType where TBatchItemData : struct, IBatchItemData<TBatchItemData>
    {
        public const ushort DefaultMaximumVerticesCount = 8192;
        public const ushort DefaultMaximumIndicesCount = 12288;

        private BatchDrawer<TVertexType, TBatchItemData> _batchDrawer;
        private BatchQueuer<TVertexType, TBatchItemData> _currentBatchQueuer;
        private ImmediateBatchQueuer<TVertexType, TBatchItemData> _immediateBatchQueuer;
        private DeferredBatchQueuer<TVertexType, TBatchItemData> _deferredBatchQueuer;
        private static readonly TVertexType[] _verticesArrayBuffer;

        /// <summary>
        ///     Gets the <see cref="GraphicsDevice" /> used by this <see cref="PrimitiveBatch{TVertexType, TBatchItemData}" />.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        ///     Gets a value indicating whether batching is currently in progress by being within a <see cref="Begin" />-
        ///     <see cref="End" /> pair block.
        /// </summary>
        public bool HasBegun { get; private set; }

        /// <summary>
        ///     Gets the maximum number of vertices that can be buffered into a single batch before the
        ///     geometry needs to be flushed to the graphics processing unit (GPU).
        /// </summary>
        public ushort MaximumVerticesCount { get; }

        /// <summary>
        ///     Gets the maximum number of indices that can be buffered into a single batch before the
        ///     geometry needs to be flushed to the graphics processing unit (GPU).
        /// </summary>
        public ushort MaximumIndicesCount { get; }

        static PrimitiveBatch()
        {
            _verticesArrayBuffer = new TVertexType[4];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PrimitiveBatch{TVertexType, TBatchItemData}" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="maximumVerticesCount">
        ///     The maximum number of vertices that can be buffered into a single batch before the
        ///     geometry needs to be flushed to the graphics processing unit (GPU). The default is 8192.
        /// </param>
        /// <param name="maximumIndicesCount">
        ///     The maximum number of indices that can be buffered into a single batch before the
        ///     geometry needs to be flushed to the graphics processing unit (GPU). The default is 12288.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <list type="bullet">
        ///         <listheader>
        ///             <term>One or more of the following is true:</term>
        ///         </listheader>
        ///         <item>
        ///             <description><paramref name="maximumVerticesCount" /> is 0.</description>
        ///         </item>
        ///         <item>
        ///             <description><paramref name="maximumIndicesCount" /> is 0.</description>
        ///         </item>
        ///     </list>
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         Memory will be allocated for the vertex and index buffers in proportion to
        ///         <paramref name="maximumVerticesCount" /> and <paramref name="maximumIndicesCount" /> respectively.
        ///     </para>
        /// </remarks>
        public PrimitiveBatch(GraphicsDevice graphicsDevice, ushort maximumVerticesCount = DefaultMaximumVerticesCount, ushort maximumIndicesCount = DefaultMaximumIndicesCount)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }

            if (maximumVerticesCount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumVerticesCount));
            }

            if (maximumIndicesCount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumIndicesCount));
            }

            GraphicsDevice = graphicsDevice;
            MaximumVerticesCount = maximumVerticesCount;
            MaximumIndicesCount = maximumIndicesCount;

            _batchDrawer = new BatchDrawer<TVertexType, TBatchItemData>(graphicsDevice, maximumVerticesCount, maximumIndicesCount);
        }

        /// <summary>
        ///     Releases used unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Immediately releases the used unmanaged resources.
        /// </summary>
        /// <param name="diposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool diposing)
        {
            if (!diposing)
            {
                return;
            }

            _batchDrawer.Dispose();
            _batchDrawer = null;
            _currentBatchQueuer = null;
            _immediateBatchQueuer.Dispose();
            _immediateBatchQueuer = null;
            _deferredBatchQueuer.Dispose();
            _deferredBatchQueuer = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureHasBegun([CallerMemberName] string callerMemberName = null)
        {
            if (!HasBegun)
            {
                throw new InvalidOperationException($"The {nameof(Begin)} method must be called before the {callerMemberName} method can be called.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureHasNotBegun([CallerMemberName] string callerMemberName = null)
        {
            if (HasBegun)
            {
                throw new InvalidOperationException($"The {nameof(End)} method must be called before the {callerMemberName} method can be called.");
            }
        }

        /// <summary>
        ///     Begins a new batch of geometry using the specified mode.
        /// </summary>
        /// <param name="effect">The <see cref="Effect" /> to use.</param>
        /// <param name="mode">The <see cref="BatchMode" />.</param>
        /// <remarks>
        ///     <para>
        ///         When the batch becomes full, the geometry is sent to the graphics processing unit (GPU) and a new batch will
        ///         begin automatically.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="mode" /> is not one of <see cref="BatchMode" />'s
        ///     discrete values.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Begin" /> has been called before calling <see cref="End" />
        ///     after the last call to <see cref="Begin" />. <see cref="Begin" /> cannot be called again until <see cref="End" />
        ///     has been successfully called.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         This method must be called before any draw calls. When all the geometry have been drawn, call
        ///         <see cref="End" />.
        ///     </para>
        /// </remarks>
        public void Begin(Effect effect, BatchMode mode = BatchMode.Deferred)
        {
            if (effect == null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            EnsureHasNotBegun();

            HasBegun = true;
            switch (mode)
            {
                case BatchMode.Immediate:
                    if (_immediateBatchQueuer == null)
                    {
                        _immediateBatchQueuer = new ImmediateBatchQueuer<TVertexType, TBatchItemData>(_batchDrawer);
                    }
                    _currentBatchQueuer = _immediateBatchQueuer;
                    break;
                case BatchMode.Deferred:
                    if (_deferredBatchQueuer == null)
                    {
                        _deferredBatchQueuer = new DeferredBatchQueuer<TVertexType, TBatchItemData>(_batchDrawer);
                    }
                    _currentBatchQueuer = _deferredBatchQueuer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }

            _currentBatchQueuer.Begin(effect);
        }

        /// <summary>
        ///     Ends the current batch by sending any geometry in the current batch to the graphics processing unit (GPU).
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="End" /> was called, but <see cref="Begin" /> has not yet been
        ///     called. You must call <see cref="Begin" /> successfully before you can call <see cref="End" />.
        /// </exception>
        public void End()
        {
            EnsureHasBegun();
            HasBegun = false;
            _currentBatchQueuer.End();
        }

        /// <summary>
        ///     Adds geometry to a batch of geometry for rendering.
        /// </summary>
        /// <param name="primitiveType">The <see cref="PrimitiveType" />.</param>
        /// <param name="vertices">The vertices which explictly define the geometry to render.</param>
        /// <param name="data">The user data associated with the geometry.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the geometry when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     This method was called before <see cref="Begin" />. <see cref="Begin" /> must be called before you can call
        ///     <see cref="Draw(PrimitiveType,TVertexType[],ref TBatchItemData,uint)" />.
        /// </exception>
        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, ref TBatchItemData data, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(ref data, primitiveType, vertices, 0, vertices.Length, null, 0, 0, sortKey);
        }

        /// <summary>
        ///     Adds geometry to a batch of geometry for rendering.
        /// </summary>
        /// <param name="primitiveType">The <see cref="PrimitiveType" />.</param>
        /// <param name="vertices">The vertices which explictly define the geometry to render.</param>
        /// <param name="indices">The indices of the <paramref name="vertices" /> to render.</param>
        /// <param name="data">The user data associated with the geometry.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the geometry when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     This method was called before <see cref="Begin" />. <see cref="Begin" /> must be called before you can call
        ///     <see cref="Draw(PrimitiveType,TVertexType[],int[],ref TBatchItemData,uint)" />.
        /// </exception>
        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int[] indices, ref TBatchItemData data, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(ref data, primitiveType, vertices, 0, vertices.Length, indices, 0, indices.Length, sortKey);
        }

        /// <summary>
        ///     Adds geometry to a batch of geometry for rendering.
        /// </summary>
        /// <param name="primitiveType">The <see cref="PrimitiveType" />.</param>
        /// <param name="vertices">The vertices which explictly define the geometry to render.</param>
        /// <param name="startVertex">The starting vertex index to use from <paramref name="vertices" />.</param>
        /// <param name="vertexCount">
        ///     The number of vertices to use from <paramref name="vertices" /> starting from
        ///     <paramref name="startVertex" />.
        /// </param>
        /// <param name="indices">
        ///     The indices of the <paramref name="vertices" /> to render.
        /// </param>
        /// <param name="startIndex">The starting index to use from <paramref name="indices" />.</param>
        /// <param name="indexCount">
        ///     The number of indices to use from <paramref name="indices" /> starting from
        ///     <paramref name="startIndex" />.
        /// </param>
        /// <param name="data">The user data associated with the geometry.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the geometry when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     This method was called before <see cref="Begin" />. <see cref="Begin" /> must be called before you can call
        ///     <see cref="Draw(PrimitiveType,TVertexType[],int,int,int[],int,int,ref TBatchItemData,uint)" />.
        /// </exception>
        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, int[] indices, int startIndex, int indexCount, ref TBatchItemData data, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(ref data, primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, sortKey);
        }

        /// <summary>
        ///     Adds a single-pixel line to a batch of geometry for rendering using the specified effect, two vertices, and
        ///     optional sort key.
        /// </summary>
        /// <param name="firstVertex">The first vertex.</param>
        /// <param name="secondVertex">The second vertex.</param>
        /// <param name="data">The user data associated with the geometry.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the triangle when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     This method was called before <see cref="Begin" />. <see cref="Begin" /> must be called before you can call
        ///     <see cref="DrawLine" />.
        /// </exception>
        public void DrawLine(ref TVertexType firstVertex, ref TVertexType secondVertex, ref TBatchItemData data, uint sortKey = 0)
        {
            EnsureHasBegun();
            _verticesArrayBuffer[0] = firstVertex;
            _verticesArrayBuffer[1] = secondVertex;
            _currentBatchQueuer.EnqueueDraw(ref data, PrimitiveType.LineList, _verticesArrayBuffer, 0, 2, PrimitiveBatchHelper.QuadrilateralClockwiseIndices, 0, 2, sortKey);
        }

        /// <summary>
        ///     Adds a triangle to a batch of geometry for rendering using the specified effect, three corner vertices, and
        ///     optional sort key.
        /// </summary>
        /// <param name="firstVertex">The first corner verex.</param>
        /// <param name="secondVertex">The second corner vertex.</param>
        /// <param name="thirdVertex">The third corner vertex.</param>
        /// <param name="data">The user data associated with the geometry.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the triangle when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     This method was called before <see cref="Begin" />. <see cref="Begin" /> must be called before you can call
        ///     <see cref="DrawTriangle" />.
        /// </exception>
        public void DrawTriangle(ref TVertexType firstVertex, ref TVertexType secondVertex, ref TVertexType thirdVertex, ref TBatchItemData data, uint sortKey = 0)
        {
            EnsureHasBegun();
            _verticesArrayBuffer[0] = firstVertex;
            _verticesArrayBuffer[1] = secondVertex;
            _verticesArrayBuffer[2] = thirdVertex;
            _currentBatchQueuer.EnqueueDraw(ref data, PrimitiveType.TriangleList, _verticesArrayBuffer, 0, 3, PrimitiveBatchHelper.QuadrilateralClockwiseIndices, 0, 3, sortKey);
        }

        /// <summary>
        ///     Adds a quadrilateral (a convex polygon with four sides) to a batch of geometry for rendering using the specified
        ///     effect, four corner vertices, and optional sort
        ///     key.
        /// </summary>
        /// <param name="firstVertex">The first corner vertex.</param>
        /// <param name="secondVertex">The second corner vertex.</param>
        /// <param name="thirdVertex">The third corcner vertex.</param>
        /// <param name="fourthVertex">The fourth corner vertex.</param>
        /// <param name="data">The user data associated with the geometry.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the quadrilateral when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     This method was called before <see cref="Begin" />. <see cref="Begin" /> must be called before you can call
        ///     <see cref="DrawQuadrilateral" />.
        /// </exception>
        public void DrawQuadrilateral(ref TVertexType firstVertex, ref TVertexType secondVertex, ref TVertexType thirdVertex, ref TVertexType fourthVertex, ref TBatchItemData data, uint sortKey = 0)
        {
            EnsureHasBegun();
            _verticesArrayBuffer[0] = firstVertex;
            _verticesArrayBuffer[1] = secondVertex;
            _verticesArrayBuffer[2] = thirdVertex;
            _verticesArrayBuffer[3] = fourthVertex;
            _currentBatchQueuer.EnqueueDraw(ref data, PrimitiveType.TriangleList, _verticesArrayBuffer, 0, 4, PrimitiveBatchHelper.QuadrilateralClockwiseIndices, 0, 6, sortKey);
        }
    }
}
