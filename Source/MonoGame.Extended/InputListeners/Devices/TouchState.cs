using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGame.Extended.InputListeners.Devices
{
    /// <summary>Stores the state of a touch panel</summary>
    public struct TouchState
    {
        /// <summary>Initializes a new touch panel state</summary>
        /// <param name="isAttached">Whether the touch panel is connected</param>
        /// <param name="touches">Touch events since the last update</param>
        public TouchState(bool isAttached, TouchCollection touches)
        {
            _isAttached = isAttached;
            _touches = touches;
        }

        /// <summary>Whether the touch panel is connected</summary>
        /// <remarks>
        ///   If the touch panel is not connected, all data in the state will
        ///   be neutral
        /// </remarks>
        public bool IsAttached
        {
            get { return _isAttached; }
        }

        /// <summary>Touch events that occured since the last update</summary>
        public TouchCollection Touches
        {
            get { return _touches; }
        }

        /// <summary>Whether the touch panel is connected</summary>
        private bool _isAttached;
        /// <summary>Collection of touches since the last update</summary>
        private TouchCollection _touches;

    }
}