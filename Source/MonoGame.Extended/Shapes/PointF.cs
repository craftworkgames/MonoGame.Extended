using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    /// <summary>
    /// Shape wrapper for Vector2
    /// </summary>
    public struct PointF : IMovable
    {
        public PointF(Vector2 position) {
            Position = position;
        }
        public Vector2 Position { get; set; }
        public float Left => Position.X;
        public float Top => Position.Y;
        public float Right => Position.X;
        public float Bottom => Position.Y;
        public RectangleF GetBoundingRectangle() => new RectangleF(Position, Vector2.Zero);

        public bool Contains(Vector2 point) => point.EqualsWithTolerance(Position);

        public Vector2 PointOnOutline(float t) => Position;
    }
}