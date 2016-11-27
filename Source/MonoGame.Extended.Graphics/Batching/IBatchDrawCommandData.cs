using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines the data for batch draw commands.
    /// </summary>
    /// <typeparam name="TCommandData">The type of the batch draw command data.</typeparam>
    /// <seealso cref="Extended.IEquatableByRef{TBatchDrawCommandData}" />
    public interface IBatchDrawCommandData<TCommandData> : IEquatableByRef<TCommandData>, IComparable<TCommandData>
    {
        /// <summary>
        ///     Applies the parameters for the <see cref="IBatchDrawCommandData{TBatchDrawCommandData}" /> to the specified
        ///     <see cref="Effect" />.
        /// </summary>
        /// <param name="effect">The effect.</param>
        void ApplyTo(Effect effect);

        /// <summary>
        ///     Sets all the associated references to <code>null</code>.
        /// </summary>
        void SetReferencesToNull();
    }
}