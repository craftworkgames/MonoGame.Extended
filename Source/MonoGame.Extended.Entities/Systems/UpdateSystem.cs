using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public interface IUpdateSystem : ISystem
    {
        void Update(GameTime gameTime);
    }

    public abstract class UpdateSystem : IUpdateSystem
    {
        public virtual void Dispose() { }
        public virtual void Initialize(World world) { }
        public abstract void Update(GameTime gameTime);
    }
}