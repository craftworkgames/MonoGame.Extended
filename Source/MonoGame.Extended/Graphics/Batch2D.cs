using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Enables a group of dynamic two-dimensional geometric objects be drawn using the same settings.
    /// </summary>
    /// <seealso cref="Batch{TVertexType,TBatchDrawCommandData}" />
    public class Batch2D : Batch<VertexPositionColorTexture, Batch2D.DrawCommandData>
    {
        internal const int DefaultMaximumVerticesCount = 8192;
        internal const int DefaultMaximumIndicesCount = 12288;

        private DrawCommandData _pixelTextureDrawContext;
        private Matrix _defaultWorld = Matrix.Identity;
        private Matrix _defaultView = Matrix.Identity;
        private Matrix _defaultProjection;
        private readonly DefaultEffect2D _effect;
        private readonly Texture2D _pixelTexture;
        private Batch2DSortMode _sortMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Batch2D" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="maximumVerticesCount">The maximum number of vertices. The default value is <code>8192</code>.</param>
        /// <param name="maximumIndicesCount">The maximum number of indices. The default value is <code>12288</code>.</param>
        /// <param name="maximumBatchCommandsCount">
        ///     The maximum number of draw calls that can be deferred before they have to be
        ///     submitted to the <see cref="GraphicsDevice" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumBatchCommandsCount" /> is less than or equal
        ///     <code>0</code>, or <paramref name="maximumVerticesCount" /> is less than or equal to <code>0</code>, or,
        ///     <paramref name="maximumVerticesCount" /> is less than or equal to <code>0</code>.
        /// </exception>
        public Batch2D(GraphicsDevice graphicsDevice, ushort maximumVerticesCount = DefaultMaximumVerticesCount,
            ushort maximumIndicesCount = DefaultMaximumIndicesCount,
            int maximumBatchCommandsCount = DefaultMaximumBatchCommandsCount)
            : base(
                new DynamicGeometryBuffer<VertexPositionColorTexture>(graphicsDevice, maximumVerticesCount,
                    maximumIndicesCount), maximumBatchCommandsCount)
        {
            _effect = new DefaultEffect2D(graphicsDevice);

            var viewport = graphicsDevice.Viewport;

            _defaultProjection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) *
                                 Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1);

            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[]
            {
                Color.White
            });

            _pixelTextureDrawContext = new DrawCommandData
            {
                Texture = _pixelTexture
            };
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _pixelTextureDrawContext.Texture = null;
            _pixelTexture?.Dispose();
        }

        /// <summary>
        ///     Starts a group of two-dimensional geometry for rendering with the specified <see cref="BatchSortMode" />,
        ///     <see cref="Effect" /> and the optional chain of <see cref="Matrix" />es for transforming between world, view, and
        ///     projection spaces.
        /// </summary>
        /// <param name="sortMode">The <see cref="Batch2DSortMode" />. Default value is <see cref="Batch2DSortMode.Deferred" />.</param>
        /// <param name="effect">
        ///     The <see cref="Effect" />. Use <code>null</code> to use the default <see cref="DefaultEffect2D" />.
        /// </param>
        /// <param name="worldMatrix">
        ///     The <see cref="Matrix" /> to transform vertices from a common model-space to world-space. Use <code>null</code> to
        ///     use <see cref="Matrix.Identity" />.
        /// </param>
        /// <param name="viewMatrix">
        ///     The <see cref="Matrix" /> to transform vertices from the world-space to view-space. Use <code>null</code> to
        ///     use <see cref="Matrix.Identity" />.
        /// </param>
        /// <param name="projectionMatrix">
        ///     The <see cref="Matrix" /> to transform vertices from the view-space to projection-space. Use <code>null</code> to
        ///     use an orthographic projection (a 2D projection) with <code>(0,0)</code> as the top-left and <code>(x,y)</code> as
        ///     the bottom-right of the viewport.
        /// </param>
        public void Begin(Batch2DSortMode sortMode = Batch2DSortMode.Deferred, Effect effect = null,
            Matrix? worldMatrix = null,
            Matrix? viewMatrix = null, Matrix? projectionMatrix = null)
        {
            if (effect == null)
                effect = _effect;

            BatchSortMode batchSortMode;
            switch (sortMode)
            {
                case Batch2DSortMode.Texture:
                case Batch2DSortMode.FrontToBack:
                case Batch2DSortMode.BackToFront:
                    batchSortMode = BatchSortMode.DeferredSorted;
                    break;
                case Batch2DSortMode.Deferred:
                    batchSortMode = BatchSortMode.Deferred;
                    break;
                case Batch2DSortMode.Immediate:
                    batchSortMode = BatchSortMode.Immediate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortMode), sortMode, null);
            }

            _sortMode = sortMode;

            var effectWorldViewProjectionMatrix = effect as IMatrixChainEffect;
            if (effectWorldViewProjectionMatrix != null)
            {
                if (worldMatrix.HasValue)
                    effectWorldViewProjectionMatrix.World = worldMatrix.Value;
                else
                    effectWorldViewProjectionMatrix.SetWorld(ref _defaultWorld);

                if (viewMatrix.HasValue)
                    effectWorldViewProjectionMatrix.View = viewMatrix.Value;
                else
                    effectWorldViewProjectionMatrix.SetView(ref _defaultView);

                if (projectionMatrix.HasValue)
                    effectWorldViewProjectionMatrix.Projection = projectionMatrix.Value;
                else
                    effectWorldViewProjectionMatrix.SetProjection(ref _defaultProjection);
            }

            Begin(effect, PrimitiveType.TriangleList, batchSortMode);
        }

        /// <summary>
        ///     Draws a sprite using a specified <see cref="Texture" />, transform <see cref="Matrix2D" /> and an optional
        ///     source <see cref="Rectangle" />, <see cref="Color" />, origin <see cref="Vector2" />,
        ///     <see cref="SpriteEffects" />, and depth.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix2D" />.</param>
        /// <param name="sourceRectangle">
        ///     The texture region <see cref="Rectangle" /> of the <paramref name="texture" />. Use
        ///     <code>null</code> to use the entire <see cref="Texture2D" />.
        /// </param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="origin">
        ///     The origin <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.Zero" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth. The default value is <code>0</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin"/> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        /// <exception cref="GeometryBufferOverflowException{VertexPositionColorTexture}">
        ///     The underlying
        ///     <see cref="GeometryBuffer{VertexPositionColorTexture}" /> is full.
        /// </exception>
        /// <exception cref="BatchCommandQueueOverflowException">The batch command queue is full.</exception>
        public void DrawSprite(Texture2D texture, ref Matrix2D transformMatrix, Rectangle? sourceRectangle = null,
            Color? color = null, Vector2? origin = null, SpriteEffects effects = SpriteEffects.None,
            float depth = 0)
        {
            var geometryBuffer = GeometryBuffer;
            var startVertex = geometryBuffer._vertexCount;
            var startIndex = geometryBuffer._indexCount;
            geometryBuffer.EnqueueSprite(startVertex, texture, ref transformMatrix, sourceRectangle, color, origin,
                effects, depth);
            var commandData = new DrawCommandData(texture);
            var sortKey = GetSortKey(depth);
            Draw(startIndex, 2, sortKey, ref commandData);
        }

        /// <summary>
        ///     Draws a sprite using a specified <see cref="Texture" />, transform <see cref="Matrix2D" /> and an optional
        ///     source <see cref="Rectangle" />, <see cref="Color" />, origin <see cref="Vector2" />,
        ///     <see cref="SpriteEffects" />, and depth.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="position">The world position.</param>
        /// <param name="sourceRectangle">
        ///     The texture region <see cref="Rectangle" /> of the <paramref name="texture" />. Use
        ///     <code>null</code> to use the entire <see cref="Texture2D" />.
        /// </param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="rotation">The angle (in radians) to rotate the sprite about its <paramref name="origin" />.</param>
        /// <param name="origin">
        ///     The origin <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.Zero" />.
        /// </param>
        /// <param name="scale">
        ///     The scale <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.One" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth. The default value is <code>0f</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin"/> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        /// <exception cref="GeometryBufferOverflowException{VertexPositionColorTexture}">
        ///     The underlying
        ///     <see cref="GeometryBuffer{VertexPositionColorTexture}" /> is full.
        /// </exception>
        /// <exception cref="BatchCommandQueueOverflowException">The batch command queue is full.</exception>
        public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null,
            Color? color = null, float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
            SpriteEffects effects = SpriteEffects.None,
            float depth = 0)
        {
            var matrix = Matrix2D.Identity;

            if (scale.HasValue)
            {
                var scaleMatrix = Matrix2D.CreateScale(scale.Value);
                Matrix2D.Multiply(ref matrix, ref scaleMatrix, out matrix);
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (rotation != 0f)
            {
                var rotationMatrix = Matrix2D.CreateRotationZ(-rotation);
                Matrix2D.Multiply(ref matrix, ref rotationMatrix, out matrix);
            }

            var translationMatrix = Matrix2D.CreateTranslation(position);
            Matrix2D.Multiply(ref matrix, ref translationMatrix, out matrix);

            DrawSprite(texture, ref matrix, sourceRectangle, color, origin, effects, depth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetSortKey(float depth)
        {
            // the sort key acts as the primary sort key
            // the secondary sort key is always the texture

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (_sortMode)
            {
                case Batch2DSortMode.FrontToBack:
                    return depth;
                case Batch2DSortMode.BackToFront:
                    return -depth;
                default:
                    return 0;
            }
        }

        /// <summary>
        ///     Defines a drawing context for two-dimensional geometric objects.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DrawCommandData : IBatchDrawCommandData<DrawCommandData>
        {
            public Texture2D Texture;
            public int TextureKey;

            internal DrawCommandData(Texture2D texture)
            {
                Texture = texture;
                TextureKey = RuntimeHelpers.GetHashCode(texture);
            }

            public void ApplyTo(Effect effect)
            {
                var textureEffect = effect as ITexture2DEffect;
                if (textureEffect != null)
                    textureEffect.Texture = Texture;
            }

            public void SetReferencesToNull()
            {
                Texture = null;
            }

            public bool Equals(ref DrawCommandData other)
            {
                return Texture == other.Texture;
            }

            public int CompareTo(DrawCommandData other)
            {
                return TextureKey.CompareTo(other.TextureKey);
            }
        }
    }
}
