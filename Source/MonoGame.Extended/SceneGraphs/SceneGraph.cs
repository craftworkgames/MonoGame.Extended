using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneGraph
    {
        public SceneGraph()
        {
            RootNode = SceneNode.CreateRootNode();
        }

        public SceneNode RootNode { get; }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (RootNode != null)
                DrawNode(spriteBatch, RootNode, Matrix.Identity);
        }

        private void DrawNode(SpriteBatch spriteBatch, SceneNode sceneNode, Matrix parentTransform)
        {
            var localTransform = sceneNode.GetLocalTransform();
            var globalTransform = Matrix.Multiply(localTransform, parentTransform);

            Vector2 position, scale;
            float rotation;
            globalTransform.Decompose(out position, out rotation, out scale);

            foreach (var sprite in sceneNode.Entities.OfType<Sprite>())
                DrawSprite(spriteBatch, sprite, position, rotation, scale);

            foreach (var child in sceneNode.Children)
                DrawNode(spriteBatch, child, globalTransform);
        }
        
        private void DrawSprite(SpriteBatch spriteBatch, Sprite sprite, Vector2 offsetPosition, float offsetRotation, Vector2 offsetScale)
        {
            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;
                var position = offsetPosition + sprite.Position;
                var rotation = offsetRotation + sprite.Rotation;
                var scale = offsetScale * sprite.Scale;

                spriteBatch.Draw(texture, position, sourceRectangle, sprite.Color, rotation, sprite.Origin, scale, sprite.Effect, 0);
            }
        }
    }
}