using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Enables <see cref="GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, int, int, int)" /> method calls to be
    ///     deferred so they can be sorted to minimize state changes.
    /// </summary>
    /// <typeparam name="TDrawCallData">The type of the draw call data.</typeparam>
    /// <seealso cref="Renderer" />
    public abstract class QueueRenderer<TDrawCallData> : Renderer where TDrawCallData : struct, IDrawCallData
    {
        internal const int DefaultMaximumDrawCallsCount = 2048;

        /// <summary>
        ///     Gets or sets the number of derferred draw calls currently enqueued.
        /// </summary>
        /// <value>
        ///     The number of deferred draw calls currently enqueued.
        /// </value>
        protected int EnqueuedDrawCallsCount { get; set; }

        /// <summary>
        ///     Gets the deferred draw calls.
        /// </summary>
        /// <value>
        ///     The draw deferred draw calls.
        /// </value>
        protected DrawCall<TDrawCallData>[] DrawCalls { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueueRenderer{TBatchDrawCommandData}" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="defaultEffect">The default effect.</param>
        /// <param name="maximumDrawCallsCount">
        ///     The maximum number of draw calls that can be deferred. The default value is <code>2024</code>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice"/>, or <paramref name="defaultEffect"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumDrawCallsCount" /> is less than or equal
        ///     <code>0</code>.
        /// </exception>
        protected QueueRenderer(GraphicsDevice graphicsDevice, Effect defaultEffect,
            int maximumDrawCallsCount = DefaultMaximumDrawCallsCount)
            : base(graphicsDevice, defaultEffect)
        {
            if (maximumDrawCallsCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumDrawCallsCount));

            DrawCalls = new DrawCall<TDrawCallData>[maximumDrawCallsCount];
        }

        /// <summary>
        ///     Attempts to enqueue a <see cref="GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, int, int, int)" /> method call
        ///     to for sorting.
        /// </summary>
        /// <param name="drawCall">The draw call.</param>
        /// <returns><c>false</c> if the enqueue buffer is full; otherwise, <c>true</c>.</returns>
        protected bool TryEnqueueDrawCall(ref DrawCall<TDrawCallData> drawCall)
        {
            if (EnqueuedDrawCallsCount >= DrawCalls.Length)
                return false;

            DrawCalls[EnqueuedDrawCallsCount++] = drawCall;
            return true;
        }

        /// <summary>
        ///     Sorts then submits the (sorted) enqueued <see cref="DrawCalls" /> to the <see cref="GraphicsDevice" /> for
        ///     rendering without ending the rendering pass.
        /// </summary>
        public virtual void Flush()
        {
            ApplyStates();
            Array.Sort(DrawCalls, 0, EnqueuedDrawCallsCount);
            SubmitDrawCalls();
        }

        /// <summary>
        ///     Ends the rendering pass.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="End" /> cannot be invoked until <see cref="Renderer.Begin" /> has been invoked.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         This method must be called after all enqueuing of draw calls.
        ///     </para>
        /// </remarks>
        public override void End()
        {
            EnsureHasBegun();
            Flush();
            HasBegun = false;
        }

        /// <summary>
        ///     Submits the draw calls to the <see cref="GraphicsDevice" />.
        /// </summary>
        protected virtual void SubmitDrawCalls()
        {
            // do the actual rendering using the graphics API
            for (var i = 0; i < EnqueuedDrawCallsCount; i++)
            {
                // change the rendering state if necessary
                DrawCalls[i].Data.ApplyTo(Effect);

                foreach (var pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    var primitiveType = DrawCalls[i].PrimitiveType;
                    var baseVertex = DrawCalls[i].BaseVertex;
                    var startIndex = DrawCalls[i].StartIndex;
                    var primitiveCount = DrawCalls[i].PrimitiveCount;
                    GraphicsDevice.DrawIndexedPrimitives(primitiveType, baseVertex, startIndex, primitiveCount);
                }

                // prevent a possible memory leak where the draw calls would be holding onto some reference(s)
                DrawCalls[i].Data.SetReferencesToNull();
            }

            EnqueuedDrawCallsCount = 0;
        }
    }
}
