using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public interface IDrawSystem : ISystem
    {
        void Draw(GameTime gameTime);
    }
}