using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneGraph : IDraw
    {
        public SceneGraph(GraphicsDevice graphicsDevice, ViewportAdapter viewportAdapter, Camera2D camera)
        {
            _viewportAdapter = viewportAdapter;
            _camera = camera;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            RootNode = SceneNode.CreateRootNode();
        }

        private readonly ViewportAdapter _viewportAdapter;
        private readonly Camera2D _camera;
        private readonly SpriteBatch _spriteBatch;

        public SceneNode RootNode { get; }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (RootNode != null)
                DrawNode(RootNode, Matrix.Identity);
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
            Draw(_spriteBatch);
            _spriteBatch.End();
        }

        public void DrawNode(SceneNode sceneNode, Matrix parentTransform)
        {
            var localTransform = sceneNode.GetLocalTransform();
            var globalTransform = Matrix.Multiply(localTransform, parentTransform);

            Vector2 position, scale;
            float rotation;
            globalTransform.Decompose(out position, out rotation, out scale);

            foreach (var sprite in sceneNode.Entities.OfType<Sprite>())
                DrawSprite(sprite, position, rotation, scale);

            foreach (var child in sceneNode.Children)
                DrawNode(child, globalTransform);
        }
        
        public void DrawSprite(Sprite sprite, Vector2 offsetPosition, float offsetRotation, Vector2 offsetScale)
        {
            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;
                var position = offsetPosition + sprite.Position;
                var rotation = offsetRotation + sprite.Rotation;
                var scale = offsetScale * sprite.Scale;

                _spriteBatch.Draw(texture, position, sourceRectangle, sprite.Color, rotation, sprite.Origin, scale, sprite.Effect, 0);
            }
        }
    }
}