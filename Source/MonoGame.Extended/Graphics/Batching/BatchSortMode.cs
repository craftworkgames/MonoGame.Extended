using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines options for submitting the geometry which batch draw commands hold to a <see cref="GraphicsDevice" />.
    /// </summary>
    public enum BatchSortMode
    {
        /// <summary>
        ///     Geometry is submitted to a <see cref="GraphicsDevice" /> for rendering as soon as the draw command is enqueued.
        /// </summary>
        Immediate,

        /// <summary>
        ///     Geometry is not submitted to <see cref="GraphicsDevice" /> for rendering until
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}.Flush" /> or
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}.End" /> is called. Draw commands are enqueued but not sorted.
        ///     The behaviour is equivalent to first come, first serve (FCFS) queue.
        /// </summary>
        Deferred,

        /// <summary>
        ///     Geometry is not submitted to <see cref="GraphicsDevice" /> for rendering until
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}.Flush" /> or
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}.End" /> is called. Draw commands are enqueued and sorted in
        ///     descending order based the <see cref="BatchCommand{TBatchDrawContext}.SortKey" />. The behaviour is equivalent to
        ///     priority queue where a command with a larger sort key value has higher priority than a command with a smaller sort
        ///     key value.
        /// </summary>
        DeferredSorted,
    }
}
