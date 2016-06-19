using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Demo.PrimitiveBatch
{
    public class SpriteEffectMaterial : EffectMaterial<SpriteEffect>, IDrawContextTexture2D
    {
        public Texture2D Texture { get; }

        public SpriteEffectMaterial(SpriteEffect effect, Texture2D texture)
            : base(effect)
        {
            Texture = texture;
        }

        public override void Apply(out Effect effect)
        {
            effect = Effect;
            Effect.Texture = Texture;
        }
    }
}
