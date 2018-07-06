using Microsoft.Xna.Framework;

namespace Platformer.Collisions
{
    public enum BodyType
    {
        Static, Dynamic
    }

    public class Body
    {
        public BodyType BodyType = BodyType.Static;
        public Vector2 Position;
        public Vector2 Velocity;
        public AABB BoundingBox => new AABB(Position - Size / 2f, Position + Size / 2f);
        public Vector2 Size;
    }
}
