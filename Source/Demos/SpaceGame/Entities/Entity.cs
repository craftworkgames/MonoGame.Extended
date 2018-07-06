using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Entities
{
    public abstract class Entity
    {
        public bool IsDestroyed { get; private set; }

        protected Entity()
        {
            IsDestroyed = false;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }
    }
}