using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Effects
{
    /// <summary>
    ///     An <see cref="Effect" /> that allows 2D objects, within a 3D context, to be represented on a 2D monitor.
    /// </summary>
    /// <seealso cref="Effect" />
    /// <seealso cref="IMatrixChainEffect" />
    public class DefaultEffect2D : MatrixChainEffect, ITexture2DEffect
    {
        private Texture2D _texture;
        private bool _textureIsDirty;
        private EffectParameter _textureParameter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultEffect2D" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        public DefaultEffect2D(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, EffectResource.DefaultEffect2D.Bytecode)
        {
            CacheEffectParameters();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultEffect2D" /> class.
        /// </summary>
        /// <param name="cloneSource">The clone source.</param>
        public DefaultEffect2D(Effect cloneSource)
            : base(cloneSource)
        {
            CacheEffectParameters();
        }

        /// <summary>
        ///     Gets or sets the <see cref="Texture2D" />.
        /// </summary>
        /// <value>
        ///     The <see cref="Texture2D" />.
        /// </value>
        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                _textureIsDirty = true;
            }
        }

        private void CacheEffectParameters()
        {
            _textureParameter = Parameters["TextureSampler+Texture"];
        }

        /// <summary>
        ///     Computes derived parameter values immediately before applying the effect.
        /// </summary>
        protected override bool OnApply()
        {
            var result = base.OnApply();

            // ReSharper disable once InvertIf
            if (_textureIsDirty)
            {
                _textureParameter.SetValue(_texture);
                _textureIsDirty = false;
            }

            return result;
        }
    }
}