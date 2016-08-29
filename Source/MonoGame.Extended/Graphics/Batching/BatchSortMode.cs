using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines options for how geometric objects are submitted to a <see cref="GraphicsDevice" />.
    /// </summary>
    public enum BatchSortMode
    {
        /// <summary>
        ///     Each geometric object is submitted to a <see cref="GraphicsDevice" /> without delay.
        /// </summary>
        Immediate,

        /// <summary>
        ///     Geometric objects are delayed for submission to a <see cref="GraphicsDevice" /> until
        ///     <see cref="Batch{TVertexType,TBatchDrawCommandData}.Flush" /> or
        ///     <see cref="Batch{TVertexType,TBatchDrawCommandData}.End" /> is called at which point they are processed on a
        ///     first come, first serve basis.
        /// </summary>
        Deferred,

        /// <summary>
        ///     Geometry is delayed for submission to a <see cref="GraphicsDevice" /> until
        ///     <see cref="Batch{TVertexType,TBatchDrawCommandData}.Flush" /> or
        ///     <see cref="Batch{TVertexType,TBatchDrawCommandData}.End" /> is called at which point they are sorted before being processed.
        ///     first come, first serve basis.
        /// </summary>
        DeferredSorted
    }
}