using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public sealed class CollisionBody
    {
        internal int Index;

        public Vector3 Position;

        public CollisionBodyFlags Flags { get; internal set; }

        internal CollisionBody()
        {
            ResetState();
        }

        internal void ResetState()
        {
            Flags = CollisionBodyFlags.None;
            Index = -1;
        }
    }
}
