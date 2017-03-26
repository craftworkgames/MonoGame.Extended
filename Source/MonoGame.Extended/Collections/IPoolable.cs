namespace MonoGame.Extended.Collections
{
    public delegate void ReturnToPoolDelegate(IPoolable poolable);

    /// <summary>
    ///     Defines a poolable object to be used by a <see cref="ObjectPool{T}" />.
    /// </summary>
    /// <remarks>
    ///     <para>See <see cref="ObjectPool{T}" /> for code example.</para>
    /// </remarks>
    public interface IPoolable
    {
        /// <summary>
        ///     Initializes <see cref="IPoolable" /> to a in-use state. This method is called by the <see cref="ObjectPool{T}" />
        ///     when a <see cref="IPoolable" /> is requested.
        /// </summary>
        /// <param name="returnDelegate">
        ///     The delegate used to return this <see cref="IPoolable" /> to it's
        ///     <see cref="ObjectPool{T}" />.
        /// </param>
        void Initialize(ReturnToPoolDelegate returnDelegate);

        /// <summary>
        ///     Returns the <see cref="IPoolable" /> to it's <see cref="ObjectPool{T}" />.
        /// </summary>
        void Return();
    }
}