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
    /// <remarks>
    ///     <para>
    ///         <see cref="PrimitiveBatch{TVertexType}" /> is a helper for easily and efficiently drawing dynamically generated
    ///         geometry such as lines and triangles which change frame-to-frame. Dynamic submission is a highly effective
    ///         pattern for drawing procedural geometry and convenient for debug rendering. It is however not as efficient at
    ///         drawing geometry which does not change every frame. Such geometry should use a static
    ///         <see cref="VertexBuffer" /> and possibly a <see cref="IndexBuffer" /> for rendering instead of
    ///         <see cref="PrimitiveBatch{TVertexType}" />.
    ///     </para>
    ///     <para>
    ///         To use a <see cref="PrimitiveBatch{TVertexType}" /> as a <see cref="SpriteBatch" /> use a vertex type of
    ///         <see cref="VertexPositionColorTexture" /> and use the extension
    ///         <see cref="PrimitiveBatchExtensions.DrawSprite" /> to draw a textured quad with sprite parameters. XNA's
    ///         SpriteBatch uses a maximum of 2048 sprites per batch. Since each sprite is two triangles of 4 vertices and 6
    ///         indices, XNA's SpriteBatch uses a maximum of 8192 vertices and 12288 indices per batch.
    ///     </para>
    /// </remarks>
    public class PrimitiveBatch<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        public const ushort DefaultMaximumVerticesCount = 8192;
        public const ushort DefaultMaximumIndicesCount = 12288;

        private BatchDrawer<TVertexType> _batchDrawer;
        private BatchQueuer<TVertexType> _currentBatchQueuer;
        private ImmediateBatchQueuer<TVertexType> _immediateBatchQueuer;
        private DeferredBatchQueuer<TVertexType> _deferredBatchQueuer;
        private static readonly TVertexType[] _verticesArrayBuffer;

        /// <summary>
        ///     Gets the <see cref="GraphicsDevice" /> used by this <see cref="PrimitiveBatch{TVertexType}" />.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        ///     Gets a value indicating whether batching is currently in progress by being within
        ///     a <see cref="Begin" />-
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
        ///     Initializes a new instance of the <see cref="PrimitiveBatch{TVertexType}" /> class.
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

            _batchDrawer = new BatchDrawer<TVertexType>(graphicsDevice, maximumVerticesCount, maximumIndicesCount);
            _immediateBatchQueuer = new ImmediateBatchQueuer<TVertexType>(_batchDrawer);
            _deferredBatchQueuer = new DeferredBatchQueuer<TVertexType>(_batchDrawer);
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
        public void Begin(BatchMode mode = BatchMode.Deferred)
        {
            EnsureHasNotBegun();

            HasBegun = true;
            switch (mode)
            {
                case BatchMode.Immediate:
                    _currentBatchQueuer = _immediateBatchQueuer;
                    break;
                case BatchMode.Deferred:
                    _currentBatchQueuer = _deferredBatchQueuer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
            _currentBatchQueuer.Begin();
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
        ///     Adds geometry to a batch of geometry for rendering using the specified effect, primitive type, vertices, and
        ///     optional sort key.
        /// </summary>
        /// <param name="effect">The <see cref="Effect" /> to use.</param>
        /// <param name="primitiveType">The <see cref="PrimitiveType" />.</param>
        /// <param name="vertices">The vertices which explictly define the geometry to render.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the geometry when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Draw(Effect,PrimitiveType,TVertexType[],uint)" /> was
        ///     called, but <see cref="Begin" /> has not yet been called. <see cref="Begin" /> must be called successfully before
        ///     you can call <see cref="Draw(Effect,PrimitiveType,TVertexType[],uint)" />.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         <paramref name="effect" /> is reponsible for the settings used when drawing the geometry, including any
        ///         render states, shaders, or transformation matrix. To minimize switching of effects between draw
        ///         calls, consider using <paramref name="sortKey" /> to sort by draw effect in conjunction with
        ///         <see cref="BatchMode.Deferred" />.
        ///     </para>
        ///     <para>
        ///         Before making any calls to <see cref="Draw(Effect,PrimitiveType,TVertexType[],uint)" />, you must call
        ///         <see cref="Begin" />. Once all draw calls are complete call <see cref="End" />.
        ///     </para>
        /// </remarks>
        public void Draw(Effect effect, PrimitiveType primitiveType, TVertexType[] vertices, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(effect, primitiveType, vertices, 0, vertices.Length, sortKey);
        }

        /// <summary>
        ///     Adds geometry to a batch of geometry for rendering using the specified effect, primitive type, vertices, start
        ///     vertex, vertex count, and optional sort key.
        /// </summary>
        /// <param name="effect">The <see cref="Effect" /> to use.</param>
        /// <param name="primitiveType">The <see cref="PrimitiveType" />.</param>
        /// <param name="vertices">The vertices which explictly define the geometry to render.</param>
        /// <param name="startVertex">The starting vertex index to use from <paramref name="vertices" />.</param>
        /// <param name="vertexCount">
        ///     The number of vertices to use from <paramref name="vertices" /> starting from
        ///     <paramref name="startVertex" />.
        /// </param>
        /// <param name="sortKey">
        ///     The sort key used to sort the geometry when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Draw(Effect,PrimitiveType,TVertexType[],int,int,uint)" /> was called, but <see cref="Begin" /> has
        ///     not yet been called. <see cref="Begin" /> must be called successfully before you can call
        ///     <see cref="Draw(Effect,PrimitiveType,TVertexType[],int,int,uint)" />.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         <paramref name="effect" /> is reponsible for the settings used when drawing the geometry, including any
        ///         render states, shaders, or transformation matrix. To minimize switching of effects between draw
        ///         calls, consider using <paramref name="sortKey" /> to sort by draw effect in conjunction with
        ///         <see cref="BatchMode.Deferred" />.
        ///     </para>
        ///     <para>
        ///         Before making any calls to <see cref="Draw(Effect,PrimitiveType,TVertexType[],int,int,uint)" />, you must
        ///         call <see cref="Begin" />. Once all draw calls are complete, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        public void Draw(Effect effect, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(effect, primitiveType, vertices, startVertex, vertexCount, sortKey);
        }

        /// <summary>
        ///     Adds geometry to a batch of geometry for rendering using the specified effect, primitive type, vertices, indices,
        ///     and optional sort key.
        /// </summary>
        /// <param name="effect">The <see cref="Effect" /> to use.</param>
        /// <param name="primitiveType">The <see cref="PrimitiveType" />.</param>
        /// <param name="vertices">The vertices which explictly define the geometry to render.</param>
        /// <param name="indices">The indices of the <paramref name="vertices" /> to render.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the geometry when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Draw(Effect,PrimitiveType,TVertexType[],short[],uint)" /> was called, but <see cref="Begin" /> has
        ///     not yet been called. <see cref="Begin" /> must be called successfully before you can call
        ///     <see cref="Draw(Effect,PrimitiveType,TVertexType[],short[],uint)" />.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         <paramref name="effect" /> is reponsible for the settings used when drawing the geometry, including any
        ///         render states, shaders, or transformation matrix. To minimize switching of effects between draw
        ///         calls, consider using <paramref name="sortKey" /> to sort by draw effect in conjunction with
        ///         <see cref="BatchMode.Deferred" />.
        ///     </para>
        ///     <para>
        ///         Before making any calls to <see cref="Draw(Effect,PrimitiveType,TVertexType[],short[],uint)" />, you must
        ///         call <see cref="Begin" />. Once all draw calls are complete, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        public void Draw(Effect effect, PrimitiveType primitiveType, TVertexType[] vertices, short[] indices, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(effect, primitiveType, vertices, 0, vertices.Length, indices, 0, indices.Length, sortKey);
        }

        /// <summary>
        ///     Adds geometry to a batch of geometry for rendering using the specified effect, primitive type, vertices, indices,
        ///     and optional sort key.
        /// </summary>
        /// <param name="effect">The <see cref="Effect" /> to use.</param>
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
        /// <param name="sortKey">
        ///     The sort key used to sort the geometry when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Draw(Effect,PrimitiveType,TVertexType[],int,int,short[],int,int,uint)" /> was called, but
        ///     <see cref="Begin" /> has not yet been called. <see cref="Begin" /> must be called successfully before you can call
        ///     <see cref="Draw(Effect,PrimitiveType,TVertexType[],int,int,short[],int,int,uint)" />.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         <paramref name="effect" /> is reponsible for the settings used when drawing the geometry, including any
        ///         render states, shaders, or transformation matrix. To minimize switching of effects between draw
        ///         calls, consider using <paramref name="sortKey" /> to sort by draw effect in conjunction with
        ///         <see cref="BatchMode.Deferred" />.
        ///     </para>
        ///     <para>
        ///         Before making any calls to
        ///         <see cref="Draw(Effect,PrimitiveType,TVertexType[],int,int,short[],int,int,uint)" />, you must
        ///         call <see cref="Begin" />. Once all draw calls are complete, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        public void Draw(Effect effect, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(effect, primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, sortKey);
        }

        /// <summary>
        ///     Adds a single-pixel line to a batch of geometry for rendering using the specified effect, two vertices, and
        ///     optional sort key.
        /// </summary>
        /// <param name="effect">The <see cref="Effect" /> to use.</param>
        /// <param name="firstVertex">The first vertex.</param>
        /// <param name="secondVertex">The second vertex.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the triangle when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="effect" /> is reponsible for the settings used when drawing the geometry, including any
        ///         render states, shaders, or transformation matrix. To minimize switching of effects between draw
        ///         calls, consider using <paramref name="sortKey" /> to sort by draw effect in conjunction with
        ///         <see cref="BatchMode.Deferred" />.
        ///     </para>
        ///     <para>
        ///         Before making any calls to
        ///         <see cref="DrawLine(Effect,ref TVertexType, ref TVertexType, uint)" />, you must
        ///         call <see cref="Begin" />. Once all draw calls are complete, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        public void DrawLine(Effect effect, ref TVertexType firstVertex, ref TVertexType secondVertex, uint sortKey = 0)
        {
            EnsureHasBegun();
            _verticesArrayBuffer[0] = firstVertex;
            _verticesArrayBuffer[1] = secondVertex;
            _currentBatchQueuer.EnqueueDraw(effect, PrimitiveType.LineList, _verticesArrayBuffer, 0, 2, sortKey);
        }

        /// <summary>
        ///     Adds a triangle to a batch of geometry for rendering using the specified effect, three corner vertices, and
        ///     optional sort
        ///     key.
        /// </summary>
        /// <param name="effect">The <see cref="Effect" /> to use.</param>
        /// <param name="firstVertex">The first corner verex.</param>
        /// <param name="secondVertex">The second corner vertex.</param>
        /// <param name="thirdVertex">The third corner vertex.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the triangle when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="effect" /> is reponsible for the settings used when drawing the geometry, including any
        ///         render states, shaders, or transformation matrix. To minimize switching of effects between draw
        ///         calls, consider using <paramref name="sortKey" /> to sort by draw effect in conjunction with
        ///         <see cref="BatchMode.Deferred" />.
        ///     </para>
        ///     <para>
        ///         Before making any calls to
        ///         <see cref="DrawTriangle(Effect,ref TVertexType, ref TVertexType, ref TVertexType, uint)" />, you must
        ///         call <see cref="Begin" />. Once all draw calls are complete, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        public void DrawTriangle(Effect effect, ref TVertexType firstVertex, ref TVertexType secondVertex, ref TVertexType thirdVertex, uint sortKey = 0)
        {
            EnsureHasBegun();
            _verticesArrayBuffer[0] = firstVertex;
            _verticesArrayBuffer[1] = secondVertex;
            _verticesArrayBuffer[2] = thirdVertex;
            _currentBatchQueuer.EnqueueDraw(effect, PrimitiveType.TriangleList, _verticesArrayBuffer, 0, 3, sortKey);
        }

        /// <summary>
        ///     Adds a quadrilateral (a convex polygon with four sides) to a batch of geometry for rendering using the specified
        ///     effect, four corner vertices, and optional sort
        ///     key.
        /// </summary>
        /// <param name="effect">The <see cref="Effect" /> to use.</param>
        /// <param name="firstVertex">The first corner vertex.</param>
        /// <param name="secondVertex">The second corner vertex.</param>
        /// <param name="thirdVertex">The third corcner vertex.</param>
        /// <param name="fourthVertex">The fourth corner vertex.</param>
        /// <param name="sortKey">
        ///     The sort key used to sort the quadrilateral when rendering with <see cref="BatchMode.Deferred" />. This
        ///     value is ignored if <see cref="BatchMode.Deferred" /> is not used.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="effect" /> is reponsible for the settings used when drawing the geometry, including any
        ///         render states, shaders, or transformation matrix. To minimize switching of effects between draw
        ///         calls, consider using <paramref name="sortKey" /> to sort by draw effect in conjunction with
        ///         <see cref="BatchMode.Deferred" />.
        ///     </para>
        ///     <para>
        ///         Before making any calls to
        ///         <see cref="DrawQuadrilateral(Effect,ref TVertexType, ref TVertexType, ref TVertexType, ref TVertexType, uint)" />
        ///         , you must call <see cref="Begin" />. Once all draw calls are complete, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        public void DrawQuadrilateral(Effect effect, ref TVertexType firstVertex, ref TVertexType secondVertex, ref TVertexType thirdVertex, ref TVertexType fourthVertex, uint sortKey = 0)
        {
            EnsureHasBegun();
            _verticesArrayBuffer[0] = firstVertex;
            _verticesArrayBuffer[1] = secondVertex;
            _verticesArrayBuffer[2] = thirdVertex;
            _verticesArrayBuffer[3] = fourthVertex;
            _currentBatchQueuer.EnqueueDraw(effect, PrimitiveType.TriangleList, _verticesArrayBuffer, 0, 4, PrimitiveBatchHelper.QuadIndices, 0, 6, sortKey);
        }
    }
}
