using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Platformer.Collisions
{
    public static class CollisionTester
    {
        public static bool AabbAabb(AABB a, AABB b)
        {
            if (a.Max.X < b.Min.X || a.Min.X > b.Max.X)
                return false;

            if (a.Max.Y < b.Min.Y || a.Min.Y > b.Max.Y)
                return false;

            return true;
        }
        
        public static bool AabbAabb(AABB aBox, AABB bBox, Vector2 vector, out Manifold manifold)
        {
            manifold = new Manifold();

            // Calculate half extents along x axis
            var axExtent = (aBox.Max.X - aBox.Min.X) / 2f;
            var bxExtent = (bBox.Max.X - bBox.Min.X) / 2f;

            // Calculate overlap on x axis
            manifold.Overlap.X = axExtent + bxExtent - Math.Abs(vector.X);

            // SAT test on x axis
            if (manifold.Overlap.X > 0)
            {
                // Calculate half extents along x axis for each object
                var ayExtent = (aBox.Max.Y - aBox.Min.Y) / 2f;
                var byExtent = (bBox.Max.Y - bBox.Min.Y) / 2f;

                // Calculate overlap on y axis
                manifold.Overlap.Y = ayExtent + byExtent - Math.Abs(vector.Y);

                // SAT test on y axis
                if (manifold.Overlap.Y > 0)
                {
                    // Find out which axis is axis of least penetration
                    if (manifold.Overlap.X < manifold.Overlap.Y)
                    {
                        manifold.Normal = vector.X < 0 ? new Vector2(-1, 0) : new Vector2(1, 0);
                        manifold.Penetration = manifold.Overlap.X;
                        return true;
                    }

                    // Point toward B knowing that n points from A to B
                    manifold.Normal = vector.Y < 0 ? new Vector2(0, -1) : new Vector2(0, 1);
                    manifold.Penetration = manifold.Overlap.Y;
                    return true;
                }
            }

            return false;
        }

        public static bool CircleCircle(CircleF a, CircleF b)
        {
            var distance = a.Radius + b.Radius;
            var distanceSquared = distance * distance;
            var x = a.Center.X + b.Center.X;
            var y = a.Center.Y + b.Center.Y;
            return distanceSquared < x * x + y * y;
        }

        public static bool CircleCircle(CircleF a, CircleF b, out Manifold manifold)
        {
            manifold = new Manifold();

            // Vector from A to B
            var vector = b.Center - a.Center;
            var radiusSquared = a.Radius + b.Radius;
            radiusSquared *= radiusSquared;
            
            if (vector.LengthSquared() > radiusSquared)
                return false;

            // Circles have collided, now compute manifold
            var distance = vector.Length();

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (distance != 0)
            {
                manifold.Penetration = radiusSquared - distance;
                manifold.Normal = vector / distance;
                return true;
            }

            // Circles are on same position
            manifold.Penetration = a.Radius;
            manifold.Normal = new Vector2(1, 0);
            return true;
        }
    }
}
