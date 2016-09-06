using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public abstract class Polygon
    {
        private Vector2[] _points;

        public IReadOnlyList<Vector2> Points => _points;

        public Vector2 Centroid { get; private set; }

        public event Action ShapeChanged;

        protected Polygon()
        {   
        }

        protected Polygon(Vector2[] vertices)
        {
            SetPoints(vertices);
        }

        protected void SetPoints(Vector2[] vertices)
        {
            _points = vertices;
            OnShapeChanged();
            ShapeChanged?.Invoke();
        }

        protected virtual void OnShapeChanged()
        {
            Vector2 centroid;
            CalculateCentroid(out centroid);
            Centroid = centroid;
        }

        protected abstract void CalculateCentroid(out Vector2 centroid);

        protected abstract void UpdateVertices(Vector2[] vertices);

        public void Invalidate()
        {
            UpdateVertices(_points);
        }
    }
}
