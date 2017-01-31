using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Graphics
{
    public class TiledMapRenderer : IDisposable
    {
        private readonly BasicEffect _basicEffect;
        private Matrix _worldMatrix = Matrix.Identity;

        /// <summary>
        ///     Gets the <see cref="GraphicsDevice" /> associated with this <see cref="TiledMapRenderer" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GraphicsDevice" /> associated with this <see cref="TiledMapRenderer" />.
        /// </value>
        public GraphicsDevice GraphicsDevice { get; }

        public TiledMapRenderer(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            GraphicsDevice = graphicsDevice;

            _basicEffect = new BasicEffect(graphicsDevice)
            {
                TextureEnabled = true
            };
        }

        public void Update(TiledMap map, GameTime gameTime)
        {
            foreach (var tileset in map.Tilesets)
            {
                foreach (var animatedTilesetTile in tileset.AnimatedTiles)
                    animatedTilesetTile.Update(gameTime);
            }

            foreach (var layer in map.TileLayers)
                UpdateAnimatedModels(layer.AnimatedModels);
        }

        public void Draw(TiledMap map, Matrix? viewMatrix = null, Matrix? projectionMatrix = null, float depth = 0.0f)
        {
            var viewMatrix1 = viewMatrix ?? Matrix.Identity;
            var projectionMatrix1 = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1);
            Draw(map, ref viewMatrix1, ref projectionMatrix1, depth);
        }

        public void Draw(TiledMap map, ref Matrix viewMatrix, ref Matrix projectionMatrix, float depth = 0.0f)
        {
            foreach (var layer in map.Layers)
                Draw(layer, ref viewMatrix, ref projectionMatrix, depth);
        }

        public void Draw(TiledMapLayer layer, Matrix? viewMatrix = null, Matrix? projectionMatrix = null,
            float depth = 0.0f)
        {
            var viewMatrix1 = viewMatrix ?? Matrix.Identity;
            var projectionMatrix1 = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1);
            Draw(layer, ref viewMatrix1, ref projectionMatrix1, depth);
        }

        public void Draw(TiledMapLayer layer, ref Matrix viewMatrix, ref Matrix projectionMatrix, float depth = 0.0f)
        {
            if (!layer.IsVisible)
                return;

            if (layer is TiledMapObjectLayer)
                return;

            _worldMatrix.Translation = new Vector3(layer.OffsetX, layer.OffsetY, depth);

            // render each model
            foreach (var model in layer.Models)
            {
                // model-to-world transform
                _basicEffect.World = _worldMatrix;
                _basicEffect.View = viewMatrix;
                _basicEffect.Projection = projectionMatrix;
                // desired alpha
                _basicEffect.Alpha = layer.Opacity;
                // bind the texture if the texture is different than what is already binded
                if (_basicEffect.Texture != model.Texture)
                    _basicEffect.Texture = model.Texture;
                // bind the vertex and index buffer
                GraphicsDevice.SetVertexBuffer(model.VertexBuffer);
                GraphicsDevice.Indices = model.IndexBuffer;

                // for each pass in our effect
                foreach (var pass in _basicEffect.CurrentTechnique.Passes)
                {
                    // apply the pass, effectively choosing which vertex shader and fragment (pixel) shader to use
                    pass.Apply();
                    // draw the geometry from the vertex buffer / index buffer
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.TrianglesCount);
                }
            }
        }

        private static unsafe void UpdateAnimatedModels(IEnumerable<TiledMapLayerAnimatedModel> animatedModels)
        {
            foreach (var animatedModel in animatedModels)
            {
                // update the texture coordinates for each animated tile
                fixed (VertexPositionTexture* fixedVerticesPointer = animatedModel.Vertices)
                {
                    var verticesPointer = fixedVerticesPointer;

                    foreach (var animatedTile in animatedModel.AnimatedTilesetTiles)
                    {
                        var currentFrameTextureCoordinates = animatedTile.CurrentAnimationFrame.TextureCoordinates;

                        (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[0];
                        (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[1];
                        (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[2];
                        (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[3];
                    }
                }

                // copy (upload) the updated vertices to the GPU's memory
                animatedModel.VertexBuffer.SetData(animatedModel.Vertices, 0, animatedModel.Vertices.Length);
            }
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
            if (diposing)
                _basicEffect.Dispose();
        }
    }
}