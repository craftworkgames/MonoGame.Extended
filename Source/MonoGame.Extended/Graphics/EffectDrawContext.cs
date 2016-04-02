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

        public int PassesCount
        {
            get { return Effect.CurrentTechnique.Passes.Count; }
        }

        public EffectDrawContext(TEffect effect)
        {
            Effect = effect;
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

        public void ApplyPass(int passIndex)
        {
            Effect.CurrentTechnique.Passes[passIndex].Apply();
        }

        public override bool Equals(object other)
        {
            var otherContext = other as IEffectDrawContext;
            return otherContext != null && ReferenceEquals(Effect, otherContext.Effect);
        }

        public override int GetHashCode()
        {
            return Effect?.GetHashCode() ?? 0;
        }
    }
}
