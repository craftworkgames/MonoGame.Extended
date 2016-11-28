using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines options for how geometric objects are submitted to a <see cref="GraphicsDevice" />.
    /// </summary>
    public enum BatchSortMode : byte
    {
        /// <summary>
        ///     Each <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}.Draw" /> call invokes
        ///     <see cref="GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, int, int, int)" /> immediately for each
        ///     <see cref="EffectPass" />.
        /// </summary>
        Immediate,

        /// <summary>
        ///     Each <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}.Draw" /> call is added to the end of an array of draw
        ///     commands. Draw commands will be merged, if possible, for batching. When
        ///     <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}.Flush" /> or
        ///     <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}.End" /> is called, the merged draw commands will be processed
        ///     on a first come, first serve, basis where each command invokes
        ///     <see cref="GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, int, int, int)" /> for each
        ///     <see cref="EffectPass" />.
        /// </summary>
        Deferred,

        /// <summary>
        ///     Each <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}.Draw" /> call is added to the end of an array of draw
        ///     commands. Draw commands will be merged, if possible, for batching as are they called and after they are sorted in
        ///     ascending order by their sort keys. When
        ///     <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}.Flush" /> or
        ///     <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}.End" /> is called, the sorting takes place and then the merged
        ///     draw commands are processed where each command invokes
        ///     <see cref="GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, int, int, int)" /> for each
        ///     <see cref="EffectPass" />.
        /// </summary>
        DeferredSorted
    }
}