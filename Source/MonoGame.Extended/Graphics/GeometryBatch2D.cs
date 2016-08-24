using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Enables a group of dynamic two-dimensional geometry to drawn using the same settings.
    /// </summary>
    /// <seealso cref="PrimitiveBatch{TVertexType,TDrawContext}" />
    /// <remarks><para>For geometry that doesn't <see cref="GeometryBatch2D"/> is</para></remarks>
    public class GeometryBatch2D : PrimitiveBatch<VertexPositionColorTexture, GeometryBatch2D.DrawContext2D>
    {
        internal const ushort DefaultMaximumVerticesCount = 8192;
        internal const ushort DefaultMaximumIndicesCount = 12288;

        private DrawContext2D _pixelTextureDrawContext;
        private Matrix _defaultWorld = Matrix.Identity;
        private Matrix _defaultView = Matrix.Identity;
        private Matrix _defaultProjection;
        private readonly DefaultEffect2D _effect;
        private readonly Texture2D _pixelTexture;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GeometryBatch2D" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="maximumVerticesCount">The maximum number of vertices. The default value is <code>8192</code>.</param>
        /// <param name="maximumIndicesCount">The maximum number of indices. The default value is <code>12288</code>.</param>
        public GeometryBatch2D(GraphicsDevice graphicsDevice, int maximumVerticesCount = DefaultMaximumVerticesCount, int maximumIndicesCount = DefaultMaximumIndicesCount)
            : base(geometryBuffer: new DynamicGeometryBuffer<VertexPositionColorTexture>(graphicsDevice, maximumVerticesCount, maximumIndicesCount))
        {
            _effect = new DefaultEffect2D(graphicsDevice);

            var viewport = graphicsDevice.Viewport;

            _defaultProjection = Matrix.CreateTranslation(xPosition: -0.5f, yPosition: -0.5f, zPosition: 0) * Matrix.CreateOrthographicOffCenter(left: 0, right: viewport.Width, bottom: viewport.Height, top: 0, zNearPlane: 0, zFarPlane: -1);

            _pixelTexture = new Texture2D(graphicsDevice, width: 1, height: 1);
            _pixelTexture.SetData(data: new[]
            {
                Color.White
            });

            _pixelTextureDrawContext = new DrawContext2D
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
        ///     Starts a group of two-dimensional geometry for rendering with the specified <see cref="BatchMode" />,
        ///     <see cref="Effect" /> and the optional chain of <see cref="Matrix" />es for transforming between world, view, and projection spaces.
        /// </summary>
        /// <param name="batchMode">The batch mode. Default value is <see cref="BatchMode.Deferred" />.</param>
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
        public void Begin(BatchMode batchMode = BatchMode.Deferred, Effect effect = null, Matrix? worldMatrix = null, Matrix? viewMatrix = null, Matrix? projectionMatrix = null)
        {
            if (effect == null)
                effect = _effect;

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

            Begin(effect, PrimitiveType.TriangleList, batchMode);
        }

        /// <summary>
        ///     Draws a sprite using a specified <see cref="TextureRegion2D" />, transform <see cref="Matrix2D" /> and an optional
        ///     <see cref="Color" />, origin <see cref="Vector2" />, <see cref="SpriteEffects" />, depth, and sort key.
        /// </summary>
        /// <param name="textureRegion">The texture region.</param>
        /// <param name="transformMatrix">The transform matrix.</param>
        /// <param name="color">The color.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="spriteOptions">The sprite options.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="sortKey">The sort key.</param>
        public void DrawSprite(TextureRegion2D textureRegion, ref Matrix2D transformMatrix, Color? color = null, Vector2? origin = null, SpriteEffects spriteOptions = SpriteEffects.None, float depth = 0, uint sortKey = 0)
        {
            var geometryBuffer = GeometryBuffer;
            var startVertex = geometryBuffer.VertexCount;
            var startIndex = geometryBuffer.IndexCount;
            geometryBuffer.EnqueueSprite(startVertex, textureRegion, ref transformMatrix, color, origin, spriteOptions, depth);
            var indexCount = geometryBuffer.IndexCount - startIndex;
            var drawContext = new DrawContext2D(textureRegion.Texture);
            EnqueueDraw(startIndex, indexCount, ref drawContext, sortKey);
        }

        /// <summary>
        ///     Defines a drawing context for two-dimensional geometry.
        /// </summary>
        /// <seealso cref="IDrawContext{TData}" />
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DrawContext2D : IDrawContext<DrawContext2D>
        {
            public Texture2D Texture;

            internal DrawContext2D(Texture2D texture)
            {
                Texture = texture;
            }

            public bool Equals(ref DrawContext2D other)
            {
                return other.Texture == Texture;
            }

            public void ApplyPass(Effect effect, int passIndex, EffectPass pass)
            {
                pass.Apply();
                effect.GraphicsDevice.Textures[index: 0] = Texture;
            }
        }
    }
}
