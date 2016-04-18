using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Copies the X and Y components of the vector to the specified memory location.
        /// </summary>
        /// <param name="vector2">The vector to copy</param>
        /// <param name="destination">The memory location to copy the coordinate to.</param>
        public static unsafe void CopyTo(this Vector2 vector2, float* destination)
        {
            destination[0] = vector2.X;
            destination[1] = vector2.Y;
        }

        /// <summary>
        /// Gets the axis in which the vector is facing.
        /// </summary>
        /// <returns>A <see cref="Axis"/> value representing the direction the vector is facing.</returns>
        public static Axis ToAxis(this Vector2 vector2)
        {
            return new Axis(vector2.X, vector2.Y);
        }
    }
}