using System;
using System.Text;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Graphics.Geometry;
using MonoGame.Extended.BitmapFonts;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     A general purpose <see cref="Batcher{TDrawCallInfo}" /> for two-dimensional geometry that change
    ///     frequently between frames such as sprites and shapes.
    /// </summary>
    /// <seealso cref="IDisposable" />
    /// <remarks>
    ///     <para>For drawing user interfaces, consider using <see cref="UIBatcher(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> instead because it supports scissor rectangles.</para>
    /// </remarks>
    public sealed class Batcher2D : Batcher<Batcher2D.DrawCallInfo>
    {

        internal const int DefaultMaximumVerticesCount = 8192;
        internal const int DefaultMaximumIndicesCount = 12288;

        private readonly VertexBuffer _vertexBuffer;
        private readonly IndexBuffer _indexBuffer;
        private readonly VertexPositionColorTexture[] _vertices;
        private int _vertexCount;
        private readonly ushort[] _indices;
        private int _indexCount;
        private readonly ushort[] _sortedIndices;
        private readonly GeometryBuilder2D _geometryBuilder;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Batcher2D" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="maximumVerticesCount">
        ///     The maximum number of vertices that can be enqueued before a
        ///     <see cref="Batcher{TDrawCallInfo}.Flush" /> is required. The default value is <code>8192</code>.
        /// </param>
        /// <param name="maximumIndicesCount">
        ///     The maximum number of indices that can be enqueued before a
        ///     <see cref="Batcher{TDrawCallInfo}.Flush" /> is required. The default value is <code>12288</code>.
        /// </param>
        /// <param name="maximumDrawCallsCount">
        ///     The maximum number of <see cref="DrawCallInfo" /> structs that can be enqueued before a
        ///     <see cref="Batcher{TDrawCallInfo}.Flush" /> is required. The default value is <code>2048</code>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumDrawCallsCount" /> is less than or equal
        ///     <code>0</code>, or <paramref name="maximumVerticesCount" /> is less than or equal to <code>0</code>, or,
        ///     <paramref name="maximumVerticesCount" /> is less than or equal to <code>0</code>.
        /// </exception>
        public Batcher2D(GraphicsDevice graphicsDevice,
            ushort maximumVerticesCount = DefaultMaximumVerticesCount,
            ushort maximumIndicesCount = DefaultMaximumIndicesCount,
            int maximumDrawCallsCount = DefaultBatchMaximumDrawCallsCount)
            : base(
                graphicsDevice,
                new DefaultEffect(graphicsDevice)
                {
                    TextureEnabled =  true,
                    VertexColorEnabled = true
                }, maximumDrawCallsCount)

        {
            _vertices = new VertexPositionColorTexture[maximumVerticesCount];
            _vertexBuffer = new DynamicVertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration, maximumVerticesCount, BufferUsage.WriteOnly);

            _indices = new ushort[maximumIndicesCount];
            _sortedIndices = new ushort[maximumIndicesCount];
            _indexBuffer = new DynamicIndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, maximumIndicesCount, BufferUsage.WriteOnly);
            _geometryBuilder = new GeometryBuilder2D(4, 6);
        }

        protected override void SortDrawCallsAndBindBuffers()
        {
            // Upload the vertices to the GPU and then select that vertex stream for drawing
            _vertexBuffer.SetData(_vertices, 0, _vertexCount);
            GraphicsDevice.SetVertexBuffer(_vertexBuffer);

            Array.Sort(DrawCalls, 0, EnqueuedDrawCallCount);
            BuildSortedIndices();

            // Upload the indices to the GPU and then select that index stream for drawing
            _indexBuffer.SetData(_sortedIndices, 0, _indexCount);
            GraphicsDevice.Indices = _indexBuffer;

            _indexCount = 0;
            _vertexCount = 0;
        }

        private void BuildSortedIndices()
        {
            var newDrawCallsCount = 0;
            DrawCalls[0].StartIndex = 0;
            var currentDrawCall = DrawCalls[0];
            DrawCalls[newDrawCallsCount++] = DrawCalls[0];

            var drawCallIndexCount = currentDrawCall.PrimitiveCount * 3;
            Array.Copy(_indices, currentDrawCall.StartIndex, _sortedIndices, 0, drawCallIndexCount);
            var sortedIndexCount = drawCallIndexCount;

            // iterate through sorted draw calls checking if any can now be merged to reduce expensive draw calls to the graphics API
            // this might need to be changed for next-gen graphics API (Vulkan, Metal, DirectX 12) where the draw calls are not so expensive
            for (var i = 1; i < EnqueuedDrawCallCount; i++)
            {
                currentDrawCall = DrawCalls[i];
                drawCallIndexCount = currentDrawCall.PrimitiveCount * 3;
                Array.Copy(_indices, currentDrawCall.StartIndex, _sortedIndices, sortedIndexCount, drawCallIndexCount);
                sortedIndexCount += drawCallIndexCount;

                if (currentDrawCall.TryMerge(ref DrawCalls[newDrawCallsCount - 1]))
                    continue;

                currentDrawCall.StartIndex = sortedIndexCount;
                DrawCalls[newDrawCallsCount++] = currentDrawCall;
            }

            EnqueuedDrawCallCount = newDrawCallsCount;
        }

        /// <summary>
        ///     Submits a draw operation to the <see cref="GraphicsDevice" /> using the specified <see cref="DrawCallInfo"/>.
        /// </summary>
        /// <param name="drawCall">The draw call information.</param>
        protected override void InvokeDrawCall(ref DrawCallInfo drawCall)
        {
            GraphicsDevice.DrawIndexedPrimitives(drawCall.PrimitiveType, 0, drawCall.StartIndex, drawCall.PrimitiveCount);
        }

        /// <summary>
        ///     Draws a sprite using a specified <see cref="Texture" />, transform <see cref="Matrix3x2" />, source
        ///     <see cref="Rectangle" />, and an optional
        ///     <see cref="Color" />, origin <see cref="Vector2" />, <see cref="FlipFlags" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix3x2" />.</param>
        /// <param name="sourceRectangle">
        ///     The texture region <see cref="Rectangle" /> of the <paramref name="texture" />. Use
        ///     <code>null</code> to use the entire <see cref="Texture2D" />.
        /// </param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="flags">The <see cref="FlipFlags" />. The default value is <see cref="FlipFlags.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Batcher{TDrawCallInfo}.Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        public void DrawSprite(Texture2D texture, ref Matrix3x2 transformMatrix, ref Rectangle sourceRectangle,
            Color? color = null, FlipFlags flags = FlipFlags.None, float depth = 0)
        {
            _geometryBuilder.BuildSprite(_vertexCount, ref transformMatrix, texture, ref sourceRectangle, color, flags, depth);
            EnqueueBuiltGeometry(texture, depth);
        }

        /// <summary>
        ///     Draws a <see cref="Texture" /> using the specified transform <see cref="Matrix3x2" /> and an optional
        ///     <see cref="Color" />, origin <see cref="Vector2" />, <see cref="FlipFlags" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="texture">The <see cref="Texture" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix3x2" />.</param>
        /// <param name="color">The <see cref="Color" />. Use <code>null</code> to use the default <see cref="Color.White" />.</param>
        /// <param name="flags">The <see cref="FlipFlags" />. The default value is <see cref="FlipFlags.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Batcher{TDrawCallInfo}.Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="texture" /> is null.</exception>
        public void DrawTexture(Texture2D texture, ref Matrix3x2 transformMatrix, Color? color = null,
            FlipFlags flags = FlipFlags.None, float depth = 0)
        {
            var rectangle = default(Rectangle);
            _geometryBuilder.BuildSprite(_vertexCount, ref transformMatrix, texture, ref rectangle, color, flags, depth);
            EnqueueBuiltGeometry(texture, depth);
        }

        private void EnqueueBuiltGeometry(Texture2D texture, float depth)
        {
            if ((_vertexCount + _geometryBuilder.VertexCount > _vertices.Length) ||
                (_indexCount + _geometryBuilder.IndexCount > _indices.Length))
                Flush();
            var drawCall = new DrawCallInfo(texture, _geometryBuilder.PrimitiveType, _indexCount,
                _geometryBuilder.PrimitivesCount, depth);
            Array.Copy(_geometryBuilder.Vertices, 0, _vertices, _vertexCount, _geometryBuilder.VertexCount);
            _vertexCount += _geometryBuilder.VertexCount;
            Array.Copy(_geometryBuilder.Indices, 0, _indices, _indexCount, _geometryBuilder.IndexCount);
            _indexCount += _geometryBuilder.IndexCount;
            Enqueue(ref drawCall);
        }

        /// <summary>
        ///     Draws unicode (UTF-16) characters as sprites using the specified <see cref="BitmapFont" />, text
        ///     <see cref="StringBuilder" />, transform <see cref="Matrix3x2" /> and optional <see cref="Color" />, origin
        ///     <see cref="Vector2" />, <see cref="FlipFlags" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="bitmapFont">The <see cref="BitmapFont" />.</param>
        /// <param name="text">The text <see cref="StringBuilder" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix3x2" />.</param>
        /// <param name="color">
        ///     The <see cref="Color" />. Use <code>null</code> to use the default
        ///     <see cref="Color.White" />.
        /// </param>
        /// <param name="flags">The <see cref="FlipFlags" />. The default value is <see cref="FlipFlags.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code>.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Batcher{TDrawCallInfo}.Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, StringBuilder text, ref Matrix3x2 transformMatrix,
            Color? color = null, FlipFlags flags = FlipFlags.None, float depth = 0f)
        {
            EnsureHasBegun();

            if (bitmapFont == null)
                throw new ArgumentNullException(nameof(bitmapFont));

            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var lineSpacing = bitmapFont.LineHeight;
            var offset = new Vector2(0, 0);

            BitmapFontCharacter lastGlyph = null;
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
                        lastGlyph = null;
                        continue;
                }

                var fontRegion = bitmapFont.GetCharacter(character);
                if (fontRegion == null)
                    continue;

                var transform1Matrix = transformMatrix;
                transform1Matrix.M31 += offset.X + fontRegion.XOffset;
                transform1Matrix.M32 += offset.Y + fontRegion.YOffset;

                var textureRegion = fontRegion.TextureRegion;
                var bounds = textureRegion.Bounds;
                DrawSprite(textureRegion.Texture, ref transform1Matrix, ref bounds, color, flags, depth);

                var advance = fontRegion.XAdvance + bitmapFont.LetterSpacing;
                if (bitmapFont.UseKernings && lastGlyph != null)
                {
                    int amount;
                    if (lastGlyph.Kernings.TryGetValue(character, out amount))
                    {
                        advance += amount;
                    }
                }

                offset.X += i != text.Length - 1
                    ? advance
                    : fontRegion.XOffset + fontRegion.TextureRegion.Width;

                lastGlyph = fontRegion;
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
        /// <exception cref="InvalidOperationException">The <see cref="Batcher{TDrawCallInfo}.Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, StringBuilder text, Vector2 position, Color? color = null,
            float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
            FlipFlags flags = FlipFlags.None, float depth = 0f)
        {
            Matrix3x2 transformMatrix;
            Matrix3x2.CreateFrom(position, rotation, scale, origin, out transformMatrix);
            DrawString(bitmapFont, text, ref transformMatrix, color, flags, depth);
        }

        /// <summary>
        ///     Draws unicode (UTF-16) characters as sprites using the specified <see cref="BitmapFont" />, text
        ///     <see cref="string" />, transform <see cref="Matrix3x2" /> and optional <see cref="Color" />, origin
        ///     <see cref="Vector2" />, <see cref="FlipFlags" />, and depth <see cref="float" />.
        /// </summary>
        /// <param name="bitmapFont">The <see cref="BitmapFont" />.</param>
        /// <param name="text">The text <see cref="string" />.</param>
        /// <param name="transformMatrix">The transform <see cref="Matrix3x2" />.</param>
        /// <param name="color">
        ///     The <see cref="Color" />. Use <code>null</code> to use the default
        ///     <see cref="Color.White" />.
        /// </param>
        /// <param name="flags">The <see cref="FlipFlags" />. The default value is <see cref="FlipFlags.None" />.</param>
        /// <param name="depth">The depth <see cref="float" />. The default value is <code>0f</code></param>
        /// <exception cref="InvalidOperationException">The <see cref="Batcher{TDrawCallInfo}.Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, string text, ref Matrix3x2 transformMatrix, Color? color = null,
            FlipFlags flags = FlipFlags.None, float depth = 0f)
        {
            EnsureHasBegun();

            if (bitmapFont == null)
                throw new ArgumentNullException(nameof(bitmapFont));

            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var glyphs = bitmapFont.GetGlyphs(text);
            foreach (var glyph in glyphs)
            {
                var transform1Matrix = transformMatrix;
                transform1Matrix.M31 += glyph.Position.X;
                transform1Matrix.M32 += glyph.Position.Y;

                var texture = glyph.Character.TextureRegion.Texture;
                var bounds = texture.Bounds;
                DrawSprite(texture, ref transform1Matrix, ref bounds, color, flags, depth);
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
        /// <exception cref="InvalidOperationException">The <see cref="Batcher{TDrawCallInfo}.Begin(ref Matrix, ref Matrix, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect)" /> method has not been called.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bitmapFont" /> is null or <paramref name="text" /> is null.</exception>
        public void DrawString(BitmapFont bitmapFont, string text, Vector2 position, Color? color = null,
            float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
            FlipFlags flags = FlipFlags.None, float depth = 0f)
        {
            Matrix3x2 matrix;
            Matrix3x2.CreateFrom(position, rotation, scale, origin, out matrix);
            DrawString(bitmapFont, text, ref matrix, color, flags, depth);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct DrawCallInfo : IBatchDrawCallInfo<DrawCallInfo>, IComparable<DrawCallInfo>
        {
            internal readonly PrimitiveType PrimitiveType;
            internal int StartIndex;
            internal int PrimitiveCount;
            internal readonly Texture2D Texture;
            internal readonly uint TextureKey;
            internal readonly uint DepthKey;

            internal unsafe DrawCallInfo(Texture2D texture, PrimitiveType primitiveType, int startIndex, int primitiveCount, float depth)
            {
                PrimitiveType = primitiveType;
                StartIndex = startIndex;
                PrimitiveCount = primitiveCount;
                Texture = texture;
                TextureKey = (uint)RuntimeHelpers.GetHashCode(texture);
                DepthKey = *(uint*)&depth;
            }

            public void SetState(Effect effect)
            {
                var textureEffect = effect as ITextureEffect;
                if (textureEffect != null)
                    textureEffect.Texture = Texture;
            }

            public bool TryMerge(ref DrawCallInfo drawCall)
            {
                if (PrimitiveType != drawCall.PrimitiveType || TextureKey != drawCall.TextureKey ||
                    DepthKey != drawCall.DepthKey)
                    return false;
                drawCall.PrimitiveCount += PrimitiveCount;
                return true;
            }

            [SuppressMessage("ReSharper", "ImpureMethodCallOnReadonlyValueField")]
            public int CompareTo(DrawCallInfo other)
            {
                var result = TextureKey.CompareTo(other.TextureKey);;
                if (result != 0)
                    return result;
                result = DepthKey.CompareTo(other.DepthKey);
                return result != 0 ? result : ((byte)PrimitiveType).CompareTo((byte)other.PrimitiveType);
            }
        }
    }
}
