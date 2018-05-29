using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Platformer.Collisions
{
    public enum BodyType
    {
        Static, Dynamic
    }

    [EntityComponent]
    public class Body
    {
        public BodyType BodyType { get; set; } = BodyType.Static;
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public AABB BoundingBox => new AABB(Position - Size / 2f, Position + Size / 2f);
        public Vector2 Size { get; set; }
    }
}
