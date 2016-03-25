namespace MonoGame.Extended
{
    public delegate void ReturnToPoolDelegate(IPoolable poolable);

    public interface IPoolable
    {
        void Initialize(ReturnToPoolDelegate returnFunction);
        void Return();
        void Reset();
    }
}
