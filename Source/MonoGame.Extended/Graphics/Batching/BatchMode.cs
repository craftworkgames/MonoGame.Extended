namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines batch rendering options.
    /// </summary>
    public enum BatchMode
    {
        /// <summary>
        ///     Geometry will be drawn within each draw call.
        /// </summary>
        Immediate,

        /// <summary>
        ///     Geometry is not drawn until <see cref="PrimitiveBatch{TVertexType}.End" /> is called or the maximum vertices or
        ///     indices count has been reached for the current batch.
        /// </summary>
        Deferred
    }
}
