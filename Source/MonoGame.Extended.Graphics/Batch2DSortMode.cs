using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Defines options for how two-dimensional geometric objects are submitted to a <see cref="GraphicsDevice" />.
    /// </summary>
    public enum Batch2DSortMode : byte
    {
        /// <summary>
        ///     Each draw call invokes
        ///     <see cref="GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, int, int, int)" /> immediately for each
        ///     <see cref="EffectPass" />.
        /// </summary>
        Immediate,

        /// <summary>
        ///     Each draw call is added to the end of an array of draw commands. Draw commands will be merged, if possible, for
        ///     batching. When <see cref="DynamicBatchRenderer2D.End" /> is called, the merged draw commands are processed on a first come,
        ///     first
        ///     serve, basis where each command invokes
        ///     <see cref="GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, int, int, int)" /> for each
        ///     <see cref="EffectPass" />.
        /// </summary>
        Deferred,

        /// <summary>
        ///     Each draw call is added to the end of an array of draw commands. Draw commands will be merged, if possible, for
        ///     batching as they are enqueued and after they are sorted by their <see cref="Texture2D" />s. When
        ///     <see cref="DynamicBatchRenderer2D.End" /> is called, the sorting takes place and then the merged draw commands are
        ///     processed where
        ///     each command invokes <see cref="GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, int, int, int)" /> for each
        ///     <see cref="EffectPass" />.
        /// </summary>
        Texture,

        /// <summary>
        ///     Each draw call is added to the end of an array of draw commands. Draw commands will be merged, if possible, for
        ///     batching as they are enqueued and after they are sorted by their depth values in ascending order. When
        ///     <see cref="DynamicBatchRenderer2D.End" /> is called, the sorting takes place and then the merged draw commands are
        ///     processed where
        ///     each command invokes <see cref="GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, int, int, int)" /> for each
        ///     <see cref="EffectPass" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Depth values, by default, range from <code>0.0f</code> to <code>1.0f</code>, inclusively, where smaller values
        ///         such as <code>0.0f</code> are considered in-front of larger values such as <code>1.0f</code>.
        ///     </para>
        /// </remarks>
        FrontToBack,

        /// <summary>
        ///     Each draw call is added to the end of an array of draw commands. Draw commands will be merged, if possible, for
        ///     batching as they are enqueued and after they are sorted by their depth values in descending order. When
        ///     <see cref="DynamicBatchRenderer2D.End" /> is called, the sorting takes place and then the merged draw commands are
        ///     processed where
        ///     each command invokes <see cref="GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, int, int, int)" /> for each
        ///     <see cref="EffectPass" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Depth values, by default, range from <code>0.0f</code> to <code>1.0f</code>, inclusively, where smaller values
        ///         such as <code>0.0f</code> are considered in-front of larger values such as <code>1.0f</code>.
        ///     </para>
        /// </remarks>
        BackToFront
    }
}