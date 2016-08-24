using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines a context for individual draw calls.
    /// </summary>
    /// <seealso cref="IEquatableByRef{T}" />
    public interface IDrawContext
    {
        void Apply(Effect effect);
        void AfterApplyPass(int passIndex, Effect effect);
    }
}
