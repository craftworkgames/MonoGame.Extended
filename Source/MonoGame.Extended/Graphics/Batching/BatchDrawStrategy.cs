namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Defines application programming interfaces (APIs) available to send vertices, and possibly indices, to the graphics
    ///     processing unit (GPU).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         On desktop platforms the <see cref="BatchDrawStrategy" /> used does not matter significantly. On other
    ///         platforms, such as consoles and mobiles, one <see cref="BatchDrawStrategy" /> may be superior, or even
    ///         required, in comparision to another <see cref="BatchDrawStrategy" /> due to the limited hardware and the
    ///         drivers controlling the hardware.
    ///     </para>
    /// </remarks>
    public enum BatchDrawStrategy
    {
        UserPrimitives,
        DynamicBuffer
    }
}
