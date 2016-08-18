using Microsoft.Xna.Framework;

namespace Demo.Platformer.Entities.Systems
{
    public abstract class DrawableComponentSystem : ComponentSystem
    {
        public abstract void Draw(GameTime gameTime);
    }
}