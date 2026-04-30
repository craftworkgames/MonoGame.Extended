using System;

namespace MonoGame.Extended.Collisions
{
    internal readonly struct ActorPairKey : IEquatable<ActorPairKey>
    {
        public readonly int FirstId;
        public readonly int SecondId;

        public ActorPairKey(ICollisionActor first, ICollisionActor second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            if (first.Id <= second.Id)
            {
                FirstId = first.Id;
                SecondId = second.Id;
            }
            else
            {
                FirstId = second.Id;
                SecondId = first.Id;
            }
        }



        public bool Equals(ActorPairKey other)
        {
            return FirstId == other.FirstId && SecondId == other.SecondId;
        }

        public override bool Equals(object obj)
        {
            return obj is ActorPairKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstId, SecondId);
        }
    }
}
