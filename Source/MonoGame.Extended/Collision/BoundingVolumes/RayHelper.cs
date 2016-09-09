using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Extended.Collision.BoundingVolumes
{
    internal static class RayHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool RayIntersectsSlab(float positionCoordinate, float directionCoordinate, float slabMinimum, float slabMaximum, ref float rayMinimumDistance, ref float rayMaximumDistance)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.3; Basic Primitive Tests - Intersecting Lines, Rays, and (Directed Segments). pg 179-181

            if (Math.Abs(directionCoordinate) < float.Epsilon)
            {
                // Ray is parallel to slab. No hit if origin is not with in the slab
                return positionCoordinate >= slabMinimum && positionCoordinate <= slabMaximum;
            }

            // Compute intersection values of ray with near and far plane of slab
            var rayNearDistance = (slabMinimum - positionCoordinate) / directionCoordinate;
            var rayFarDistance = (slabMaximum - positionCoordinate) / directionCoordinate;

            if (rayNearDistance > rayFarDistance)
            {
                var temp = rayFarDistance;
                rayNearDistance = rayFarDistance;
                rayFarDistance = temp;
            }

            // Compute the intersection of slab intersection intervals
            rayMinimumDistance = rayNearDistance > rayMinimumDistance ? rayNearDistance : rayMinimumDistance;
            rayMaximumDistance = rayFarDistance < rayMaximumDistance ? rayFarDistance : rayMaximumDistance;

            // Exit with no collision as soon as slab intersection becomes empty
            return rayMinimumDistance <= rayMaximumDistance;
        }
    }
}
