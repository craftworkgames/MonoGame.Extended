using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    internal class PrimitivesHelper
    {
        // Used by Ray2 and Segment2
        internal static bool IntersectsSlab(float positionCoordinate, float directionCoordinate, float slabMinimum,
            float slabMaximum, ref float rayMinimumDistance, ref float rayMaximumDistance)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.3; Basic Primitive Tests - Intersecting Lines, Rays, and (Directed Segments). pg 179-181

            if (Math.Abs(directionCoordinate) < float.Epsilon)
                return (positionCoordinate >= slabMinimum) && (positionCoordinate <= slabMaximum);

            // Compute intersection values of ray with near and far plane of slab
            var rayNearDistance = (slabMinimum - positionCoordinate)/directionCoordinate;
            var rayFarDistance = (slabMaximum - positionCoordinate)/directionCoordinate;

            if (rayNearDistance > rayFarDistance)
            {
                // Swap near and far distance
                var temp = rayNearDistance;
                rayNearDistance = rayFarDistance;
                rayFarDistance = temp;
            }

            // Compute the intersection of slab intersection intervals
            rayMinimumDistance = rayNearDistance > rayMinimumDistance ? rayNearDistance : rayMinimumDistance;
            rayMaximumDistance = rayFarDistance < rayMaximumDistance ? rayFarDistance : rayMaximumDistance;

            // Exit with no collision as soon as slab intersection becomes empty
            return rayMinimumDistance <= rayMaximumDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CreateRectangleFromPoints(IReadOnlyList<Vector2> points, out Vector2 minimum, out Vector2 maximum)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 82-84

            if (points == null || points.Count == 0)
            {
                minimum = Vector2.Zero;
                maximum = Vector2.Zero;
                return;
            }

            minimum = maximum = points[0];

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = points.Count - 1; index > 0; --index)
            {
                var point = points[index];
                minimum = MathExtended.CalculateMinimumVector2(minimum, point);
                maximum = MathExtended.CalculateMaximumVector2(maximum, point);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void TransformRectangle(ref Vector2 center, ref Vector2 halfExtents, ref Matrix3x2 transformMatrix)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 86-87

            center = transformMatrix.Transform(center);
            var xRadius = halfExtents.X;
            var yRadius = halfExtents.Y;
            halfExtents.X = xRadius * Math.Abs(transformMatrix.M11) + yRadius * Math.Abs(transformMatrix.M12);
            halfExtents.Y = xRadius * Math.Abs(transformMatrix.M21) + yRadius * Math.Abs(transformMatrix.M22);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void TransformOrientedRectangle(
            ref Vector2 center,
            ref Matrix3x2 orientation,
            ref Matrix3x2 transformMatrix)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.4; Oriented Bounding Boxes (OBBs), pg 101-105.

            center = transformMatrix.Transform(center);
            orientation *= transformMatrix;
            // Reset the translation since orientation is only about rotation
            orientation.M31 = 0;
            orientation.M32 = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float SquaredDistanceToPointFromRectangle(Vector2 minimum, Vector2 maximum, Vector2 point)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.1.3.1; Basic Primitive Tests - Closest-point Computations - Distance of Point to AABB.  pg 130-131
            var squaredDistance = 0.0f;

            // for each axis add up the excess distance outside the box

            // x-axis
            if (point.X < minimum.X)
            {
                var distance = minimum.X - point.X;
                squaredDistance += distance * distance;
            }
            else if (point.X > maximum.X)
            {
                var distance = maximum.X - point.X;
                squaredDistance += distance * distance;
            }

            // y-axis
            if (point.Y < minimum.Y)
            {
                var distance = minimum.Y - point.Y;
                squaredDistance += distance * distance;
            }
            else if (point.Y > maximum.Y)
            {
                var distance = maximum.Y - point.Y;
                squaredDistance += distance * distance;
            }
            return squaredDistance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ClosestPointToPointFromRectangle(Vector2 minimum, Vector2 maximum, Vector2 point, out Vector2 result)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.1.2; Basic Primitive Tests - Closest-point Computations. pg 130-131

            result = point;

            // For each coordinate axis, if the point coordinate value is outside box, clamp it to the box, else keep it as is
            if (result.X < minimum.X)
                result.X = minimum.X;
            else if (result.X > maximum.X)
                result.X = maximum.X;

            if (result.Y < minimum.Y)
                result.Y = minimum.Y;
            else if (result.Y > maximum.Y)
                result.Y = maximum.Y;
        }

    }
}
