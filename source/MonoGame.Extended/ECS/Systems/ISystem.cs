using System;

namespace MonoGame.Extended.ECS.Systems
{
    public interface ISystem : IDisposable
    {
        void Initialize(World world);
    }
}