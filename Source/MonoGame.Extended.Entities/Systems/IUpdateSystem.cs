using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public interface IUpdateSystem : ISystem
    {
        void Update(GameTime gameTime);
    }
}