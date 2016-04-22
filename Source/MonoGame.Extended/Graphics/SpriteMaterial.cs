using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;

namespace MonoGame.Extended.Graphics
{
    public class SpriteMaterial<TEffect> : Material<TEffect>, ISpriteDrawContext where TEffect : Effect
    {
        private SamplerState _samplerState = SamplerState.LinearClamp;

        public Texture2D Texture { get; }

        public SamplerState SamplerState
        {
            get { return _samplerState; }
            set
            {
                if (_samplerState == value)
                {
                    return;
                }

                _samplerState = value;
                NeedsUpdate = true;
            }
        }

        public SpriteMaterial(TEffect effect, Texture2D texture)
            : base(effect)
        {
            Texture = texture;
        }

        public override void Apply(out Effect effect)
        {
            base.Apply(out effect);

            var graphicsDevice = effect.GraphicsDevice;
            graphicsDevice.SamplerStates[0] = _samplerState;
            graphicsDevice.Textures[0] = Texture;
        }
    }
}
