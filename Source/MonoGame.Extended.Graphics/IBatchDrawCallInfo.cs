using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Defines the for a deferred draw call when batching.
    /// </summary>
    public interface IBatchDrawCallInfo<TDrawCallInfo> where TDrawCallInfo : IBatchDrawCallInfo<TDrawCallInfo>
    {
        /// <summary>
        ///     Applies any state from the <see cref="IBatchDrawCallInfo{TDrawCallInfo}" /> to the
        ///     <see cref="Effect" /> or <see cref="Effect.GraphicsDevice"/>.
        /// </summary>
        /// <param name="effect">The effect.</param>
        void SetState(Effect effect);

        bool TryMerge(ref TDrawCallInfo drawCall);
    }
}