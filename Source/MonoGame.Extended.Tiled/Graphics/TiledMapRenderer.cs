using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Graphics
{
    public class TiledMapRenderer : IDisposable
    {
        private readonly TiledMapEffect _defaultEffect;
        private Matrix _worldMatrix = Matrix.Identity;

        /// <summary>
        ///     Gets the <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice" /> associated with this <see cref="TiledMapRenderer" />.
        /// </summary>
        /// <value>
        ///     The <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice" /> associated with this <see cref="TiledMapRenderer" />.
        /// </value>
        public GraphicsDevice GraphicsDevice { get; }

        public TiledMapRenderer(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            GraphicsDevice = graphicsDevice;

            _defaultEffect = new TiledMapEffect(graphicsDevice);
        }

        public void Update(TiledMap map, GameTime gameTime)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var tilesetIndex = 0; tilesetIndex < map.Tilesets.Count; tilesetIndex++)
            {
                var tileset = map.Tilesets[tilesetIndex];
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var animatedTileIndex = 0; animatedTileIndex < tileset.AnimatedTiles.Count; animatedTileIndex++)
                {
                    var animatedTilesetTile = tileset.AnimatedTiles[animatedTileIndex];
                    animatedTilesetTile.Update(gameTime);
                }
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < map.TileLayers.Count; index++)
            {
                var layer = map.TileLayers[index];
                UpdateAnimatedModels(layer.AnimatedModels);
            }
        }

        public void Draw(TiledMap map, Matrix? viewMatrix = null, Matrix? projectionMatrix = null, Effect effect = null, float depth = 0.0f)
        {
            var viewMatrix1 = viewMatrix ?? Matrix.Identity;
            var projectionMatrix1 = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1);
            Draw(map, ref viewMatrix1, ref projectionMatrix1, effect, depth);
        }

        public void Draw(TiledMap map, ref Matrix viewMatrix, ref Matrix projectionMatrix, Effect effect = null, float depth = 0.0f)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < map.Layers.Count; index++)
            {
                var layer = map.Layers[index];
                Draw(layer, ref viewMatrix, ref projectionMatrix, effect, depth);
            }
        }

        public void Draw(TiledMapLayer layer, Matrix? viewMatrix = null, Matrix? projectionMatrix = null, Effect effect = null, float depth = 0.0f)
        {
            var viewMatrix1 = viewMatrix ?? Matrix.Identity;
            var projectionMatrix1 = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1);
            Draw(layer, ref viewMatrix1, ref projectionMatrix1, effect, depth);
        }

        public void Draw(TiledMapLayer layer, ref Matrix viewMatrix, ref Matrix projectionMatrix, Effect effect = null, float depth = 0.0f)
        {
            if (!layer.IsVisible)
                return;

            if (layer is TiledMapObjectLayer)
                return;

            _worldMatrix.Translation = new Vector3(layer.OffsetX, layer.OffsetY, depth);

            var effect1 = effect ?? _defaultEffect;
            var tiledMapEffect = effect1 as ITiledMapEffect;
            if (tiledMapEffect == null)
                return;

            foreach (var model in layer.Models)
            {
// model-to-world transform
                tiledMapEffect.World = _worldMatrix;
                tiledMapEffect.View = viewMatrix;
                tiledMapEffect.Projection = projectionMatrix;

                // desired alpha
                tiledMapEffect.Alpha = layer.Opacity;

                // desired texture
                tiledMapEffect.Texture = model.Texture;

                // bind the vertex and index buffer
                GraphicsDevice.SetVertexBuffer(model.VertexBuffer);
                GraphicsDevice.Indices = model.IndexBuffer;

                // for each pass in our effect
                foreach (var pass in effect1.CurrentTechnique.Passes)
                {
                    // apply the pass, effectively choosing which vertex shader and fragment (pixel) shader to use
                    pass.Apply();

                    // draw the geometry from the vertex buffer / index buffer
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.TrianglesCount);
                }
            }
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static unsafe void UpdateAnimatedModels(TiledMapLayerAnimatedModel[] animatedModels)
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
                _defaultEffect.Dispose();
        }
    }
}