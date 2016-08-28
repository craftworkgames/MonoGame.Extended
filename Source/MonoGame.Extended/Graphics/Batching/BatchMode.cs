using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines options for how geometry is submitted to a <see cref="GraphicsDevice" />.
    /// </summary>
    public enum BatchMode
    {
        /// <summary>
        ///     Geometry is submitted to a <see cref="GraphicsDevice" /> immediately with out deferment.
        /// </summary>
        Immediate,

        /// <summary>
        ///     Geometry is delayed for submission to a <see cref="GraphicsDevice" /> until
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}.Flush" /> or
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}.End" /> is called at which point the geometry is processed on a
        ///     first come, first serve basis.
        /// </summary>
        Deferred,

        /// <summary>
        ///     Same as <see cref="Deferred" /> except the geometry is sorted by the sort key in descending order before being
        ///     submitted to a <see cref="GraphicsDevice" />.
        /// </summary>
        DeferredSorted
    }
}