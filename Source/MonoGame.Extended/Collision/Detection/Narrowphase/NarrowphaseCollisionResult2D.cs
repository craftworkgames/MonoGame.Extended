using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collision.Detection.Narrowphase
{
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct NarrowphaseCollisionResult2D
    {
        public readonly Collider2D FirstCollider;
        public readonly Collider2D SecondCollider;
        public readonly Vector2 MinimumPenetrationAxis;
        public readonly float MinimumPenetration;
        public readonly int ContactPointsCount;
        public readonly Vector2 FirstContactPoint;
        public readonly Vector2 SecondContactPoint;

        public NarrowphaseCollisionResult2D(Collider2D firstCollider, Collider2D secondCollider, Vector2 minimumPenetrationAxis, float minimumPenetration)
        {
            if (firstCollider == null)
            {
                throw new ArgumentNullException(nameof(firstCollider));
            }

            if (secondCollider == null)
            {
                throw new ArgumentNullException(nameof(secondCollider));
            }

            if (firstCollider == secondCollider)
            {
                throw new ArgumentException(
                    $"{nameof(firstCollider)} and {nameof(secondCollider)} can not be the same object.");
            }

            FirstCollider = firstCollider;
            SecondCollider = secondCollider;
            MinimumPenetrationAxis = minimumPenetrationAxis;
            MinimumPenetration = minimumPenetration;
            ContactPointsCount = 0;
            FirstContactPoint = default(Vector2);
            SecondContactPoint = default(Vector2);
        }

        public NarrowphaseCollisionResult2D(Collider2D firstCollider, Collider2D secondCollider, Vector2 minimumPenetrationAxis, float minimumPenetration, Vector2 contactPoint)
            : this(firstCollider, secondCollider, minimumPenetrationAxis, minimumPenetration)
        {
            ContactPointsCount = 1;
            FirstContactPoint = contactPoint;
        }

        public NarrowphaseCollisionResult2D(Collider2D firstCollider, Collider2D secondCollider, Vector2 minimumPenetrationAxis, float minimumPenetration, Vector2 firstContactPoint, Vector2 secondContactPoint)
            : this(firstCollider, secondCollider, minimumPenetrationAxis, minimumPenetration)
        {
            ContactPointsCount = 2;
            FirstContactPoint = firstContactPoint;
            SecondContactPoint = secondContactPoint;
        }

        internal string DebugDisplayString => $"Normal = {MinimumPenetrationAxis}, Penetration = {MinimumPenetration}";

        public override string ToString()
        {
            return $"{{Normal = {MinimumPenetrationAxis}, Penetration = {MinimumPenetration}}}";
        }
    }
}
