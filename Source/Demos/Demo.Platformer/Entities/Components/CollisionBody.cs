using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Shapes;
using System;

namespace Demo.Platformer.Entities.Components
{
    public sealed class CollisionBody
    {
        public CollisionHandler OnCollision;
        public event Action<bool> StaticChanged;

        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Velocity { get; set; }
        public Size2 Size { get; set; }
        
        public RectangleF BoundingRectangle => new RectangleF(Position - Size * Origin, Size);

        private bool _isStatic = false;
        public bool IsStatic
        {
            get { return _isStatic; }
            set
            {
                if (value != _isStatic)
                {
                    _isStatic = value;
                    StaticChanged?.Invoke(value);
                }
            }
        }
    }
}