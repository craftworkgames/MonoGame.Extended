using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Components
{
    [Component]
    [ComponentPool(Capacity = 100)]
    public class BasicCollisionBodyComponent : Component
    {
        public Vector2 Velocity { get; set; }
        public Size2 Size { get; set; }
        public Vector2 Origin { get; set; }
        public RectangleF BoundingRectangle { get; set; } //=> new RectangleF(Entity.Position - Size * Origin, Size);
        public bool IsStatic { get; set; }

        public override void Reset()
        {
            Velocity = Vector2.Zero;
            Size = Size2.Empty;
            Origin = Vector2.Zero;
            BoundingRectangle = RectangleF.Empty;
            IsStatic = false;
        }
    }
}