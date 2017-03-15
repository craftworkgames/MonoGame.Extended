using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Defines an <see cref="Effect" /> that uses the standard chain of matrix transformations to represent a 3D object on
    ///     a 2D monitor.
    /// </summary>
    public interface IMatrixChainEffect : IEffectMatrices
    {
        /// <summary>
        ///     Sets the model-to-world <see cref="Matrix" />.
        /// </summary>
        /// <param name="world">The model-to-world <see cref="Matrix" />.</param>
        void SetWorld(ref Matrix world);

        /// <summary>
        ///     Sets the world-to-view <see cref="Matrix" />.
        /// </summary>
        /// <param name="view">The world-to-view <see cref="Matrix" />.</param>
        void SetView(ref Matrix view);

        /// <summary>
        ///     Sets the view-to-projection <see cref="Matrix" />.
        /// </summary>
        /// <param name="projection">The view-to-projection <see cref="Matrix" />.</param>
        void SetProjection(ref Matrix projection);
    }
}