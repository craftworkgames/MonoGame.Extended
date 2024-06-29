using System;

namespace MonoGame.Extended.Collections
{
    public interface IPoolable
    {
        IPoolable NextNode { get; set; }
        IPoolable PreviousNode { get; set; }
        void Initialize(Action<IPoolable> returnDelegate);
        void Return();
    }
}
