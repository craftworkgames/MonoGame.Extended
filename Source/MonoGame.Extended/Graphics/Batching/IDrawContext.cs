using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines a context for individual draw calls.
    /// </summary>
    /// <typeparam name="TDrawContext">The type of the drawing context.</typeparam>
    /// <seealso cref="IEquatableByRef{T}" />
    public interface IDrawContext<TDrawContext> : IEquatableByRef<TDrawContext>
    {
        void ApplyPass(Effect effect, int passIndex, EffectPass pass);
    }
}
