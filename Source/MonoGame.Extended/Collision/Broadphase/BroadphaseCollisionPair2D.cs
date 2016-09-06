using System;
using System.Diagnostics;

namespace MonoGame.Extended.Collision.Broadphase
{
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct BroadphaseCollisionPair2D : IEquatable<BroadphaseCollisionPair2D>
    {
        public readonly Collider2D FirstCollider;
        public readonly Collider2D SecondCollider;

        public BroadphaseCollisionPair2D(Collider2D firstCollider, Collider2D secondCollider)
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

        public static bool operator ==(BroadphaseCollisionPair2D first, BroadphaseCollisionPair2D second)
        {
            return first.FirstCollider == second.FirstCollider && first.SecondCollider == second.SecondCollider;
        }

        public static bool operator !=(BroadphaseCollisionPair2D first, BroadphaseCollisionPair2D second)
        {
            return !(first == second);
        }

        public bool Equals(BroadphaseCollisionPair2D other)
        {
            return FirstCollider == other.FirstCollider && SecondCollider == other.SecondCollider;
        }

        public override bool Equals(object other)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FirstCollider.GetHashCode() * 397) ^ SecondCollider.GetHashCode();
            }
        }

        internal string DebugDisplayString
        {
            get { return $"FirstBody = {FirstCollider}, SecondBody = {SecondCollider}"; }
        }

        public override string ToString()
        {
            return $"{{FirstBody = {FirstCollider}, SecondBody = {SecondCollider}}}";
        }
    }
}
