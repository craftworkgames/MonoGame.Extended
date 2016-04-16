using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public class EffectDrawContext<TEffect> : IEffectDrawContext, IDisposable
        where TEffect : Effect
    {
        public TEffect Effect { get; }

        Effect IEffectDrawContext.Effect
        {
            get { return Effect; }
        }

        public uint SortKey { get; }

        public int PassesCount
        {
            get { return Effect.CurrentTechnique.Passes.Count; }
        }

        public EffectDrawContext(TEffect effect, uint sortKey = 0)
        {
            if (effect == null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            Effect = effect;
            SortKey = sortKey;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Effect?.Dispose();
        }

        public virtual void Begin()
        {
        }

        public virtual void End()
        {
        }

        public void ApplyPass(int passIndex)
        {
            Effect.CurrentTechnique.Passes[passIndex].Apply();
        }
    }
}
