namespace MonoGame.Extended.Entities.Systems
{
    public abstract class BaseSystem
    {
        public EntityWorld  World { get; internal set; }
        public abstract void Initialize(ComponentManager componentManager);
    }
}