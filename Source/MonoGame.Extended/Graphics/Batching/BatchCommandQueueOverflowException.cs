using System;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     The exception that is thrown when enqueuing draw commands into a <see cref="PrimitiveBatch{TVertexType,TDrawContext}" />
    ///     results in an overflow.
    /// </summary>
    /// <seealso cref="Exception" />
    public class BatchCommandQueueOverflowException : Exception
    {
        internal BatchCommandQueueOverflowException(int maximumCommandsCount)
            : base(message: $"The maximum number of batch commands ({maximumCommandsCount}) has been reached. Consider increasing the maximum, reducing the number of commands, or flushing the commands to the GraphicsDevice.")
        {
        }
    }
}
