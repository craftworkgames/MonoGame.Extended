#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

#endregion

namespace MonoGame.Extended.Tiled.Graphics
{
    public class TiledMapRenderer : Renderer
    {
        public TiledMapRenderer(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, new BasicEffect(graphicsDevice))
        {
            _basicEffect = (BasicEffect)Effect;
            _basicEffect.TextureEnabled = true;

            SamplerState = SamplerState.PointClamp;
        }

        private readonly BasicEffect _basicEffect;
        private SamplerState _samplerState;
        private Matrix _modelToWorldTransformMatrix = Matrix.Identity;

        /// <summary>
        ///     Gets or sets the <see cref="Microsoft.Xna.Framework.Graphics.SamplerState" /> associated with this
        ///     <see cref="TiledMapRenderer" />.
        /// </summary>
        /// <value>
        ///     The <see cref="Microsoft.Xna.Framework.Graphics.SamplerState" /> associated with this
        ///     <see cref="TiledMapRenderer" />.
        /// </value>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="SamplerState" /> cannot be set until <see cref="Renderer.End" /> has been invoked.
        /// </exception>
        public SamplerState SamplerState
        {
            get { return _samplerState; }
            set
            {
                EnsureHasNotBegun();
                _samplerState = value;
            }
        }

        protected override void ApplyStates()
        {
            base.ApplyStates();

            GraphicsDevice.SamplerStates[0] = SamplerState;

            _basicEffect.View = View;
            _basicEffect.Projection = Projection;
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

        public void Draw(TiledMap map)
        {
            foreach (var layer in map.Layers)
                Draw(layer);
        } 

        public void Draw(TiledMapLayer layer, float depth = 0.0f)
        {
            if (!layer.IsVisible)
                return;

            if (layer is TiledMapObjectLayer)
                return;

            _modelToWorldTransformMatrix.Translation = new Vector3(layer.OffsetX, layer.OffsetY, depth);

            // render each model
            foreach (var model in layer.Models)
            {
                // model-to-world transform
                _basicEffect.World = _modelToWorldTransformMatrix;
                // desired alpha
                _basicEffect.Alpha = layer.Opacity;
                // bind the texture if the texture is different than what is already binded
                if (_basicEffect.Texture != model.Texture)
                    _basicEffect.Texture = model.Texture;
                // bind the vertex and index buffer
                GraphicsDevice.SetVertexBuffer(model.VertexBuffer);
                GraphicsDevice.Indices = model.IndexBuffer;

                // for each pass in our effect
                foreach (var pass in Effect.CurrentTechnique.Passes)
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
    }
}