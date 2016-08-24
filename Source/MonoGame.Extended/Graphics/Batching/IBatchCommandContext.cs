using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines a context for batch draw commands.
    /// </summary>
    /// <seealso cref="IEquatableByRef{T}" />
    public interface IBatchCommandContext
    {
        /// <summary>
        ///     Applies the parameters for the <see cref="IBatchCommandContext" /> to the specified <see cref="Effect" />.
        /// </summary>
        /// <param name="effect">The effect.</param>
        void ApplyParameters(Effect effect);
    }
}
