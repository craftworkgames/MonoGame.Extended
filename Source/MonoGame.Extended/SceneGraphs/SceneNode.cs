using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneNode : IMovable, IRotatable, IScalable
    {
        public SceneNode()
        {
            Children = new SceneNodeCollection();
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public SceneNodeCollection Children { get; }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var child in Children)
                child.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var child in Children)
                child.Update(gameTime);
        }
    }
}
