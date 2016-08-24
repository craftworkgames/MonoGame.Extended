using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines batch rendering options.
    /// </summary>
    public enum BatchMode
    {
        /// <summary>
        ///     Geometry is submitted to <see cref="GraphicsDevice" /> for rendering with each draw call..
        /// </summary>
        Immediate,

        /// <summary>
        ///     Geometry is not submitted to <see cref="GraphicsDevice" /> for rendering until
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}.End" /> is called, or the maximum vertices or indices count has
        ///     been reached for the current batch.
        /// </summary>
        Deferred
    }
}
