using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    /// Defines a drawing context for individual draw calls.
    /// </summary>
    /// <typeparam name="TDrawContext">The type of the draw context.</typeparam>
    /// <seealso cref="IEquatableByReference{IDrawContext}" />
    public interface IDrawContext<TDrawContext> : IEquatableByReference<TDrawContext>
    {
        void ApplyTo(Effect effect);
    }
}
