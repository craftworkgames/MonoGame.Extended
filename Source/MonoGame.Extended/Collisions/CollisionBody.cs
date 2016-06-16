using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public sealed class CollisionBody
    {
        //TODO: Expose this hard coded value as a parameter for the simulation.
        internal const int DefaultFixtureCapacity = 5;

        internal int Index;
        public Vector3 Position;
        public List<int> FixtureIndices;

        public CollisionBodyFlags Flags { get; internal set; }

        internal CollisionBody()
        {
            FixtureIndices = new List<int>(DefaultFixtureCapacity);
            ResetState();
        }

        internal void ResetState()
        {
            FixtureIndices.Clear();
            FixtureIndices.Capacity = DefaultFixtureCapacity;
            Flags = CollisionBodyFlags.None;
            Index = -1;
        }
    }
}
