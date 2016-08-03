using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Shapes.Explicit;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Enables a group of shapes and sprites to drawn using the same settings.
    /// </summary>
    /// <seealso cref="PrimitiveBatch{TVertexType,TBatchItemData}" />
    public class ShapeBatch : PrimitiveBatch<VertexPositionColorTexture, ShapeBatch.DrawContext>
    {
        internal const ushort DefaultMaximumVerticesCount = 8192;
        internal const ushort DefaultMaximumIndicesCount = 12288;

        private readonly GeometryBuilder<VertexPositionColorTexture> _geometryBuilder;
        private DrawContext _shapeItem;
        private Matrix _defaultWorld;
        private Matrix _defaultView;
        private Matrix _defaultProjection;
        private readonly ShapeEffect _effect;
        private readonly Texture2D _pixelTexture;

        /// <summary>
        ///     Gets or sets the number of circle segments used when drawing circles or arcs. The default value is <code>64</code>.
        /// </summary>
        /// <value>
        ///     The default number of circle segments used when drawing circles or arcs.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         Increasing <see cref="CircleSegmentsCount" /> will result in smoother circles and arcs at the cost of
        ///         increased CPU computation.
        ///     </para>
        /// </remarks>
        public int CircleSegmentsCount { get; set; } = ExplicitShapeBuilder.DefaultCircleSegmentsCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShapeBatch" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="maximumVerticesCount">The maximum number of vertices. The default value is <code>8192</code>.</param>
        /// <param name="maximumIndicesCount">The maximum number of indices. The default value is <code>12288</code>.</param>
        public ShapeBatch(GraphicsDevice graphicsDevice, int maximumVerticesCount = DefaultMaximumVerticesCount, int maximumIndicesCount = DefaultMaximumIndicesCount)
            : base(new GeometryBuffer<VertexPositionColorTexture>(graphicsDevice, GeometryBufferType.Dynamic, maximumVerticesCount, maximumIndicesCount))
        {
            _geometryBuilder = new GeometryBuilder<VertexPositionColorTexture>(GeometryBuffer.Enqueue, GeometryBuffer.Enqueue);

            _effect = new ShapeEffect(graphicsDevice);

            var viewport = graphicsDevice.Viewport;

            _defaultWorld = Matrix.Identity;
            _defaultView = Matrix.Identity;
            _defaultProjection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1);

            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[]
            {
                Color.White
            });

            _shapeItem = new DrawContext
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
            {
                return;
            }

            _shapeItem.Texture = null;
            _pixelTexture?.Dispose();
        }

        /// <summary>
        ///     Starts a group of shapes and sprites for rendering with the specified <see cref="BatchMode" />,
        ///     <see cref="Effect" /> and the <see cref="Matrix" /> for transforming world, view and projection spaces.
        /// </summary>
        /// <param name="batchMode">The batch mode. Default value is <see cref="BatchMode.Deferred" />.</param>
        /// <param name="effect">The <see cref="Effect" />. Use <code>null</code> to use the default <see cref="ShapeEffect" />.</param>
        /// <param name="transformWorld">
        ///     The <see cref="Matrix" /> to transform vertices from the model space to the world space.
        ///     Use <code>null</code> to use <see cref="Matrix.Identity" />.
        /// </param>
        /// <param name="transformView">
        ///     The <see cref="Matrix" /> to transform vertices from the world space to the view (a.k.a.
        ///     camera) space. Use <code>null</code> to use <see cref="Matrix.Identity" />.
        /// </param>
        /// <param name="transformProjection">
        ///     The <see cref="Matrix" /> to transform vertices from the view space to the screen
        ///     space. Use <code>null</code> to use an orthographic projection (a 2D projection) with <code>(0,0)</code> as the
        ///     top-left and <code>(x,y)</code> as the bottom-right of the screen or window.
        /// </param>
        public void Begin(BatchMode batchMode = BatchMode.Deferred, Effect effect = null, Matrix? transformWorld = null, Matrix? transformView = null, Matrix? transformProjection = null)
        {
            if (effect == null)
            {
                effect = _effect;
            }

            var effectWorldViewProjectionMatrix = effect as IEffectWorldViewProjectionMatrix;
            if (effectWorldViewProjectionMatrix != null)
            {
                if (transformWorld.HasValue)
                {
                    effectWorldViewProjectionMatrix.World = transformWorld.Value;
                }
                else
                {
                    effectWorldViewProjectionMatrix.SetWorld(ref _defaultWorld);
                }

                if (transformView.HasValue)
                {
                    effectWorldViewProjectionMatrix.View = transformView.Value;
                }
                else
                {
                    effectWorldViewProjectionMatrix.SetView(ref _defaultView);
                }

                if (transformProjection.HasValue)
                {
                    effectWorldViewProjectionMatrix.Projection = transformProjection.Value;
                }
                else
                {
                    effectWorldViewProjectionMatrix.SetProjection(ref _defaultProjection);
                }
            }

            base.Begin(batchMode, effect);
        }

        /// <summary>
        ///     Draws the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="color">The color.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="sortKey">The sort key.</param>
        public void Draw(ref Line2F line, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildLine(out primitiveType, ref line, color, depth, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItem, sortKey);
        }

        /// <summary>
        ///     Draws the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="color">The color.</param>
        /// <param name="sortKey">The sort key.</param>
        public void Draw(ref Line3F line, Color color, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildLine(out primitiveType, ref line, color, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItem, sortKey);
        }

        /// <summary>
        ///     Draws the specified arc with fill mode.
        /// </summary>
        /// <param name="arc">The arc.</param>
        /// <param name="color">The color.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="sortKey">The sort key.</param>
        public void Draw(ref ArcF arc, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildArc(out primitiveType, ref arc, color, depth, CircleSegmentsCount, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItem, sortKey);
        }

        /// <summary>
        ///     Draws the specified arc with stroke mode.
        /// </summary>
        /// <param name="arc">The arc.</param>
        /// <param name="color">The color.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="sortKey">The sort key.</param>
        public void DrawOutline(ref ArcF arc, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildArcOutline(out primitiveType, ref arc, color, depth, CircleSegmentsCount, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItem, sortKey);
        }

        /// <summary>
        ///     Draws the specified circle with fill mode.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <param name="color">The color.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="sortKey">The sort key.</param>
        public void Draw(ref CircleF circle, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildCircle(out primitiveType, ref circle, color, depth, CircleSegmentsCount, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItem, sortKey);
        }

        /// <summary>
        ///     Draws the specified circle with stroke mode.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <param name="color">The color.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="sortKey">The sort key.</param>
        public void DrawOutline(ref CircleF circle, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildCircleOutline(out primitiveType, ref circle, color, depth, CircleSegmentsCount, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItem, sortKey);
        }

        /// <summary>
        ///     Draws the specified rectangle with fill mode.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="color">The color.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="sortKey">The sort key.</param>
        public void Draw(ref RectangleF rectangle, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildRectangle(out primitiveType, ref rectangle, color, depth, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItem, sortKey);
        }

        /// <summary>
        ///     Draws the specified rectangle with stroke mode.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="color">The color.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="sortKey">The sort key.</param>
        public void DrawOutline(ref RectangleF rectangle, Color color, float depth = 0f, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildRectangleOutline(out primitiveType, ref rectangle, color, depth, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref _shapeItem, sortKey);
        }

        /// <summary>
        ///     Draws the specified sprite.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="sortKey">The sort key.</param>
        public void Draw(Sprite sprite, float depth = 0, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            PrimitiveType primitiveType;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;

            _geometryBuilder.BuildSprite(out primitiveType, sprite, depth, startVertex);

            var vertexCount = geometryBuffer.VertexCount - startVertex;
            var indexCount = geometryBuffer.IndexCount - startIndex;

            var itemData = new DrawContext(sprite.TextureRegion.Texture);

            EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref itemData, sortKey);
        }

        /// <summary>
        ///     Defines a drawing context for shapes and sprites.
        /// </summary>
        /// <seealso cref="IDrawContext{TData}" />
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DrawContext : IDrawContext<DrawContext>
        {
            public Texture2D Texture;

            internal DrawContext(Texture2D texture)
            {
                Texture = texture;
            }

            public bool Equals(ref DrawContext other)
            {
                return other.Texture == Texture;
            }

            public void ApplyTo(Effect effect)
            {
                var textureEffect = effect as IEffectTexture2D;
                if (textureEffect != null)
                {
                    textureEffect.Texture = Texture;
                }
            }
        }
    }
}
