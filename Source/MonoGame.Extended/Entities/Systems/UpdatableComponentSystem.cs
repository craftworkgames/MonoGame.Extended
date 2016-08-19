using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class UpdatableComponentSystem : ComponentSystem
    {
        public abstract void Update(GameTime gameTime);
    }
}
