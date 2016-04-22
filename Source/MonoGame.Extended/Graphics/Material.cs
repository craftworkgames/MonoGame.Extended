using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public class Material<TEffect> : IDrawContext
        where TEffect : Effect
    {
        private BlendState _blendState = BlendState.AlphaBlend;
        private DepthStencilState _depthStencilState = DepthStencilState.None;

        public TEffect Effect { get; }

        public bool NeedsUpdate { get; protected set; }

        public BlendState BlendState
        {
            get { return _blendState; }
            set
            {
                if (_blendState == value)
                {
                    return;
                }

                _blendState = value;
                NeedsUpdate = true;
            }
        }

        public DepthStencilState DepthStencilState
        {
            get { return _depthStencilState; }
            set
            {
                if (_depthStencilState == value)
                {
                    return;
                }

                _depthStencilState = value;
                NeedsUpdate = true;
            }
        }

        public Material(TEffect effect)
        {
            if (effect == null)
            {
                throw new ArgumentNullException(nameof(effect));
            }
            Effect = effect;
        }

        // ReSharper disable once RedundantAssignment

        public virtual void Apply(out Effect effect)
        {
            var graphicsDevice = Effect.GraphicsDevice;
            graphicsDevice.BlendState = _blendState;
            graphicsDevice.DepthStencilState = _depthStencilState;
            effect = Effect;
        }
    }
}
