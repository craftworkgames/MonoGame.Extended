using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Defines the ability to apply an <see cref="Effect" /> for rendering geometry.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Settings may be changed, possible including any render states, shaders, or transformation matrix, for drawing
    ///         the geometry when the <see cref="Effect" /> is applied.
    ///     </para>
    /// </remarks>
    public interface IDrawContext
    {
        bool NeedsToApplyChanges { get; }
        void Apply(out Effect effect);
    }
}
