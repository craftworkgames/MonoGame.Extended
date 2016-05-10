using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Demo.PrimitiveBatch
{
    public class SpriteMaterial : EffectMaterial<SpriteEffect>
    {
        public Texture2D Texture { get; }

        public SpriteMaterial(SpriteEffect effect, Texture2D texture)
            : base(effect)
        {
            Texture = texture;
        }

        public override void Apply(out Effect effect)
        {
            Effect.Texture = Texture;
        }
    }
}
