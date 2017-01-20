using System;

namespace MonoGame.Extended.Entities.Components
{
    public abstract class EntityComponent : IDisposable
    {
        public Entity Entity { get; internal set; }

        public virtual void Dispose() { }
    }
}