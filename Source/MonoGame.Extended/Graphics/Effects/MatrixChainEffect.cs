using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Effects
{
    /// <summary>
    ///     An <see cref="Effect" /> that uses the standard chain of matrix transformations to represent a 3D object on a 2D
    ///     monitor.
    /// </summary>
    /// <seealso cref="Effect" />
    /// <seealso cref="IMatrixChainEffect" />
    public abstract class MatrixChainEffect : Effect, IMatrixChainEffect
    {
        private Matrix _projection = Matrix.Identity;
        private Matrix _view = Matrix.Identity;
        private Matrix _world = Matrix.Identity;
        private bool _worldViewProjectionIsDirty;
        private EffectParameter _worldViewProjectionParameter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MatrixChainEffect" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="effectCode">The effect code.</param>
        protected MatrixChainEffect(GraphicsDevice graphicsDevice, byte[] effectCode)
            : base(graphicsDevice, effectCode)
        {
            CacheEffectParameters();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MatrixChainEffect" /> class.
        /// </summary>
        /// <param name="cloneSource">The clone source.</param>
        protected MatrixChainEffect(Effect cloneSource)
            : base(cloneSource)
        {
            CacheEffectParameters();
        }

        /// <summary>
        ///     Gets or sets the model-to-world <see cref="Matrix" />.
        /// </summary>
        /// <value>
        ///     The model-to-world <see cref="Matrix" />.
        /// </value>
        public Matrix World
        {
            get { return _world; }
            set { SetWorld(ref value); }
        }

        /// <summary>
        ///     Gets or sets the world-to-view <see cref="Matrix" />.
        /// </summary>
        /// <value>
        ///     The world-to-view <see cref="Matrix" />.
        /// </value>
        public Matrix View
        {
            get { return _view; }
            set { SetView(ref value); }
        }

        /// <summary>
        ///     Gets or sets the view-to-projection <see cref="Matrix" />.
        /// </summary>
        /// <value>
        ///     The view-to-projection <see cref="Matrix" />.
        /// </value>
        public Matrix Projection
        {
            get { return _projection; }
            set { SetProjection(ref value); }
        }

        /// <summary>
        ///     Sets the model-to-world <see cref="Matrix" />.
        /// </summary>
        /// <param name="world">The model-to-world <see cref="Matrix" />.</param>
        public void SetWorld(ref Matrix world)
        {
            _world = world;
            _worldViewProjectionIsDirty = true;
        }

        /// <summary>
        ///     Sets the world-to-view <see cref="Matrix" />.
        /// </summary>
        /// <param name="view">The world-to-view <see cref="Matrix" />.</param>
        public void SetView(ref Matrix view)
        {
            _view = view;
            _worldViewProjectionIsDirty = true;
        }

        /// <summary>
        ///     Sets the view-to-projection <see cref="Matrix" />.
        /// </summary>
        /// <param name="projection">The view-to-projection <see cref="Matrix" />.</param>
        public void SetProjection(ref Matrix projection)
        {
            _projection = projection;
            _worldViewProjectionIsDirty = true;
        }

        private void CacheEffectParameters()
        {
            _worldViewProjectionParameter = Parameters["WorldViewProjection"];
        }

        /// <summary>
        ///     Computes derived parameter values immediately before applying the effect.
        /// </summary>
        protected override bool OnApply()
        {
            base.OnApply();

            // ReSharper disable once InvertIf
            if (_worldViewProjectionIsDirty)
            {
                _worldViewProjectionIsDirty = false;

                Matrix worldViewProjection;
                Matrix.Multiply(ref _world, ref _view, out worldViewProjection);
                Matrix.Multiply(ref worldViewProjection, ref _projection, out worldViewProjection);
                _worldViewProjectionParameter.SetValue(worldViewProjection);
            }

            // in MonoGame 3.6 this method won't return anything; see https://github.com/MonoGame/MonoGame/pull/5090
            return false;
        }
    }
}