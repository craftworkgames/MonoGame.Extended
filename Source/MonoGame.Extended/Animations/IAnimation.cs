using System;

namespace MonoGame.Extended.Animations
{
    public interface IAnimation : IUpdate, IDisposable
    {
        bool IsComplete { get; }
        bool IsDisposed { get; }
    }
}