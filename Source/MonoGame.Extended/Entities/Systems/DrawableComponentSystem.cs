using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class DrawableComponentSystem : ComponentSystem
    {
        public abstract void Draw(GameTime gameTime);
    }
}