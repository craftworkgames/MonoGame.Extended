using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    /// <summary>
    /// Wrapper for Vector2 acting as shape
    /// </summary>
    public struct PointF : IShape1D, IMovable
    {
        public PointF(Vector2 position) {
            Position = position;
            Tolerance = 1E-05f;
        }
        public Vector2 Position { get; set; }
        public float Tolerance { get; set; }
        public RectangleF GetBoundingRectangle() => new RectangleF(Position,new Vector2(Tolerance));

        public bool Contains(Vector2 point) => point.EqualsWithTolerance(Position, Tolerance);

        public Vector2 RandomPointInside() => Position;

        public Vector2 PointOnOutline(float t) => Position;
    }
}