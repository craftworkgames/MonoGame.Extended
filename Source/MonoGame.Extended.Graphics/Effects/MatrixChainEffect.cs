using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Effects
{
    /// <summary>
    ///     An <see cref="Effect" /> that uses the standard chain of matrix transformations to represent a 3D object on a 2D
    ///     monitor.
    /// </summary>
    /// <seealso cref="Effect" />
    /// <seealso cref="IEffectMatrices" />
    public abstract class MatrixChainEffect : Effect, IMatrixChainEffect
    {
        /// <summary>
        ///     The bitmask for use with <see cref="Flags"/> indicating wether <see cref="World"/>, <see cref="View"/>, or <see cref="Projection"/> has changed in the last frame.
        /// </summary>
        protected static int DirtyWorldViewProjectionBitMask = BitVector32.CreateMask();

        /// <summary>
        ///     The bitmask for use with <see cref="Flags"/> indicating wether to use a default projection matrix or a custom projection matrix.
        /// </summary>
        protected static int UseDefaultProjectionBitMask = BitVector32.CreateMask(DirtyWorldViewProjectionBitMask);

        /// <summary>
        ///     The dirty flags associated with this <see cref="MatrixChainEffect"/>.
        /// </summary>
        protected BitVector32 Flags;

        private Matrix _projection = Matrix.Identity;
        private Matrix _view = Matrix.Identity;
        private Matrix _world = Matrix.Identity;
        private EffectParameter _matrixParameter;

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
        ///     Initializes a new instance of the <see cref="MatrixChainEffect" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="byteCode">The effect code.</param>
        protected MatrixChainEffect(GraphicsDevice graphicsDevice, byte[] byteCode)
            : base(graphicsDevice, byteCode)
        {
            Initialize();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MatrixChainEffect" /> class.
        /// </summary>
        /// <param name="cloneSource">The clone source.</param>
        protected MatrixChainEffect(Effect cloneSource)
            : base(cloneSource)
        {
            Initialize();
        }

        private void Initialize()
        {
            Flags[UseDefaultProjectionBitMask] = true;

            _matrixParameter = Parameters["WorldViewProjection"];
        }

        /// <summary>
        ///     Sets the model-to-world <see cref="Matrix" />.
        /// </summary>
        /// <param name="world">The model-to-world <see cref="Matrix" />.</param>
        public void SetWorld(ref Matrix world)
        {
            _world = world;
            Flags[DirtyWorldViewProjectionBitMask] = true;
        }

        /// <summary>
        ///     Sets the world-to-view <see cref="Matrix" />.
        /// </summary>
        /// <param name="view">The world-to-view <see cref="Matrix" />.</param>
        public void SetView(ref Matrix view)
        {
            _view = view;
            Flags[DirtyWorldViewProjectionBitMask] = true;
        }

        /// <summary>
        ///     Sets the view-to-projection <see cref="Matrix" />.
        /// </summary>
        /// <param name="projection">The view-to-projection <see cref="Matrix" />.</param>
        public void SetProjection(ref Matrix projection)
        {
            _projection = projection;
            Flags[DirtyWorldViewProjectionBitMask] = true;
            Flags[UseDefaultProjectionBitMask] = false;
        }

        /// <summary>
        ///     Computes derived parameter values immediately before applying the effect.
        /// </summary>
        protected override void OnApply()
        {
            base.OnApply();

            // ReSharper disable once InvertIf
            if (Flags[DirtyWorldViewProjectionBitMask] || Flags[UseDefaultProjectionBitMask])
            {
                if (Flags[UseDefaultProjectionBitMask])
                {
                    var viewport = GraphicsDevice.Viewport;
                    _projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1);
                }

                Matrix worldViewProjection;
                Matrix.Multiply(ref _world, ref _view, out worldViewProjection);
                Matrix.Multiply(ref worldViewProjection, ref _projection, out worldViewProjection);
                _matrixParameter.SetValue(worldViewProjection);

                Flags[DirtyWorldViewProjectionBitMask] = false;
            }
        }
    }
}