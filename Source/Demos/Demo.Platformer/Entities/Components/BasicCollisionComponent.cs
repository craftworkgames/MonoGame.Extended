using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Shapes;

namespace Demo.Platformer.Entities.Components
{
    public class BasicCollisionComponent : EntityComponent
    {
        public BasicCollisionComponent(SizeF size)
        {
            Size = size;
            IsOnGround = false;
        }

        public SizeF Size { get; }
        public RectangleF BoundingRectangle => new RectangleF(Position, Size);
        public Vector2 Velocity { get; set; }
        public bool IsOnGround { get; set; }
    }
}