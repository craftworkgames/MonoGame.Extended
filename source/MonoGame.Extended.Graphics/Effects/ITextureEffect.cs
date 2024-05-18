using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Effects
{
    /// <summary>
    ///     Defines an <see cref="Effect" /> that uses a <see cref="Texture2D" />.
    /// </summary>
    public interface ITextureEffect
    {
        /// <summary>
        ///     Gets or sets the <see cref="Texture2D" />.
        /// </summary>
        /// <value>
        ///     The <see cref="Texture2D" />.
        /// </value>
        Texture2D Texture { get; set; }
    }
}