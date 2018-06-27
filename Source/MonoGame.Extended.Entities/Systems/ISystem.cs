using System;

namespace MonoGame.Extended.Entities.Systems
{
    public interface ISystem : IDisposable
    {
        void Initialize(EntityWorld world);
    }
}