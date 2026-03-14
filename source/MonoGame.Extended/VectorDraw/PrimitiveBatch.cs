// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

// Adapted from Velcro Physics (formerly known as Farseer Physics).
// Used with permission: https://github.com/craftworkgames/MonoGame.Extended/issues/574

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.VectorDraw
{
    /// <summary>
    /// A low-level primitive batch that renders colored lines and filled triangles directly to the GPU using
    /// <see cref="BasicEffect"/> with vertex colors.
    /// </summary>
    /// <remarks>
    /// Call <see cref="Begin"/> before adding vertices and <see cref="End"/> to flush all pending geometry to the GPU.
    /// This class is the underlying renderer used by <see cref="PrimitiveDrawing"/>. For drawing shapes via
    /// <see cref="SpriteBatch"/> for debug or prototype purposes, see <c>ShapeExtensions</c>.
    /// </remarks>
    public class PrimitiveBatch : IDisposable
    {
        private const int DEFAULT_BUFFER_SIZE = 500;

        private readonly BasicEffect _basicEffect;
        private readonly GraphicsDevice _device;
        private readonly VertexPositionColor[] _lineVertices;
        private readonly VertexPositionColor[] _triangleVertices;
        private BlendState _previousBlendState;
        private bool _hasBegun;
        private bool _isDisposed;
        private int _lineVertsCount;
        private int _triangleVertsCount;

        /// <summary>
        /// Initializes a new instance of <see cref="PrimitiveBatch"/>.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device used to issue draw calls.</param>
        /// <param name="bufferSize">
        /// The maximum number of vertices buffered before an automatic flush occurs. Defaults to 500.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice"/> is <see langword="null"/>.</exception>
        public PrimitiveBatch(GraphicsDevice graphicsDevice, int bufferSize = DEFAULT_BUFFER_SIZE)
        {
            ArgumentNullException.ThrowIfNull(graphicsDevice);

            _device = graphicsDevice;
            _triangleVertices = new VertexPositionColor[bufferSize - bufferSize % 3];
            _lineVertices = new VertexPositionColor[bufferSize - bufferSize % 2];

            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.VertexColorEnabled = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sets the projection matrix applied during rendering.
        /// </summary>
        /// <param name="projection">The projection matrix to apply.</param>
        public void SetProjection(ref Matrix projection)
        {
            _basicEffect.Projection = projection;
        }

        /// <summary>
        /// Returns <see langword="true"/> if <see cref="Begin"/> has been called and <see cref="End"/> has not yet
        /// been called; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsReady()
        {
            return _hasBegun;
        }

        /// <summary>
        /// Begins a new batch, setting the view and projection matrices for this frame.
        /// Must be called before any <see cref="AddVertex"/> calls.
        /// </summary>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="view">The view matrix.</param>
        /// <param name="blendState">
        /// The <see cref="BlendState"/> to use while drawing. Defaults to
        /// <see cref="BlendState.NonPremultiplied"/>, which correctly blends vertex colors specified
        /// as straight (non-premultiplied) alpha. Pass <see langword="null"/> to use the default.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="End"/> must be called before <see cref="Begin"/> can be called again.
        /// </exception>
        public void Begin(ref Matrix projection, ref Matrix view, BlendState blendState = null)
        {
            if (_hasBegun)
            {
                throw new InvalidOperationException("End must be called before Begin can be called again.");
            }

            _previousBlendState = _device.BlendState;
            _device.BlendState = blendState ?? BlendState.NonPremultiplied;

            _basicEffect.Projection = projection;
            _basicEffect.View = view;
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            _hasBegun = true;
        }

        /// <summary>
        /// Adds a single vertex to the current batch.
        /// </summary>
        /// <param name="vertex">The 2D position of the vertex.</param>
        /// <param name="color">The color of the vertex.</param>
        /// <param name="primitiveType">
        /// The primitive type this vertex belongs to. Only <see cref="PrimitiveType.TriangleList"/> and
        /// <see cref="PrimitiveType.LineList"/> are supported.
        /// </param>
        /// <exception cref="InvalidOperationException"><see cref="Begin"/> must be called before adding vertices.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="primitiveType"/> is <see cref="PrimitiveType.LineStrip"/> or
        /// <see cref="PrimitiveType.TriangleStrip"/>.
        /// </exception>
        public void AddVertex(Vector2 vertex, Color color, PrimitiveType primitiveType)
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
            }

            if (primitiveType == PrimitiveType.LineStrip || primitiveType == PrimitiveType.TriangleStrip)
            {
                throw new NotSupportedException("The specified primitiveType is not supported by PrimitiveBatch.");
            }

            if (primitiveType == PrimitiveType.TriangleList)
            {
                if (_triangleVertsCount >= _triangleVertices.Length)
                {
                    FlushTriangles();
                }

                _triangleVertices[_triangleVertsCount].Position = new Vector3(vertex, -0.1f);
                _triangleVertices[_triangleVertsCount].Color = color;
                _triangleVertsCount++;
            }

            if (primitiveType == PrimitiveType.LineList)
            {
                if (_lineVertsCount >= _lineVertices.Length)
                {
                    FlushLines();
                }

                _lineVertices[_lineVertsCount].Position = new Vector3(vertex, 0f);
                _lineVertices[_lineVertsCount].Color = color;
                _lineVertsCount++;
            }
        }

        /// <summary>
        /// Flushes all pending geometry to the GPU and ends the current batch.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Begin"/> must be called before <see cref="End"/>.</exception>
        public void End()
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before End can be called.");
            }

            FlushTriangles();
            FlushLines();
            _device.BlendState = _previousBlendState;
            _hasBegun = false;
        }

        /// <summary>
        /// Releases all managed and unmanaged resources used by this <see cref="PrimitiveBatch"/>.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release managed resources; <see langword="false"/> to release only unmanaged
        /// resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                if (_basicEffect != null)
                {
                    _basicEffect.Dispose();
                }

                _isDisposed = true;
            }
        }

        private void FlushTriangles()
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before Flush can be called.");
            }

            if (_triangleVertsCount >= 3)
            {
                int primitiveCount = _triangleVertsCount / 3;
                _device.SamplerStates[0] = SamplerState.AnisotropicClamp;
                _device.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleVertices, 0, primitiveCount);
                _triangleVertsCount -= primitiveCount * 3;
            }
        }

        private void FlushLines()
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before Flush can be called.");
            }

            if (_lineVertsCount >= 2)
            {
                int primitiveCount = _lineVertsCount / 2;
                _device.SamplerStates[0] = SamplerState.AnisotropicClamp;
                _device.DrawUserPrimitives(PrimitiveType.LineList, _lineVertices, 0, primitiveCount);
                _lineVertsCount -= primitiveCount * 2;
            }
        }
    }
}
