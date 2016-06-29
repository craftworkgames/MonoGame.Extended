using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public interface IBatchItemData<TBatchItemData, in TEffect> : IEquatableByReference<TBatchItemData>
        where TBatchItemData : IBatchItemData<TBatchItemData, TEffect> where TEffect : Effect
    {
        void ApplyTo(TEffect effect);
    }
}
