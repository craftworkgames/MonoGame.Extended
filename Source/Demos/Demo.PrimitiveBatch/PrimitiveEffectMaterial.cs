using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Demo.PrimitiveBatch
{
    // just a simple effect material for the polygon effect
    public class PrimitiveEffectMaterial : EffectMaterial<PrimitiveEffect>
    {
        public PrimitiveEffectMaterial(PrimitiveEffect effect)
            : base(effect)
        {
        }

        public override void Apply(out Effect effect)
        {
            effect = Effect;
        }
    }
}
