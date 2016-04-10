using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    /// <summary>
    /// Wrapper for Vector2 acting as shape
    /// </summary>
    public struct PointF : IShapeF, IMovable
    {
        public PointF(Vector2 position) {
            Position = position;
            Tolerance = 1E-05f;
        }
        public Vector2 Position { get; set; }
        public float Tolerance { get; set; }
        public float Left => Position.X;
        public float Top => Position.Y;
        public float Right => Position.X;
        public float Bottom => Position.Y;
        public RectangleF GetBoundingRectangle() => new RectangleF(Position,new Vector2(Tolerance));

        public bool Contains(Vector2 point) => point.EqualsWithTolerance(Position, Tolerance);

        public Vector2 RandomPointInside() => Position;

        public Vector2 PointOnOutline(float t) => Position;
    }
}