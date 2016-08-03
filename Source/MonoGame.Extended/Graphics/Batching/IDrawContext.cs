using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public interface IDrawContext<TData> : IEquatableByReference<TData>
        where TData : IDrawContext<TData>
    {
        void ApplyTo(Effect effect);
    }
}
