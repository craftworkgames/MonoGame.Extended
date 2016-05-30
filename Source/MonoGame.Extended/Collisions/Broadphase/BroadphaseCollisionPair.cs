using System;
using System.Diagnostics;

namespace MonoGame.Extended.Collisions.Broadphase
{
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct BroadphaseCollisionPair : IEquatable<BroadphaseCollisionPair>
    {
        public readonly CollisionFixture FirstFixture;
        public readonly CollisionFixture SecondFixture;

        public BroadphaseCollisionPair(CollisionFixture firstFixture, CollisionFixture secondFixture)
        {
            if (firstFixture == null)
            {
                throw new ArgumentNullException(nameof(firstFixture));
            }
            if (secondFixture == null)
            {
                throw new ArgumentNullException(nameof(secondFixture));
            }
            if (firstFixture == secondFixture)
            {
                throw new ArgumentException($"{nameof(firstFixture)} and {nameof(secondFixture)} can not be the same object");
            }
            FirstFixture = firstFixture;
            SecondFixture = secondFixture;
        }

        public static bool operator ==(BroadphaseCollisionPair first, BroadphaseCollisionPair second)
        {
            return first.FirstFixture == second.FirstFixture && first.SecondFixture == second.SecondFixture;
        }

        public static bool operator !=(BroadphaseCollisionPair first, BroadphaseCollisionPair second)
        {
            return !(first == second);
        }

        public bool Equals(BroadphaseCollisionPair other)
        {
            return FirstFixture == other.FirstFixture && SecondFixture == other.SecondFixture;
        }

        public override bool Equals(object other)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FirstFixture.GetHashCode() * 397) ^ SecondFixture.GetHashCode();
            }
        }

        internal string DebugDisplayString
        {
            get { return $"FirstBody = {FirstFixture}, SecondBody = {SecondFixture}"; }
        }

        public override string ToString()
        {
            return $"{{FirstBody = {FirstFixture}, SecondBody = {SecondFixture}}}";
        }
    }
}
