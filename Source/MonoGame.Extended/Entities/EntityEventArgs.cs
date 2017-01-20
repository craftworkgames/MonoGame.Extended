using System;

namespace MonoGame.Extended.Entities
{
    public class EntityEventArgs : EventArgs
    {
        public EntityEventArgs(Entity entity)
        {
            Entity = entity;
        }

        public Entity Entity { get; }
    }
}
