using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    public class TiledMapRenderer : IDisposable
    {
        private readonly TiledMapModelBuilder _mapModelBuilder;
        private readonly TiledMapEffect _defaultEffect;
        private readonly GraphicsDevice _graphicsDevice;
        private TiledMapModel _mapModel;
        private Matrix _worldMatrix = Matrix.Identity;

        public TiledMapRenderer(GraphicsDevice graphicsDevice, TiledMap map = null)
        {
            if (graphicsDevice == null) throw new ArgumentNullException(nameof(graphicsDevice));

            _graphicsDevice = graphicsDevice;
            _defaultEffect = new TiledMapEffect(graphicsDevice);
            _mapModelBuilder = new TiledMapModelBuilder(graphicsDevice);

            if(map != null)
                LoadMap(map);
        }

        public void Dispose()
        {
            _mapModel?.Dispose();
            _defaultEffect.Dispose();
        }

        public void LoadMap(TiledMap map)
        {
            _mapModel?.Dispose();
            _mapModel = map != null ? _mapModelBuilder.Build(map) : null;
        }

        public void Update(GameTime gameTime)
        {
            if(_mapModel == null)
                return;

            for (var tilesetIndex = 0; tilesetIndex < _mapModel.Tilesets.Count; tilesetIndex++)
            {
                foreach (var animatedTilesetTile in _mapModel.GetAnimatedTiles(tilesetIndex))
                    animatedTilesetTile.Update(gameTime);
            }

            foreach(var layer in _mapModel.LayersOfLayerModels)
                UpdateAnimatedLayerModels(layer.Value.OfType<TiledMapAnimatedLayerModel>());
        }

        private static unsafe void UpdateAnimatedLayerModels(IEnumerable<TiledMapAnimatedLayerModel> animatedLayerModels)
        {
            foreach (var animatedModel in animatedLayerModels)
            {
                // update the texture coordinates for each animated tile
                fixed (VertexPositionTexture* fixedVerticesPointer = animatedModel.Vertices)
                {
                    var verticesPointer = fixedVerticesPointer;
                    for (int i = 0; i < animatedModel.AnimatedTilesetTiles.Length; i++)
                    {                       
                        var currentFrameTextureCoordinates = animatedModel.AnimatedTilesetTiles[i].CurrentAnimationFrame.GetTextureCoordinates(animatedModel.AnimatedTilesetFlipFlags[i]);

                        // ReSharper disable ArrangeRedundantParentheses
                        (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[0];
                        (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[1];
                        (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[2];
                        (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[3];
                        // ReSharper restore ArrangeRedundantParentheses
                    }
                }

                // copy (upload) the updated vertices to the GPU's memory
                animatedModel.VertexBuffer.SetData(animatedModel.Vertices, 0, animatedModel.Vertices.Length);
            }
        }

        public void Draw(Matrix? viewMatrix = null, Matrix? projectionMatrix = null, Effect effect = null, float depth = 0.0f)
        {
            var viewMatrix1 = viewMatrix ?? Matrix.Identity;
            var projectionMatrix1 = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0, 0, -1);

            Draw(ref viewMatrix1, ref projectionMatrix1, effect, depth);
        }

        public void Draw(ref Matrix viewMatrix, ref Matrix projectionMatrix, Effect effect = null, float depth = 0.0f)
        {
            if (_mapModel == null)
                return;

            for (var index = 0; index < _mapModel.Layers.Count; index++)
                Draw(index, ref viewMatrix, ref projectionMatrix, effect, depth);
        }

		public void Draw(TiledMapLayer layer, Matrix? viewMatrix = null, Matrix? projectionMatrix = null, Effect effect = null, float depth = 0.0f)
		{
			var viewMatrix1 = viewMatrix ?? Matrix.Identity;
			var projectionMatrix1 = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0, 0, -1);

			Draw(layer, ref viewMatrix1, ref projectionMatrix1, effect, depth);
		}

        public void Draw(int layerIndex, Matrix? viewMatrix = null, Matrix? projectionMatrix = null, Effect effect = null, float depth = 0.0f)
        {
            var viewMatrix1 = viewMatrix ?? Matrix.Identity;
            var projectionMatrix1 = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0, 0, -1);

            Draw(layerIndex, ref viewMatrix1, ref projectionMatrix1, effect, depth);
        }

		public void Draw(int layerIndex, ref Matrix viewMatrix, ref Matrix projectionMatrix, Effect effect = null, float depth = 0.0f)
		{
            var layer = _mapModel.Layers[layerIndex];

			Draw(layer, ref viewMatrix, ref projectionMatrix, effect, depth);
		}

		public void Draw(TiledMapLayer layer, ref Matrix viewMatrix, ref Matrix projectionMatrix, Effect effect = null, float depth = 0.0f)
		{
			if (_mapModel == null)
				return;

			if (!layer.IsVisible)
				return;

			if (layer is TiledMapObjectLayer)
				return;

			Draw(layer, Vector2.Zero, Vector2.One, ref viewMatrix, ref projectionMatrix, effect, depth);
		}

		private void Draw(TiledMapLayer layer, Vector2 parentOffset, Vector2 parentParallaxFactor, ref Matrix viewMatrix, ref Matrix projectionMatrix, Effect effect, float depth)
		{
			var offset = parentOffset + layer.Offset;
            var parallaxFactor = parentParallaxFactor * layer.ParallaxFactor;

			if (layer is TiledMapGroupLayer groupLayer)
			{
				foreach (var subLayer in groupLayer.Layers)
					Draw(subLayer, offset, parallaxFactor, ref viewMatrix, ref projectionMatrix, effect, depth);
			}
			else
			{
				_worldMatrix.Translation = new Vector3(offset, depth);

				var effect1 = effect ?? _defaultEffect;
				var tiledMapEffect = effect1 as ITiledMapEffect;
				if (tiledMapEffect == null)
					return;

                // model-to-world transform
                tiledMapEffect.World = _worldMatrix;
				tiledMapEffect.View = parallaxFactor == Vector2.One ? viewMatrix : IncludeParallax(viewMatrix, parallaxFactor);
				tiledMapEffect.Projection = projectionMatrix;

				foreach (var layerModel in _mapModel.LayersOfLayerModels[layer])
				{
					// desired alpha
					tiledMapEffect.Alpha = layer.Opacity;

					// desired texture
					tiledMapEffect.Texture = layerModel.Texture;

					// bind the vertex and index buffer
					_graphicsDevice.SetVertexBuffer(layerModel.VertexBuffer);
					_graphicsDevice.Indices = layerModel.IndexBuffer;

                    // for each pass in our effect
                    foreach (var pass in effect1.CurrentTechnique.Passes)
					{
						// apply the pass, effectively choosing which vertex shader and fragment (pixel) shader to use
						pass.Apply();

                        // draw the geometry from the vertex buffer / index buffer
                        _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, layerModel.TriangleCount);
                    }
				}
			}
        }

        private Matrix IncludeParallax(Matrix viewMatrix, Vector2 parallaxFactor)
        {
            viewMatrix.Translation *=new Vector3(parallaxFactor, 1f);
            return viewMatrix;
        }
    }
}
