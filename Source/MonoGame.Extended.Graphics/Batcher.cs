using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Minimizes draw calls to a <see cref="GraphicsDevice" /> by sorting them and attempting to merge them together
    ///     before submitting them.
    /// </summary>
    /// <typeparam name="TDrawCallInfo">The type of the information for a draw call.</typeparam>
    /// <seealso cref="IDisposable" />
    public abstract class Batcher<TDrawCallInfo> : IDisposable
        where TDrawCallInfo : struct, IBatchDrawCallInfo<TDrawCallInfo>, IComparable<TDrawCallInfo>
    {
        internal const int DefaultBatchMaximumDrawCallsCount = 2048;
        private BlendState _blendState;
        private SamplerState _samplerState;
        private DepthStencilState _depthStencilState;
        private RasterizerState _rasterizerState;
        private readonly Effect _defaultEffect;
        private Effect _currentEffect;
        private Matrix? _viewMatrix;
        private Matrix? _projectionMatrix;

        /// <summary>
        ///     The array of <see cref="TDrawCallInfo" /> structs currently enqueued.
        /// </summary>
        protected TDrawCallInfo[] DrawCalls;

        /// <summary>
        ///     The number of <see cref="TDrawCallInfo" /> structs currently enqueued.
        /// </summary>
        protected int EnqueuedDrawCallCount;

        /// <summary>
        ///     Gets the <see cref="GraphicsDevice" /> associated with this <see cref="Batcher{TDrawCallInfo}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GraphicsDevice" /> associated with this <see cref="Batcher{TDrawCallInfo}" />.
        /// </value>
        public GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        ///     Gets a value indicating whether batching is currently in progress by being within a <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair block of code.
        /// </summary>
        /// <value>
        ///     <c>true</c> if batching has begun; otherwise, <c>false</c>.
        /// </value>
        public bool HasBegun { get; internal set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Batcher{TDrawCallInfo}" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="defaultEffect">The default effect.</param>
        /// <param name="maximumDrawCallsCount">
        ///     The maximum number of <see cref="TDrawCallInfo" /> structs that can be enqueued before a
        ///     <see cref="Batcher{TDrawCallInfo}.Flush" />
        ///     is required. The default value is <code>2048</code>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="graphicsDevice" /> is
        ///     null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumDrawCallsCount" /> is less than or equal
        ///     <code>0</code>.
        /// </exception>
        protected Batcher(GraphicsDevice graphicsDevice, Effect defaultEffect,
            int maximumDrawCallsCount = DefaultBatchMaximumDrawCallsCount)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            if (maximumDrawCallsCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumDrawCallsCount));

            GraphicsDevice = graphicsDevice;
            _defaultEffect = defaultEffect;
            DrawCalls = new TDrawCallInfo[maximumDrawCallsCount];
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="diposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool diposing)
        {
            if (!diposing)
                return;
            _defaultEffect.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnsureHasBegun([CallerMemberName] string callerMemberName = null)
        {
            if (!HasBegun)
                throw new InvalidOperationException(
                    $"The {nameof(Begin)} method must be called before the {callerMemberName} method can be called.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnsureHasNotBegun([CallerMemberName] string callerMemberName = null)
        {
            if (HasBegun)
                throw new InvalidOperationException(
                    $"The {nameof(End)} method must be called before the {callerMemberName} method can be called.");
        }

        /// <summary>
        ///     Begins the batch operation using an optional <see cref="BlendState" />, <see cref="SamplerState" />,
        ///     <see cref="DepthStencilState" />, <see cref="RasterizerState" />, <see cref="Effect" />, world-to-view
        ///     <see cref="Matrix" />, or view-to-projection <see cref="Matrix" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default objects for <paramref name="blendState" />, <paramref name="samplerState" />,
        ///         <paramref name="depthStencilState" />, and <paramref name="rasterizerState" /> are
        ///         <see cref="BlendState.AlphaBlend" />, <see cref="SamplerState.LinearClamp" />,
        ///         <see cref="DepthStencilState.None" /> and <see cref="RasterizerState.CullCounterClockwise" /> respectively.
        ///         Passing
        ///         <code>null</code> for any of the previously mentioned parameters result in using their default object.
        ///     </para>
        /// </remarks>
        /// <param name="blendState">The <see cref="BlendState" /> to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" />, <see cref="End" /> pair.</param>
        /// <param name="samplerState">
        ///     The texture <see cref="SamplerState" /> to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair.
        /// </param>
        /// <param name="depthStencilState">
        ///     The <see cref="DepthStencilState" /> to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair.
        /// </param>
        /// <param name="rasterizerState">
        ///     The <see cref="RasterizerState" /> to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair.
        /// </param>
        /// <param name="effect">The <see cref="Effect" /> to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and <see cref="End" /> pair.</param>
        /// <param name="viewMatrix">
        ///     The world-to-view transformation matrix to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair.
        /// </param>
        /// <param name="projectionMatrix">
        ///     The view-to-projection transformation matrix to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> cannot be invoked again until <see cref="End" /> has been invoked.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         This method must be called before any enqueuing of draw calls. When all the geometry have been enqueued for
        ///         drawing, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        public virtual void Begin(Matrix? viewMatrix = null, Matrix? projectionMatrix = null, BlendState blendState = null, SamplerState samplerState = null,
            DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null)
        {
            var viewMatrix1 = viewMatrix ?? Matrix.Identity;
            var projectionMatrix1 = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1);
            Begin(ref viewMatrix1, ref projectionMatrix1, blendState, samplerState, depthStencilState, rasterizerState, effect);
        }

        /// <summary>
        ///     Begins the batch operation using an optional <see cref="BlendState" />, <see cref="SamplerState" />,
        ///     <see cref="DepthStencilState" />, <see cref="RasterizerState" />, <see cref="Effect" />, world-to-view
        ///     <see cref="Matrix" />, or view-to-projection <see cref="Matrix" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default objects for <paramref name="blendState" />, <paramref name="samplerState" />,
        ///         <paramref name="depthStencilState" />, and <paramref name="rasterizerState" /> are
        ///         <see cref="BlendState.AlphaBlend" />, <see cref="SamplerState.LinearClamp" />,
        ///         <see cref="DepthStencilState.None" /> and <see cref="RasterizerState.CullCounterClockwise" /> respectively.
        ///         Passing
        ///         <code>null</code> for any of the previously mentioned parameters result in using their default object.
        ///     </para>
        /// </remarks>
        /// <param name="blendState">The <see cref="BlendState" /> to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" />, <see cref="End" /> pair.</param>
        /// <param name="samplerState">
        ///     The texture <see cref="SamplerState" /> to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair.
        /// </param>
        /// <param name="depthStencilState">
        ///     The <see cref="DepthStencilState" /> to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair.
        /// </param>
        /// <param name="rasterizerState">
        ///     The <see cref="RasterizerState" /> to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair.
        /// </param>
        /// <param name="effect">The <see cref="Effect" /> to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and <see cref="End" /> pair.</param>
        /// <param name="viewMatrix">
        ///     The world-to-view transformation matrix to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair.
        /// </param>
        /// <param name="projectionMatrix">
        ///     The view-to-projection transformation matrix to use for the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and
        ///     <see cref="End" /> pair.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> cannot be invoked again until <see cref="End" /> has been invoked.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         This method must be called before any enqueuing of draw calls. When all the geometry have been enqueued for
        ///         drawing, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        public virtual void Begin(ref Matrix viewMatrix, ref Matrix projectionMatrix, BlendState blendState = null, SamplerState samplerState = null,
            DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null)
        {
            EnsureHasNotBegun();
            HasBegun = true;

            // Store the states to be applied on End()
            // This ensures that two or more batchers will not affect each other
            _blendState = blendState ?? BlendState.AlphaBlend;
            _samplerState = samplerState ?? SamplerState.PointClamp;
            _depthStencilState = depthStencilState ?? DepthStencilState.None;
            _rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
            _currentEffect = effect ?? _defaultEffect;
            _projectionMatrix = projectionMatrix;
            _viewMatrix = viewMatrix;
        }

        /// <summary>
        ///     Flushes the batched geometry to the <see cref="GraphicsDevice" /> and restores it's state to how it was before
        ///     <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> was called.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="End" /> cannot be invoked until <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> has been invoked.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         This method must be called after all enqueuing of draw calls.
        ///     </para>
        /// </remarks>
        public void End()
        {
            EnsureHasBegun();
            Flush();
            HasBegun = false;
        }

        /// <summary>
        ///     Sorts then submits the (sorted) enqueued draw calls to the <see cref="GraphicsDevice" /> for
        ///     rendering without ending the <see cref="Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> and <see cref="End" /> pair.
        /// </summary>
        protected void Flush()
        {
            if (EnqueuedDrawCallCount == 0)
                return;
            SortDrawCallsAndBindBuffers();
            ApplyStates();
            SubmitDrawCalls();
            RestoreStates();
        }

        /// <summary>
        ///     Sorts the enqueued draw calls and binds any used <see cref="VertexBuffer" /> or <see cref="IndexBuffer" /> to the <see cref="GraphicsDevice" />.
        /// </summary>
        protected abstract void SortDrawCallsAndBindBuffers();

        private void ApplyStates()
        {
            var oldBlendState = GraphicsDevice.BlendState;
            var oldSamplerState = GraphicsDevice.SamplerStates[0];
            var oldDepthStencilState = GraphicsDevice.DepthStencilState;
            var oldRasterizerState = GraphicsDevice.RasterizerState;

            GraphicsDevice.BlendState = _blendState;

            GraphicsDevice.SamplerStates[0] = _samplerState;
            GraphicsDevice.DepthStencilState = _depthStencilState;
            GraphicsDevice.RasterizerState = _rasterizerState;

            _blendState = oldBlendState;
            _samplerState = oldSamplerState;
            _depthStencilState = oldDepthStencilState;
            _rasterizerState = oldRasterizerState;

            var viewMatrix = _viewMatrix ?? Matrix.Identity;
            var projectionMatrix = _projectionMatrix ??
                                   Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
                                       GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            var matrixChainEffect = _currentEffect as IMatrixChainEffect;
            if (matrixChainEffect != null)
            {
                matrixChainEffect.World = Matrix.Identity;
                matrixChainEffect.SetView(ref viewMatrix);
                matrixChainEffect.SetProjection(ref projectionMatrix);
            }
            else
            {
                var effectMatrices = _currentEffect as IEffectMatrices;
                if (effectMatrices == null)
                    return;
                effectMatrices.World = Matrix.Identity;
                effectMatrices.View = viewMatrix;
                effectMatrices.Projection = projectionMatrix;
            }
        }

        private void RestoreStates()
        {
            GraphicsDevice.BlendState = _blendState;
            GraphicsDevice.SamplerStates[0] = _samplerState;
            GraphicsDevice.DepthStencilState = _depthStencilState;
            GraphicsDevice.RasterizerState = _rasterizerState;
        }

        /// <summary>
        ///     Enqueues draw call information.
        /// </summary>
        /// <param name="drawCall">The draw call information.</param>
        /// <remarks>
        ///     <para>
        ///         If possible, the <paramref name="drawCall" /> is merged with the last enqueued draw call information instead of
        ///         being
        ///         enqueued.
        ///     </para>
        ///     <para>
        ///         If the enqueue buffer is full, a <see cref="Flush" /> is invoked and then afterwards
        ///         <paramref name="drawCall" /> is enqueued.
        ///     </para>
        /// </remarks>
        protected void Enqueue(ref TDrawCallInfo drawCall)
        {
            if (EnqueuedDrawCallCount > 0 && drawCall.TryMerge(ref DrawCalls[EnqueuedDrawCallCount - 1]))
                return;
            if (EnqueuedDrawCallCount >= DrawCalls.Length)
                Flush();
            DrawCalls[EnqueuedDrawCallCount++] = drawCall;
        }

        /* It might be better to have derived classes just implement the for loop instead of having this virtual method call...
         *      However, if the derived class is only going to override this method once and the code is short, which should both be
         *      true, then maybe we can get away with this virtual method call by having it inlined. So tell the JIT or AOT compiler
         *      we would like it be so. This does NOT guarantee the compiler will respect our wishes.
         */

        /// <summary>
        ///     Submits a draw operation to the <see cref="GraphicsDevice" /> using the specified <see cref="TDrawCallInfo"/>.
        /// </summary>
        /// <param name="drawCall">The draw call information.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void InvokeDrawCall(ref TDrawCallInfo drawCall);

        private void SubmitDrawCalls()
        {
            if (EnqueuedDrawCallCount == 0)
                return;

            for (var i = 0; i < EnqueuedDrawCallCount; i++)
            {
                DrawCalls[i].SetState(_currentEffect);

                foreach (var pass in _currentEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    InvokeDrawCall(ref DrawCalls[i]);
                }
            }

            Array.Clear(DrawCalls, 0, EnqueuedDrawCallCount);
            EnqueuedDrawCallCount = 0;
        }
    }
}
 