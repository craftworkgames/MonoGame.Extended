using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class UpdateSystem : IDisposable
    {
        protected UpdateSystem()
        {
        }

        public virtual void Dispose() { }

        public abstract void Update(GameTime gameTime);
    }
}