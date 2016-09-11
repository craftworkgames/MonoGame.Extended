namespace MonoGame.Extended.InputListeners.Devices
{
    /// <summary>Generic input device</summary>
    public interface IInputDevice
    {
        /// <summary>Whether the input device is connected to the system</summary>
        bool IsAttached { get; }

        /// <summary>Human-readable name of the input device</summary>
        string Name { get; }

        /// <summary>Updates the state of the input device</summary>
        /// <remarks>
        ///   <para>
        ///     If this method is called with no snapshots in the queue, it will take
        ///     an immediate snapshot and make it the current state. This way, you
        ///     can use the input devices without caring for the snapshot system if
        ///     you wish.
        ///   </para>
        ///   <para>
        ///     If this method is called while one or more snapshots are waiting in
        ///     the queue, this method takes the next snapshot from the queue and makes
        ///     it the current state.
        ///   </para>
        /// </remarks>
        void Update();

        /// <summary>Takes a snapshot of the current state of the input device</summary>
        /// <remarks>
        ///   This snapshot will be queued until the user calls the Update() method,
        ///   where the next polled snapshot will be taken from the queue and provided
        ///   to the user.
        /// </remarks>
        void TakeSnapshot();
    }
}
