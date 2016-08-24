using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Effects
{
    /// <summary>
    ///     An <see cref="Effect" /> that allows 2D objects, within a 3D context, to be represented on a 2D monitor.
    /// </summary>
    /// <seealso cref="Effect" />
    /// <seealso cref="IMatrixChainEffect" />
    public class DefaultEffect2D : MatrixChainEffect
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultEffect2D" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        public DefaultEffect2D(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, EffectResource.DefaultEffect2D.Bytecode)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultEffect2D" /> class.
        /// </summary>
        /// <param name="cloneSource">The clone source.</param>
        public DefaultEffect2D(Effect cloneSource)
            : base(cloneSource)
        {
        }
    }
}
