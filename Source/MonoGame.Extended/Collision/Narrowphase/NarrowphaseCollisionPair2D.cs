using System;
using System.Diagnostics;

namespace MonoGame.Extended.Collision.Narrowphase
{
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct NarrowphaseCollisionPair2D : IEquatable<NarrowphaseCollisionPair2D>
    {
        public readonly Collider2D FirstCollider;
        public readonly Collider2D SecondCollider;

        public NarrowphaseCollisionPair2D(Collider2D firstCollider, Collider2D secondCollider)
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

        public static bool operator ==(NarrowphaseCollisionPair2D first, NarrowphaseCollisionPair2D second)
        {
            return first.FirstCollider == second.FirstCollider && first.SecondCollider == second.SecondCollider;
        }

        public static bool operator !=(NarrowphaseCollisionPair2D first, NarrowphaseCollisionPair2D second)
        {
            return !(first == second);
        }

        public bool Equals(NarrowphaseCollisionPair2D other)
        {
            return FirstCollider == other.FirstCollider && SecondCollider == other.SecondCollider;
        }

        public override bool Equals(object other)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException();
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
