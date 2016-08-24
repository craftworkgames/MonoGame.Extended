using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines a context for individual draw calls.
    /// </summary>
    /// <seealso cref="IEquatableByRef{T}" />
    public interface IDrawContext
    {
        void ApplyPass(Effect effect, int passIndex, EffectPass pass);
    }
}
