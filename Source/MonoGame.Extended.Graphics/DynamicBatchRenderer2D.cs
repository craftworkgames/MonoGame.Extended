using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Graphics.Geometry;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Enables a group of dynamic two-dimensional geometric objects be batched together for rendering, if possible, using
    ///     the same settings.
    /// </summary>
    /// <seealso cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" />
    public class DynamicBatchRenderer2D : BatchRenderer<VertexPositionColorTexture, ushort, DynamicBatchRenderer2D.DrawCommandData>
    {
        internal const int DefaultMaximumVerticesCount = 8192;
        internal const int DefaultMaximumIndicesCount = 12288;
        private readonly DefaultEffect2D _defaultEffect;
        private readonly Texture2D _pixelTexture;
        private BlendState _blendState;
        private DepthStencilState _depthStencilState;
        private Effect _effect;

        private DrawCommandData _pixelTextureDrawCommandData;
        private Matrix? _projectionMatrix;
        private RasterizerState _rasterizerState;
        private SamplerState _samplerState;
        private Batch2DSortMode _sortMode;
        private Matrix _viewMatrix;
        private Matrix _worldMatrix;

        private readonly SpriteBuilderPositionColorTextureU16 _spriteBuilder;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicBatchRenderer2D" /> class.
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
        public DynamicBatchRenderer2D(GraphicsDevice graphicsDevice, ushort maximumVerticesCount = DefaultMaximumVerticesCount,
            ushort maximumIndicesCount = DefaultMaximumIndicesCount,
            int maximumBatchCommandsCount = DefaultMaximumBatchCommandsCount)
            : base(
                graphicsDevice,
                new DynamicVertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration,
                    maximumVerticesCount, BufferUsage.WriteOnly),
                new DynamicIndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, maximumIndicesCount,
                    BufferUsage.WriteOnly),
                new GraphicsGeometryData<VertexPositionColorTexture, ushort>(maximumVerticesCount, maximumIndicesCount),
                maximumBatchCommandsCount)
        {
            _defaultEffect = new DefaultEffect2D(graphicsDevice);

            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[]
            {
                Color.White
            });

            _pixelTextureDrawCommandData = new DrawCommandData
            {
                Texture = _pixelTexture
            };

            _spriteBuilder = new SpriteBuilderPositionColorTextureU16();
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

            _pixelTextureDrawCommandData.Texture = null;
            _pixelTexture?.Dispose();
        }

        /// <summary>
        ///     Starts a group of two-dimensional geometry for rendering with the specified <see cref="BatchSortMode" />,
        ///     <see cref="Effect" /> and the optional chain of <see cref="Matrix" />es for transforming between world, view, and
        ///     projection spaces.
        /// </summary>
        /// <param name="sortMode">The <see cref="Batch2DSortMode" />. Default value is <see cref="Batch2DSortMode.Deferred" />.</param>
        /// <param name="blendState">
        ///     The <see cref="BlendState" />. Use <code>null</code> to use the default
        ///     <see cref="BlendState.AlphaBlend" />.
        /// </param>
        /// <param name="samplerState">
        ///     The <see cref="SamplerState" />. Use <code>null</code> to use the default
        ///     <see cref="SamplerState.LinearClamp" />.
        /// </param>
        /// <param name="depthStencilState">
        ///     The <see cref="DepthStencilState" />. Use <code>null</code> to use the default
        ///     <see cref="DepthStencilState.None" />.
        /// </param>
        /// <param name="rasterizerState">
        ///     The <see cref="RasterizerState" />. Use <code>null</code> to use the default
        ///     <see cref="RasterizerState.CullCounterClockwise" />.
        /// </param>
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
        public void Begin(Batch2DSortMode sortMode = Batch2DSortMode.Deferred, BlendState blendState = null,
            SamplerState samplerState = null, DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null, Effect effect = null,
            Matrix? worldMatrix = null,
            Matrix? viewMatrix = null, Matrix? projectionMatrix = null)
        {
            _blendState = blendState ?? BlendState.AlphaBlend;
            _samplerState = samplerState ?? SamplerState.LinearClamp;
            _depthStencilState = depthStencilState ?? DepthStencilState.None;
            _rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
            _effect = effect ?? _defaultEffect;
            _worldMatrix = worldMatrix ?? Matrix.Identity;
            _viewMatrix = viewMatrix ?? Matrix.Identity;
            _projectionMatrix = projectionMatrix;

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
                    ApplyStates();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortMode), sortMode, null);
            }

            _sortMode = sortMode;

            Begin(_effect, batchSortMode);
        }

        /// <summary>
        ///     Ends and submits the group of geometry to the
        ///     <see cref="P:MonoGame.Extended.Graphics.Batching.BatchRenderer`2.GraphicsDevice" /> for rendering.
        /// </summary>
        /// <remarks>
        ///     This method must be called after all enqueuing of draw calls.
        /// </remarks>
        public override void End()
        {
            if (_sortMode != Batch2DSortMode.Immediate)
                ApplyStates();

            base.End();
            GeometryData.VerticesCount = 0;
            GeometryData.IndicesCount = 0;
        }

        protected override void Flush()
        {
            base.Flush();
            GeometryData.VerticesCount = 0;
            GeometryData.IndicesCount = 0;
        }

        private void ApplyStates()
        {
            var graphicsDevice = GraphicsDevice;
            graphicsDevice.BlendState = _blendState;
            graphicsDevice.SamplerStates[0] = _samplerState;
            graphicsDevice.DepthStencilState = _depthStencilState;
            graphicsDevice.RasterizerState = _rasterizerState;

            var matrixChainEffect = _effect as IMatrixChainEffect;
            if (matrixChainEffect == null)
                return;

            matrixChainEffect.SetWorld(ref _worldMatrix);
            matrixChainEffect.SetView(ref _viewMatrix);

            if (_projectionMatrix.HasValue)
            {
                matrixChainEffect.Projection = _projectionMatrix.Value;
            }
            else
            {
                var viewport = GraphicsDevice.Viewport;
                var projectionOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
                var projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1);
                Matrix.Multiply(ref projectionOffset, ref projection, out projection);
                matrixChainEffect.SetProjection(ref projection);
            }
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
        ///     Draws a sprite using a specified <see cref="Texture" />, transform <see cref="Matrix2D" /> and an optional
        ///     source <see cref="Rectangle" />, <see cref="Color" />, origin <see cref="Vector2" />,
        ///     <see cref="SpriteEffects" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="destinationRectangle">
        ///     The destination <see cref="Rectangle" /> that specifies the world destination for
        ///     drawing the sprite. If this rectangle is not the same size as the <paramref name="sourceRectangle" />, the sprite
        ///     will be scaled to fit.
        /// </param>
        /// <param name="sourceRectangle">
        ///     The texture region <see cref="Rectangle" /> of the <paramref name="texture" />. Use
        ///     <code>null</code> to use the entire <see cref="Texture2D" />.
        /// </param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="rotation">
        ///     The angle <see cref="float" /> (in radians) to rotate the sprite about its <paramref name="origin" />. The default
        ///     value is <code>0f</code>.
        /// </param>
        /// <param name="origin">
        ///     The origin <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.Zero" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        [Obsolete("The Draw method is deprecated, please use the DrawSprite method instead.")]
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle = null,
            Color? color = null, float rotation = 0f, Vector2? origin = null, SpriteEffects effects = SpriteEffects.None,
            float depth = 0)
        {
            DrawSprite(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, depth);
        }

        /// <summary>
        ///     Draws a sprite using a specified <see cref="Texture" />, transform <see cref="Matrix2D" /> and an optional
        ///     source <see cref="Rectangle" />, <see cref="Color" />, origin <see cref="Vector2" />,
        ///     <see cref="SpriteEffects" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="destinationRectangle">
        ///     The destination <see cref="Rectangle" /> that specifies the world destination for
        ///     drawing the sprite. If this rectangle is not the same size as the <paramref name="sourceRectangle" />, the sprite
        ///     will be scaled to fit.
        /// </param>
        /// <param name="sourceRectangle">
        ///     The texture region <see cref="Rectangle" /> of the <paramref name="texture" />. Use
        ///     <code>null</code> to use the entire <see cref="Texture2D" />.
        /// </param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="rotation">
        ///     The angle <see cref="float" /> (in radians) to rotate the sprite about its <paramref name="origin" />. The default
        ///     value is <code>0f</code>.
        /// </param>
        /// <param name="origin">
        ///     The origin <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.Zero" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        public void DrawSprite(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle = null,
            Color? color = null, float rotation = 0f, Vector2? origin = null, SpriteEffects effects = SpriteEffects.None,
            float depth = 0)
        {
            var position = new Vector2(destinationRectangle.X, destinationRectangle.Y);
            var size = new Size(destinationRectangle.Width, destinationRectangle.Height);

            Matrix2D transformMatrix;
            Matrix2D.CreateFrom(position, rotation, null, origin, out transformMatrix);

            _spriteBuilder.BuildSprite(ref transformMatrix, texture, sourceRectangle, color, size, effects, depth);

            var commandData = new DrawCommandData(texture);
            DrawGeometry(_spriteBuilder, ref commandData, depth);
        }

        /// <summary>
        ///     Draws a sprite using a specified <see cref="Texture" />, transform <see cref="Matrix2D" /> and an optional
        ///     source <see cref="Rectangle" />, <see cref="Color" />, origin <see cref="Vector2" />,
        ///     <see cref="SpriteEffects" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix2D" />.</param>
        /// <param name="sourceRectangle">
        ///     The texture region <see cref="Rectangle" /> of the <paramref name="texture" />. Use
        ///     <code>null</code> to use the entire <see cref="Texture2D" />.
        /// </param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        public void DrawSprite(Texture2D texture, ref Matrix2D transformMatrix, Rectangle? sourceRectangle = null,
            Color? color = null, SpriteEffects effects = SpriteEffects.None, float depth = 0)
        {
            _spriteBuilder.BuildSprite(ref transformMatrix, texture, sourceRectangle, color, null, effects, depth);
            var commandData = new DrawCommandData(texture);
            DrawGeometry(_spriteBuilder, ref commandData, depth);
        }

        private unsafe void DrawGeometry(GraphicsGeometryBuilder<VertexPositionColorTexture, ushort> geometryBuilder, ref DrawCommandData drawCommandData, float depth)
        {
            if (GeometryData.VerticesCount + geometryBuilder.Vertices.Length > GeometryData.Vertices.Length || GeometryData.IndicesCount + geometryBuilder.Indices.Length > GeometryData.Indices.Length)
            {
                Flush();
            }

            var startVertex = GeometryData.VerticesCount;
            var startIndex = GeometryData.IndicesCount;
            var indexOffset = (ushort)startVertex;

            Array.Copy(geometryBuilder.Vertices, 0, GeometryData.Vertices, startVertex, geometryBuilder.Vertices.Length);

            fixed (ushort* fixedDestinationPointer = GeometryData.Indices)
            fixed (ushort* fixedSourcePointer = geometryBuilder.Indices)
            {
                var pointerDestination = fixedDestinationPointer + startIndex;
                var pointerDestinationEnd = pointerDestination + geometryBuilder.Indices.Length;
                var pointerSource = fixedSourcePointer;

                while (pointerDestination < pointerDestinationEnd)
                {
                    *pointerDestination++ = (ushort)(*pointerSource++ + indexOffset);
                }
            }

            GeometryData.VerticesCount += geometryBuilder.Vertices.Length;
            GeometryData.IndicesCount += geometryBuilder.Indices.Length;

            var sortKey = GetSortKey(depth);
            Draw(geometryBuilder.PrimitiveType, startIndex, geometryBuilder.PrimitivesCount, sortKey, ref drawCommandData);
        }

        /// <summary>
        ///     Draws a sprite using a specified <see cref="Texture" />, position <see cref="Vector2" /> and an optional source
        ///     <see cref="Rectangle" />, <see cref="Color" />, rotation <see cref="float" />, origin <see cref="Vector2" />, scale
        ///     <see cref="Vector2" />, <see cref="SpriteEffects" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="position">The world position <see cref="Vector2" />.</param>
        /// <param name="sourceRectangle">
        ///     The texture region <see cref="Rectangle" /> of the <paramref name="texture" />. Use
        ///     <code>null</code> to use the entire <see cref="Texture2D" />.
        /// </param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="rotation">
        ///     The angle <see cref="float" /> (in radians) to rotate the sprite about its <paramref name="origin" />. The default
        ///     value is <code>0f</code>.
        /// </param>
        /// <param name="origin">
        ///     The origin <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.Zero" />.
        /// </param>
        /// <param name="scale">
        ///     The scale <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.One" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null,
            Color? color = null, float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
            SpriteEffects effects = SpriteEffects.None,
            float depth = 0)
        {
            Matrix2D transformMatrix;
            Matrix2D.CreateFrom(position, rotation, scale, origin, out transformMatrix);
            DrawSprite(texture, ref transformMatrix, sourceRectangle, color, effects, depth);
        }

        /// <summary>
        ///     Draws a sprite using a specified <see cref="Texture" />, position <see cref="Vector2" /> and an optional source
        ///     <see cref="Rectangle" />, <see cref="Color" />, rotation <see cref="float" />, origin <see cref="Vector2" />, scale
        ///     <see cref="Vector2" />, <see cref="SpriteEffects" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="position">The world position <see cref="Vector2" />.</param>
        /// <param name="sourceRectangle">
        ///     The texture region <see cref="Rectangle" /> of the <paramref name="texture" />. Use
        ///     <code>null</code> to use the entire <see cref="Texture2D" />.
        /// </param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="rotation">
        ///     The angle <see cref="float" /> (in radians) to rotate the sprite about its <paramref name="origin" />. The default
        ///     value is <code>0f</code>.
        /// </param>
        /// <param name="origin">
        ///     The origin <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.Zero" />.
        /// </param>
        /// <param name="scale">
        ///     The scale <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.One" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        [Obsolete("The Draw method is deprecated, please use the DrawSprite method instead.")]
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null,
            Color? color = null, float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
            SpriteEffects effects = SpriteEffects.None,
            float depth = 0)
        {
            DrawSprite(texture, position, sourceRectangle, color, rotation, origin, scale, effects, depth);
        }

        /// <summary>
        ///     Draws unicode (UTF-16) characters as sprites using the specified <see cref="BitmapFont" />, text
        ///     <see cref="StringBuilder" />, transform <see cref="Matrix2D" /> and optional <see cref="Color" />, origin
        ///     <see cref="Vector2" />, <see cref="SpriteEffects" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="bitmapFont">The <see cref="BitmapFont" />.</param>
        /// <param name="text">The text <see cref="StringBuilder" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix2D" />.</param>
        /// <param name="color">
        ///     The <see cref="Color" />. Use <code>null</code> to use the default
        ///     <see cref="Color.White" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, StringBuilder text, ref Matrix2D transformMatrix,
            Color? color = null, SpriteEffects effects = SpriteEffects.None, float depth = 0f)
        {
            EnsureHasBegun();

            if (bitmapFont == null)
                throw new ArgumentNullException(nameof(bitmapFont));

            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var lineSpacing = bitmapFont.LineHeight;
            var offset = new Vector2(0, 0);

            for (var i = 0; i < text.Length;)
            {
                int character;
                if (char.IsLowSurrogate(text[i]))
                {
                    character = char.ConvertToUtf32(text[i - 1], text[i]);
                    i += 2;
                }
                else if (char.IsHighSurrogate(text[i]))
                {
                    character = char.ConvertToUtf32(text[i], text[i - 1]);
                    i += 2;
                }
                else
                {
                    character = text[i];
                    i += 1;
                }

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (character)
                {
                    case '\r':
                        continue;
                    case '\n':
                        offset.X = 0;
                        offset.Y += lineSpacing;
                        continue;
                }

                var fontRegion = bitmapFont.GetCharacterRegion(character);
                if (fontRegion == null)
                    continue;

                var transform1Matrix = transformMatrix;
                transform1Matrix.M31 += offset.X + fontRegion.XOffset;
                transform1Matrix.M32 += offset.Y + fontRegion.YOffset;

                var textureRegion = fontRegion.TextureRegion;
                var bounds = textureRegion.Bounds;
                DrawSprite(textureRegion.Texture, ref transform1Matrix, bounds, color, effects, depth);

                offset.X += i != text.Length - 1
                    ? fontRegion.XAdvance + bitmapFont.LetterSpacing
                    : fontRegion.XOffset + fontRegion.Width;
            }
        }

        /// <summary>
        ///     Draws unicode (UTF-16) characters as sprites using the specified <see cref="BitmapFont" />, text
        ///     <see cref="StringBuilder" />, position <see cref="Vector2" /> and optional <see cref="Color" />, rotation
        ///     <see cref="float" />, origin <see cref="Vector2" />, scale <see cref="Vector2" /> <see cref="SpriteEffects" />, and
        ///     depth <see cref="float" />.
        /// </summary>
        /// <param name="bitmapFont">The <see cref="BitmapFont" />.</param>
        /// <param name="text">The text <see cref="string" />.</param>
        /// <param name="position">The position <see cref="Vector2" />.</param>
        /// <param name="color">
        ///     The <see cref="Color" />. Use <code>null</code> to use the default
        ///     <see cref="Color.White" />.
        /// </param>
        /// <param name="rotation">
        ///     The angle <see cref="float" /> (in radians) to rotate each sprite about its <paramref name="origin" />. The default
        ///     value is <code>0f</code>.
        /// </param>
        /// <param name="origin">
        ///     The origin <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.Zero" />.
        /// </param>
        /// <param name="scale">
        ///     The scale <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.One" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code></param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, StringBuilder text, Vector2 position, Color? color = null,
            float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
            SpriteEffects effects = SpriteEffects.None, float depth = 0f)
        {
            Matrix2D transformMatrix;
            Matrix2D.CreateFrom(position, rotation, scale, origin, out transformMatrix);
            DrawString(bitmapFont, text, ref transformMatrix, color, effects, depth);
        }

        /// <summary>
        ///     Draws unicode (UTF-16) characters as sprites using the specified <see cref="BitmapFont" />, text
        ///     <see cref="string" />, transform <see cref="Matrix2D" /> and optional <see cref="Color" />, origin
        ///     <see cref="Vector2" />, <see cref="SpriteEffects" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="bitmapFont">The <see cref="BitmapFont" />.</param>
        /// <param name="text">The text <see cref="string" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix2D" />.</param>
        /// <param name="color">
        ///     The <see cref="Color" />. Use <code>null</code> to use the default
        ///     <see cref="Color.White" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code></param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, string text, ref Matrix2D transformMatrix, Color? color = null, SpriteEffects effects = SpriteEffects.None, float depth = 0f)
        {
            EnsureHasBegun();

            if (bitmapFont == null)
                throw new ArgumentNullException(nameof(bitmapFont));

            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var lineSpacing = bitmapFont.LineHeight;
            var offset = new Vector2(0, 0);

            for (var i = 0; i < text.Length;)
            {
                int character;
                if (char.IsLowSurrogate(text[i]))
                {
                    character = char.ConvertToUtf32(text[i - 1], text[i]);
                    i += 2;
                }
                else if (char.IsHighSurrogate(text[i]))
                {
                    character = char.ConvertToUtf32(text[i], text[i - 1]);
                    i += 2;
                }
                else
                {
                    character = text[i];
                    i += 1;
                }

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (character)
                {
                    case '\r':
                        continue;
                    case '\n':
                        offset.X = 0;
                        offset.Y += lineSpacing;
                        continue;
                }

                var fontRegion = bitmapFont.GetCharacterRegion(character);
                if (fontRegion == null)
                    continue;

                var transform1Matrix = transformMatrix;
                transform1Matrix.M31 += offset.X + fontRegion.XOffset;
                transform1Matrix.M32 += offset.Y + fontRegion.YOffset;

                var textureRegion = fontRegion.TextureRegion;
                var bounds = textureRegion.Bounds;
                DrawSprite(textureRegion.Texture, ref transform1Matrix, bounds, color, effects, depth);

                offset.X += i != text.Length - 1
                    ? fontRegion.XAdvance + bitmapFont.LetterSpacing
                    : fontRegion.XOffset + fontRegion.Width;
            }
        }

        /// <summary>
        ///     Draws unicode (UTF-16) characters as sprites using the specified <see cref="BitmapFont" />, text
        ///     <see cref="string" />, position <see cref="Vector2" /> and optional <see cref="Color" />, rotation
        ///     <see cref="float" />, origin <see cref="Vector2" />, scale <see cref="Vector2" /> <see cref="SpriteEffects" />, and
        ///     depth <see cref="float" />.
        /// </summary>
        /// <param name="bitmapFont">The <see cref="BitmapFont" />.</param>
        /// <param name="text">The text <see cref="string" />.</param>
        /// <param name="position">The position <see cref="Vector2" />.</param>
        /// <param name="color">
        ///     The <see cref="Color" />. Use <code>null</code> to use the default
        ///     <see cref="Color.White" />.
        /// </param>
        /// <param name="rotation">
        ///     The angle <see cref="float" /> (in radians) to rotate each sprite about its <paramref name="origin" />. The default
        ///     value is <code>0f</code>.
        /// </param>
        /// <param name="origin">
        ///     The origin <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.Zero" />.
        /// </param>
        /// <param name="scale">
        ///     The scale <see cref="Vector2" />. Use <code>null</code> to use the default
        ///     <see cref="Vector2.One" />.
        /// </param>
        /// <param name="effects">The <see cref="SpriteEffects" />. The default value is <see cref="SpriteEffects.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code></param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, string text, Vector2 position, Color? color = null,
            float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
            SpriteEffects effects = SpriteEffects.None, float depth = 0f)
        {
            Matrix2D matrix;
            Matrix2D.CreateFrom(position, rotation, scale, origin, out matrix);
            DrawString(bitmapFont, text, ref matrix, color, effects, depth);
        }

        /// <summary>
        ///     Defines a drawing context for two-dimensional geometric objects.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [EditorBrowsable(EditorBrowsableState.Never)]
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