using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Shapes;

namespace Demo.Platformer.Entities.Components
{
    public class BasicCollisionBody : EntityComponent
    {
        public BasicCollisionBody(SizeF size, Vector2 origin)
        {
            Size = size;
            Origin = origin;
            IsStatic = false;
        }

        public Vector2 Velocity { get; set; }
        public SizeF Size { get; set; }
        public Vector2 Origin { get; set; }
        public RectangleF BoundingRectangle => new RectangleF(Position - Size * Origin, Size);
        public bool IsStatic { get; set; }
    }
}