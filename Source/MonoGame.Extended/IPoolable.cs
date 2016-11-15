using MonoGame.Extended.Collections;

namespace MonoGame.Extended
{
    public delegate void ReturnToPoolDelegate(IPoolable poolable);

    /// <summary>
    ///     Defines a poolable object to be used by a <see cref="Pool{T}" />.
    /// </summary>
    /// <remarks>
    ///     <para>See <see cref="Pool{T}" /> for code example.</para>
    /// </remarks>
    public interface IPoolable
    {
        /// <summary>
        ///     Initializes <see cref="IPoolable" /> to a in-use state. This method is called by the <see cref="Pool{T}" />
        ///     when a <see cref="IPoolable" /> is requested.
        /// </summary>
        /// <param name="returnFunction">
        ///     The return delegate used to return this <see cref="IPoolable" /> to it's
        ///     <see cref="Pool{T}" />.
        /// </param>
        void Initialize(ReturnToPoolDelegate returnFunction);

        /// <summary>
        ///     Returns the <see cref="IPoolable" /> to it's <see cref="Pool{T}" />.
        /// </summary>
        void Return();
    }
}