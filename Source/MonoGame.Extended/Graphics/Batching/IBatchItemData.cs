using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public interface IBatchItemData<TBatchItemData> : IEquatableByReference<TBatchItemData>
        where TBatchItemData : IBatchItemData<TBatchItemData>
    {
        void ApplyTo(Effect effect);
    }
}
