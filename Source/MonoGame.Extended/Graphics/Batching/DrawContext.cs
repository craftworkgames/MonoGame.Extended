using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Graphics.Batching
{
    public sealed class DrawContext<TEffect> : IDrawContext
        where TEffect : Effect
    {
        public TEffect Effect { get; }

        Effect IDrawContext.Effect
        {
            get { return Effect; }
        }

        private BitVector32 SortKey { get; }

        uint IDrawContext.SortKey
        {
            get { return SortKey; }
        }

        public DrawContext(TEffect effect)
        {
            Effect = effect;
        }  

        private void UpdateSortKey()
        {
        }


        public void Begin()
        {
            throw new System.NotImplementedException();
        }

        public void End()
        {
            throw new System.NotImplementedException();
        }
    }
}
