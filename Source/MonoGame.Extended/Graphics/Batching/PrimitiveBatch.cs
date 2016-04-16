using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public class PrimitiveBatch<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        public const int DefaultMaximumBatchSize = 8192;

        private BatchDrawer<TVertexType> _batchDrawer;
        private BatchQueuer<TVertexType> _currentBatchQueuer;
        private BatchQueuer<TVertexType>[] _batchQueuers;

        public IDrawContext DefaultDrawContext { get; }
        public BatchDrawStrategy DrawStrategy { get; }
        public bool HasBegun { get; private set; }
        public GraphicsDevice GraphicsDevice { get; }

        public PrimitiveBatch(GraphicsDevice graphicsDevice, BatchDrawStrategy batchDrawStrategy = BatchDrawStrategy.UserPrimitives, IDrawContext defaultDrawContext = null, int maximumBatchSize = DefaultMaximumBatchSize)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }

            GraphicsDevice = graphicsDevice;

            if (defaultDrawContext == null)
            {
                var viewport = graphicsDevice.Viewport;
                var basicEffect = new BasicEffect(graphicsDevice)
                {
                    VertexColorEnabled = true,
                    TextureEnabled = true,
                    Projection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1),
                    World = Matrix.Identity,
                    View = Matrix.Identity
                };
                defaultDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            }
            DefaultDrawContext = defaultDrawContext;

            DrawStrategy = batchDrawStrategy;
            switch (batchDrawStrategy)
            {
                case BatchDrawStrategy.UserPrimitives:
                {
                    _batchDrawer = new UserPrimitivesBatchDrawer<TVertexType>(graphicsDevice, DefaultDrawContext, maximumBatchSize);
                    break;
                }
                case BatchDrawStrategy.DynamicVertexBuffer:
                {
                    _batchDrawer = new DynamicVertexBufferBatchDrawer<TVertexType>(graphicsDevice, DefaultDrawContext, maximumBatchSize);
                    break;
                }
            }

            var batchSortModes = (BatchSortMode[])Enum.GetValues(typeof (BatchSortMode));
            _batchQueuers = new BatchQueuer<TVertexType>[batchSortModes.Length];

            for (var index = 0; index < batchSortModes.Length; index++)
            {
                var batchSortMode = (BatchSortMode)index;
                BatchQueuer<TVertexType> batchQueuer = null;
                switch (batchSortMode)
                {
                    case BatchSortMode.Immediate:
                    {
                        batchQueuer = new ImmediateBatchQueuer<TVertexType>(_batchDrawer);
                        break;
                    }
                    case BatchSortMode.Deferred:
                    {
                        batchQueuer = new DeferredBatchQueuer<TVertexType>(_batchDrawer);
                        break;
                    }
                    case BatchSortMode.DrawContext:
                    {
                        batchQueuer = new DrawContextBatchQueuer<TVertexType>(_batchDrawer);
                        break;
                    }
                }
                _batchQueuers[index] = batchQueuer;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            _batchDrawer.Dispose();
            _batchDrawer = null;

            foreach (var batchQueuer in _batchQueuers)
            {
                batchQueuer.Dispose();
            }

            Array.Clear(_batchQueuers, 0, _batchQueuers.Length);
            _batchQueuers = null;
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

        public void Begin(BatchSortMode sortMode)
        {
            EnsureHasNotBegun();
            HasBegun = true;
            _currentBatchQueuer = _batchQueuers[(int)sortMode];
            _currentBatchQueuer.Begin();
        }

        public void End()
        {
            EnsureHasBegun();
            HasBegun = false;
            _currentBatchQueuer.End();
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, IDrawContext drawContext = null)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, 0, vertices.Length, drawContext);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, IDrawContext drawContext = null)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, startVertex, vertexCount, drawContext);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, short[] indices, IDrawContext drawContext = null)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, 0, vertices.Length, indices, 0, indices.Length, drawContext);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, IDrawContext drawContext = null)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, drawContext);
        }
    }
}
