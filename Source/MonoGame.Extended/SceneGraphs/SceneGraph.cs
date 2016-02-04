using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneGraph : IDraw, IUpdate
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

        public void Draw(GameTime gameTime)
        {
            if (RootNode != null)
            {
                _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

                DrawNode(RootNode);

                _spriteBatch.End();
            }
        }

        public void DrawNode(SceneNode sceneNode)
        {
            var position = sceneNode.GetWorldPosition();
            var rotation = sceneNode.GetWorldRotation();
            var scale = sceneNode.GetWorldScale();

            foreach (var entity in sceneNode.Entities)
            {
                var sprite = entity as Sprite;

                if (sprite != null)
                    DrawSprite(sprite, position, rotation, scale);
            }

            foreach (var child in sceneNode.Children)
                DrawNode(child);
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

        public void Update(GameTime gameTime)
        {
            //RootNode?.Update(gameTime);
        }
    }
}