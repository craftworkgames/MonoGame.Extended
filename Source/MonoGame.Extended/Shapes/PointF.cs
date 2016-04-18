using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    /// <summary>
    /// Shape wrapper for Vector2
    /// </summary>
    public struct PointF : IShapeBase, IMovable
    {
        public PointF(Vector2 position) {
            Position = position;
        }
        public Vector2 Position { get; set; }
        public RectangleF GetBoundingRectangle() => new RectangleF(Position,Vector2.Zero);

        public bool Contains(Vector2 point) => point.EqualsWithTolerance(Position);
        
        public Vector2 PointOnOutline(float t) => Position;
    }
}