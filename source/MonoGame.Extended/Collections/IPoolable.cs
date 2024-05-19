namespace MonoGame.Extended.Collections
{
    public delegate void ReturnToPoolDelegate(IPoolable poolable);

    public interface IPoolable
    {
        IPoolable NextNode { get; set; }
        IPoolable PreviousNode { get; set; }
        void Initialize(ReturnToPoolDelegate returnDelegate);
        void Return();
    }
}