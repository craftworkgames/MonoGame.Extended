using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Shapes;

namespace Demo.Platformer.Entities.Components
{
    public class BasicCollisionBody : EntityComponent
    {
        public Vector2 Velocity { get; set; }
        public Size2 Size { get; set; }
        public Vector2 Origin { get; set; }
        public RectangleF BoundingRectangle => new RectangleF(Position - Size * Origin, Size);
        public bool IsStatic { get; set; }
        public object Tag { get; set; }

        public BasicCollisionBody(Size2 size, Vector2 origin)
        {
            Size = size;
            Origin = origin;
            IsStatic = false;
        }
    }
}