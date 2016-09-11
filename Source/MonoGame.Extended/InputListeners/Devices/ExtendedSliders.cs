using System;

namespace MonoGame.Extended.InputListeners.Devices
{
    /// <summary>Extended slider axes provided by a game pad or joystick</summary>
    [Flags]
    public enum ExtendedSliders
    {
        /// <summary>First additional axis (formerly called U-axis)</summary>
        Slider1 = (1 << 0),
        /// <summary>Second additional axis (formerly called V-axis)</summary>
        Slider2 = (1 << 1),
        /// <summary>First extra velocity axis</summary>
        Velocity1 = (1 << 2),
        /// <summary>Second extra velocity axis</summary>
        Velocity2 = (1 << 3),
        /// <summary>First extra acceleration axis</summary>
        Acceleration1 = (1 << 4),
        /// <summary>Second extra acceleration axis</summary>
        Acceleration2 = (1 << 5),
        /// <summary>First extra force axis</summary>
        Force1 = (1 << 6),
        /// <summary>Second extra force axis</summary>
        Force2 = (1 << 7)
    }
}