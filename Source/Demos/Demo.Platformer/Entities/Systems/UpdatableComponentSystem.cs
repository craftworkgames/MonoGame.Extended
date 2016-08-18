using Microsoft.Xna.Framework;

namespace Demo.Platformer.Entities.Systems
{
    public abstract class UpdatableComponentSystem : ComponentSystem
    {
        public abstract void Update(GameTime gameTime);
    }
}
