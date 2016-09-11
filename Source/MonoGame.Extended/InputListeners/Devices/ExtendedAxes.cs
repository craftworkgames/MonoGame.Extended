using System;

namespace MonoGame.Extended.InputListeners.Devices
{
    /// <summary>Available axes on a controller</summary>
    [Flags]
    public enum ExtendedAxes
    {

        /// <summary>X-axis, usually the left-right movement of a stick</summary>
        X = (1 << 0),
        /// <summary>Y-axis, usually the forward-backward movement of a stick</summary>
        Y = (1 << 1),
        /// <summary>Z-axis, often the throttle control</summary>
        Z = (1 << 2),
        /// <summary>X-axis velocity</summary>
        VelocityX = (1 << 3),
        /// <summary>Y-axis velocity</summary>
        VelocityY = (1 << 4),
        /// <summary>Z-axis velocity</summary>
        VelocityZ = (1 << 5),
        /// <summary>X-axis acceleration</summary>
        AccelerationX = (1 << 6),
        /// <summary>Y-axis acceleration</summary>
        AccelerationY = (1 << 7),
        /// <summary>Z-axis acceleration</summary>
        AccelerationZ = (1 << 8),
        /// <summary>X-axis force</summary>
        ForceX = (1 << 9),
        /// <summary>Y-axis force</summary>
        ForceY = (1 << 10),
        /// <summary>Z-axis force</summary>
        ForceZ = (1 << 11),

        /// <summary>X-axis rotation</summary>
        RotationX = (1 << 12),
        /// <summary>Y-axis rotation</summary>
        RotationY = (1 << 13),
        /// <summary>Z-axis rotation (often called the rudder)</summary>
        RotationZ = (1 << 14),
        /// <summary>X-axis angular velocity</summary>
        AngularVelocityX = (1 << 15),
        /// <summary>Y-axis angular velocity</summary>
        AngularVelocityY = (1 << 16),
        /// <summary>Z-axis angular velocity</summary>
        AngularVelocityZ = (1 << 17),
        /// <summary>X-axis angular acceleration</summary>
        AngularAccelerationX = (1 << 18),
        /// <summary>Y-axis angular acceleration</summary>
        AngularAccelerationY = (1 << 19),
        /// <summary>Z-axis angular acceleration</summary>
        AngularAccelerationZ = (1 << 20),
        /// <summary>X-axis torque</summary>
        TorqueX = (1 << 21),
        /// <summary>Y-axis torque</summary>
        TorqueY = (1 << 22),
        /// <summary>Z-axis torque</summary>
        TorqueZ = (1 << 23)
    }
}