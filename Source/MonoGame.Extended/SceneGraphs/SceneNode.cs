using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.SceneGraphs
{
    public interface ISceneEntity
    {
    }

    public class SceneNode : IMovable, IRotatable, IScalable
    {
        public SceneNode()
        {
            Position = Vector2.Zero;
            Rotation = 0;
            Scale = Vector2.One;
            Children = new SceneNodeCollection(this);
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public SceneNodeCollection Children { get; }

        public ISceneEntity Entity { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            var sprite = Entity as Sprite;

            if (sprite != null)
                spriteBatch.Draw(sprite, Position, Rotation, Scale);

            foreach (var child in Children)
                child.Draw(spriteBatch);
        }
        
        public void Update(GameTime gameTime)
        {
            // TODO
            //var updatable = Entity as IUpdate;
            //updatable?.Update(gameTime);

            foreach (var child in Children)
                child.Update(gameTime);
        }
    }
}
