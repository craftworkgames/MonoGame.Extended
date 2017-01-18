using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Graphics.Geometry;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Enables a group of dynamic two-dimensional geometric objects be batched together for rendering, if possible, using
    ///     the same settings.
    /// </summary>
    /// <seealso cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" />
    public class DynamicBatchRenderer2D : DynamicBatchRenderer<VertexPositionColorTexture, ushort, DynamicBatchRenderer2D.DrawCallData>
    {
        internal const int DefaultMaximumVerticesCount = 8192;
        internal const int DefaultMaximumIndicesCount = 12288;

        private readonly SpriteBuilderPositionColorTextureU16 _spriteBuilder;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicBatchRenderer2D" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="maximumVerticesCount">The maximum number of vertices. The default value is <code>8192</code>.</param>
        /// <param name="maximumIndicesCount">The maximum number of indices. The default value is <code>12288</code>.</param>
        /// <param name="maximumDrawCallsCount">
        ///     The maximum number of draw calls that can be deferred before they have to be
        ///     submitted to the <see cref="GraphicsDevice" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumDrawCallsCount" /> is less than or equal
        ///     <code>0</code>, or <paramref name="maximumVerticesCount" /> is less than or equal to <code>0</code>, or,
        ///     <paramref name="maximumVerticesCount" /> is less than or equal to <code>0</code>.
        /// </exception>
        public DynamicBatchRenderer2D(GraphicsDevice graphicsDevice, ushort maximumVerticesCount = DefaultMaximumVerticesCount,
            ushort maximumIndicesCount = DefaultMaximumIndicesCount,
            int maximumDrawCallsCount = DefaultMaximumDrawCallsCount)
            : base(
                graphicsDevice, 
                new DefaultEffect2D(graphicsDevice), 
                new GraphicsGeometryBufferPositionColorTextureU16(graphicsDevice, maximumVerticesCount, maximumIndicesCount, true),
                maximumDrawCallsCount)
        {
            _spriteBuilder = new SpriteBuilderPositionColorTextureU16();
        }

        /// <summary>
        ///     Draws a sprite using a specified <see cref="Texture" />, transform <see cref="Matrix2D" />, source <see cref="Rectangle"/>, and an optional
        ///     <see cref="Color" />, origin <see cref="Vector2" />, <see cref="FlipFlags" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix2D" />.</param>
        /// <param name="sourceRectangle">
        ///     The texture region <see cref="Rectangle" /> of the <paramref name="texture" />. Use
        ///     <code>null</code> to use the entire <see cref="Texture2D" />.
        /// </param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="flags">The <see cref="FlipFlags" />. The default value is <see cref="FlipFlags.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Renderer.Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        public void DrawSprite(Texture2D texture, ref Matrix2D transformMatrix, ref Rectangle sourceRectangle,
            Color? color = null, FlipFlags flags = FlipFlags.None, float depth = 0)
        {
            _spriteBuilder.BuildSprite(ref transformMatrix, texture, ref sourceRectangle, color, flags, depth);
            var commandData = new DrawCallData(texture);
            DrawGeometry(_spriteBuilder, ref commandData, depth);
        }

        /// <summary>
        ///     Draws a <see cref="Texture" /> using the specified transform <see cref="Matrix2D" /> and an optional
        ///     <see cref="Color" />, origin <see cref="Vector2" />, <see cref="FlipFlags" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix2D" />.</param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="flags">The <see cref="FlipFlags" />. The default value is <see cref="FlipFlags.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Renderer.Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        public void DrawTexture(Texture2D texture, ref Matrix2D transformMatrix, Color? color = null, FlipFlags flags = FlipFlags.None, float depth = 0)
        {
            _spriteBuilder.BuildSprite(ref transformMatrix, texture, color, flags, depth);
            var commandData = new DrawCallData(texture);
            DrawGeometry(_spriteBuilder, ref commandData, depth);
        }

        private void DrawGeometry(GraphicsGeometryBuilder<VertexPositionColorTexture, ushort> geometryBuilder, ref DrawCallData drawCallData, float depth)
        {
            if (GeometryBuffer.VerticesCount + geometryBuilder.Vertices.Length > GeometryBuffer.Vertices.Length || GeometryBuffer.IndicesCount + geometryBuilder.Indices.Length > GeometryBuffer.Indices.Length)
            {
                Flush();
            }

            var startVertex = GeometryBuffer.VerticesCount;
            var indexOffset = startVertex;

            Array.Copy(geometryBuilder.Vertices, 0, GeometryBuffer.Vertices, startVertex, geometryBuilder.Vertices.Length);

            GeometryBuffer.VerticesCount += geometryBuilder.Vertices.Length;

            var startIndex = GeometryBuffer.EnqueueIndicesFrom(geometryBuilder.Indices, 0, geometryBuilder.Indices.Length, indexOffset);

            ulong key;
            GenerateDrawCallKey(drawCallData.TextureKey, depth, out key);
                
            var drawCall = new DrawCall<DrawCallData>
            {
                PrimitiveType = geometryBuilder.PrimitiveType,
                Key = key,
                BaseVertex = 0,
                StartIndex = startIndex,
                PrimitiveCount = geometryBuilder.PrimitivesCount,
                Data = drawCallData
            };

            if (TryEnqueueDrawCall(ref drawCall))
                return;
            Flush();
            TryEnqueueDrawCall(ref drawCall);
        }

        private static unsafe void GenerateDrawCallKey(int textureKey, float depth, out ulong key)
        {
            var depthKey = *(uint*)&depth;

            const ulong textureMask = 0x00000000ffffffffUL;
            const ulong depthMask = 0xffffffff00000000UL;

            // the key is used to group draw calls to minimize state changes
            // for general purposes we want to group draw calls by depth then by material
            // a more advanced renderer would consider wether the geometry is opaque and if so group by texture first instead            
            key = depthKey & depthMask | (uint)textureKey & textureMask;
        }

        /// <summary>
        ///     Draws unicode (UTF-16) characters as sprites using the specified <see cref="BitmapFont" />, text
        ///     <see cref="StringBuilder" />, transform <see cref="Matrix2D" /> and optional <see cref="Color" />, origin
        ///     <see cref="Vector2" />, <see cref="FlipFlags" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="bitmapFont">The <see cref="BitmapFont" />.</param>
        /// <param name="text">The text <see cref="StringBuilder" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix2D" />.</param>
        /// <param name="color">
        ///     The <see cref="Color" />. Use <code>null</code> to use the default
        ///     <see cref="Color.White" />.
        /// </param>
        /// <param name="flags">The <see cref="FlipFlags" />. The default value is <see cref="FlipFlags.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Renderer.Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, StringBuilder text, ref Matrix2D transformMatrix,
            Color? color = null, FlipFlags flags = FlipFlags.None, float depth = 0f)
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
                DrawSprite(textureRegion.Texture, ref transform1Matrix, ref bounds, color, flags, depth);

                offset.X += i != text.Length - 1
                    ? fontRegion.XAdvance + bitmapFont.LetterSpacing
                    : fontRegion.XOffset + fontRegion.Width;
            }
        }

        /// <summary>
        ///     Draws unicode (UTF-16) characters as sprites using the specified <see cref="BitmapFont" />, text
        ///     <see cref="StringBuilder" />, position <see cref="Vector2" /> and optional <see cref="Color" />, rotation
        ///     <see cref="float" />, origin <see cref="Vector2" />, scale <see cref="Vector2" /> <see cref="FlipFlags" />, and
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
        /// <param name="flags">The <see cref="FlipFlags" />. The default value is <see cref="FlipFlags.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code></param>
        /// <exception cref="InvalidOperationException">The <see cref="Renderer.Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, StringBuilder text, Vector2 position, Color? color = null,
            float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
            FlipFlags flags = FlipFlags.None, float depth = 0f)
        {
            Matrix2D transformMatrix;
            Matrix2D.CreateFrom(position, rotation, scale, origin, out transformMatrix);
            DrawString(bitmapFont, text, ref transformMatrix, color, flags, depth);
        }

        /// <summary>
        ///     Draws unicode (UTF-16) characters as sprites using the specified <see cref="BitmapFont" />, text
        ///     <see cref="string" />, transform <see cref="Matrix2D" /> and optional <see cref="Color" />, origin
        ///     <see cref="Vector2" />, <see cref="FlipFlags" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="bitmapFont">The <see cref="BitmapFont" />.</param>
        /// <param name="text">The text <see cref="string" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix2D" />.</param>
        /// <param name="color">
        ///     The <see cref="Color" />. Use <code>null</code> to use the default
        ///     <see cref="Color.White" />.
        /// </param>
        /// <param name="flags">The <see cref="FlipFlags" />. The default value is <see cref="FlipFlags.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code></param>
        /// <exception cref="InvalidOperationException">The <see cref="Renderer.Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, string text, ref Matrix2D transformMatrix, Color? color = null, FlipFlags flags = FlipFlags.None, float depth = 0f)
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
                DrawSprite(textureRegion.Texture, ref transform1Matrix, ref bounds, color, flags, depth);

                offset.X += i != text.Length - 1
                    ? fontRegion.XAdvance + bitmapFont.LetterSpacing
                    : fontRegion.XOffset + fontRegion.Width;
            }
        }

        /// <summary>
        ///     Draws unicode (UTF-16) characters as sprites using the specified <see cref="BitmapFont" />, text
        ///     <see cref="string" />, position <see cref="Vector2" /> and optional <see cref="Color" />, rotation
        ///     <see cref="float" />, origin <see cref="Vector2" />, scale <see cref="Vector2" /> <see cref="FlipFlags" />, and
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
        /// <param name="flags">The <see cref="FlipFlags" />. The default value is <see cref="FlipFlags.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code></param>
        /// <exception cref="InvalidOperationException">The <see cref="Renderer.Begin" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, string text, Vector2 position, Color? color = null,
            float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
            FlipFlags flags = FlipFlags.None, float depth = 0f)
        {
            Matrix2D matrix;
            Matrix2D.CreateFrom(position, rotation, scale, origin, out matrix);
            DrawString(bitmapFont, text, ref matrix, color, flags, depth);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct DrawCallData : IDrawCallData
        {
            public Texture2D Texture;
            public int TextureKey;

            internal DrawCallData(Texture2D texture)
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

            public bool Equals(ref DrawCallData other)
            {
                return Texture == other.Texture;
            }

            public int CompareTo(DrawCallData other)
            {
                return TextureKey.CompareTo(other.TextureKey);
            }
        }
    }
}