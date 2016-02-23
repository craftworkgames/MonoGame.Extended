using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Demo.SpaceGame.Entities
{
    public abstract class Entity
    {
        protected Entity()
        {
            IsDestroyed = false;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        public bool IsDestroyed { get; private set; }

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }
    }
}