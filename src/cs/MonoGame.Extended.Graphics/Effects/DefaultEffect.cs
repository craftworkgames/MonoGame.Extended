using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Effects
{
    /// <summary>
    ///     An <see cref="Effect" /> that allows objects, within a 3D context, to be represented on a 2D monitor.
    /// </summary>
    /// <seealso cref="MatrixChainEffect" />
    /// <seealso cref="ITextureEffect" />
    public class DefaultEffect : MatrixChainEffect, ITextureEffect
    {
        /// <summary>
        ///     The bitmask for use with <see cref="MatrixChainEffect.Flags" /> indicating wether <see cref="Texture" /> has
        ///     changed in the last frame.
        /// </summary>
        protected static int DirtyTextureBitMask = BitVector32.CreateMask(UseDefaultProjectionBitMask);

        /// <summary>
        ///     The bitmask for use with <see cref="MatrixChainEffect.Flags" /> indicating wether the underlying vertex shader and
        ///     fragment (pixel) shaders have changed to one of the pre-defined shaders in the last frame.
        /// </summary>
        protected static int DirtyShaderIndexBitMask = BitVector32.CreateMask(DirtyTextureBitMask);

        /// <summary>
        ///     The bitmask for use with <see cref="MatrixChainEffect.Flags" /> indicating wether the material color has changed in
        ///     the last frame.
        /// </summary>
        public static int DirtyMaterialColorBitMask = BitVector32.CreateMask(DirtyShaderIndexBitMask);

        private Texture2D _texture;
        private EffectParameter _textureParameter;

        private float _alpha = 1;
        private Color _diffuseColor = Color.White;
        private EffectParameter _diffuseColorParameter;

        private bool _textureEnabled;
        private bool _vertexColorEnabled;

        /// <summary>
        ///     Gets or sets the material <see cref="Texture2D" />.
        /// </summary>
        /// <value>
        ///     The material <see cref="Texture2D" />.
        /// </value>
        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                Flags[DirtyTextureBitMask] = true;
            }
        }

        /// <summary>
        ///     Gets or sets the material color alpha.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The alpha channel uses the premultiplied (associated) representation. This means that the RGB components of a
        ///         color represent
        ///         the color of the object of pixel, adjusted for its opacity by multiplication of <see cref="Alpha" />.
        ///     </para>
        /// </remarks>
        public float Alpha
        {
            get { return _alpha; }

            set
            {
                _alpha = value;
                Flags[DirtyMaterialColorBitMask] = true;
            }
        }

        /// <summary>
        ///     Gets or sets whether texturing is enabled.
        /// </summary>
        public bool TextureEnabled
        {
            get { return _textureEnabled; }

            set
            {
                if (_textureEnabled == value)
                    return;
                _textureEnabled = value;
                Flags[DirtyShaderIndexBitMask] = true;
            }
        }

        /// <summary>
        ///     Gets or sets whether vertex color is enabled.
        /// </summary>
        public bool VertexColorEnabled
        {
            get { return _vertexColorEnabled; }

            set
            {
                if (_vertexColorEnabled == value)
                    return;
                _vertexColorEnabled = value;
                Flags[DirtyShaderIndexBitMask] = true;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultEffect" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        public DefaultEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, EffectResource.DefaultEffect.Bytecode)
        {
            Initialize();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultEffect" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="byteCode">The byte code of the shader program.</param>
        public DefaultEffect(GraphicsDevice graphicsDevice, byte[] byteCode)
            : base(graphicsDevice, byteCode)
        {
            Initialize();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultEffect" /> class.
        /// </summary>
        /// <param name="cloneSource">The clone source.</param>
        public DefaultEffect(Effect cloneSource)
            : base(cloneSource)
        {
            Initialize();
        }

        private void Initialize()
        {
            Flags[DirtyMaterialColorBitMask] = true;
            _textureParameter = Parameters["Texture"];
            _diffuseColorParameter = Parameters["DiffuseColor"];
        }

        /// <summary>
        ///     Computes derived parameter values immediately before applying the effect.
        /// </summary>
        protected override void OnApply()
        {
            base.OnApply();

            if (Flags[DirtyTextureBitMask])
            {
                _textureParameter.SetValue(_texture);
                Flags[DirtyTextureBitMask] = false;
            }

            // ReSharper disable once InvertIf
            if (Flags[DirtyMaterialColorBitMask])
            {
                UpdateMaterialColor();
                Flags[DirtyMaterialColorBitMask] = false;
            }

            // ReSharper disable once InvertIf
            if (Flags[DirtyShaderIndexBitMask])
            {
                var shaderIndex = 0;

                if (_textureEnabled)
                    shaderIndex += 1;

                if (_vertexColorEnabled)
                    shaderIndex += 2;

                Flags[DirtyShaderIndexBitMask] = false;
                CurrentTechnique = Techniques[shaderIndex];
            }
        }

        /// <summary>
        ///     Updates the material color parameters associated with this <see cref="DefaultEffect" />.
        /// </summary>
        protected virtual void UpdateMaterialColor()
        {
            var diffuseColorVector3 = _diffuseColor.ToVector3();

            var diffuseColorVector4 = new Vector4()
            {
                X = diffuseColorVector3.X * Alpha,
                Y = diffuseColorVector3.Y * Alpha,
                Z = diffuseColorVector3.Z * Alpha,
                W = Alpha,
            };

            _diffuseColorParameter.SetValue(diffuseColorVector4);
        }
    }
}