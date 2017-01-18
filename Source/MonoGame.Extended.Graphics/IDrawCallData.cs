using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Defines the data for a deferred draw call
    /// </summary>
    public interface IDrawCallData
    {
        /// <summary>
        ///     Applies the parameters for the <see cref="IDrawCallData" /> to the specified
        ///     <see cref="Effect" />.
        /// </summary>
        /// <param name="effect">The effect.</param>
        void ApplyTo(Effect effect);

        /// <summary>
        ///     Sets all the associated references to <code>null</code>.
        /// </summary>
        void SetReferencesToNull();
    }
}