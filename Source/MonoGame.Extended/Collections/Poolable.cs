namespace MonoGame.Extended.Collections
{
    public abstract class Poolable : IPoolable
    {
        public virtual bool ResetState()
        {
            return true;
        }
    }
}
