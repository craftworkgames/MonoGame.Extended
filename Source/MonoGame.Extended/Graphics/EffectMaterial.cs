using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public abstract class EffectMaterial<TEffect> : IDrawContext
        where TEffect : Effect
    {
        public TEffect Effect { get; }

        public bool NeedsToApplyChanges { get; protected set; }

        protected EffectMaterial(TEffect effect)
        {
            if (effect == null)
            {
                throw new ArgumentNullException(nameof(effect));
            }
            Effect = effect;
        }

        public abstract void Apply(out Effect effect);
    }
}
