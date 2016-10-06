using System;
using System.Diagnostics;

namespace MonoGame.Extended.Collision.Detection.Broadphase
{
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct BroadphaseCollisionResult2D : IEquatable<BroadphaseCollisionResult2D>
    {
        public readonly Collider2D FirstCollider;
        public readonly Collider2D SecondCollider;

        public BroadphaseCollisionResult2D(Collider2D firstCollider, Collider2D secondCollider)
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
                throw new ArgumentException($"{nameof(firstCollider)} and {nameof(secondCollider)} can not be the same object.");
            }

            FirstCollider = firstCollider;
            SecondCollider = secondCollider;
        }

        public static bool operator ==(BroadphaseCollisionResult2D first, BroadphaseCollisionResult2D second)
        {
            return first.FirstCollider == second.FirstCollider && first.SecondCollider == second.SecondCollider;
        }

        public static bool operator !=(BroadphaseCollisionResult2D first, BroadphaseCollisionResult2D second)
        {
            return !(first == second);
        }

        public bool Equals(BroadphaseCollisionResult2D other)
        {
            return (FirstCollider == other.SecondCollider && SecondCollider == other.FirstCollider) || (FirstCollider == other.FirstCollider && SecondCollider == other.SecondCollider);
        }

        public override bool Equals(object other)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            return FirstCollider.GetHashCode() ^ SecondCollider.GetHashCode();
        }

        internal string DebugDisplayString => $"FirstBody = {FirstCollider}, SecondBody = {SecondCollider}";

        public override string ToString()
        {
            return $"{{FirstBody = {FirstCollider}, SecondBody = {SecondCollider}}}";
        }
    }
}
