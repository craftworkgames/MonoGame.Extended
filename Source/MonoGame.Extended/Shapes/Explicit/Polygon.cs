using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public abstract class Polygon
    {
        private PolygonFlags _flags = PolygonFlags.All;
        private Vector2[] _localVertices;
        private Vector2[] _worldVertices;
        private Vector2[] _worldNormals;

        public IReadOnlyList<Vector2> LocalVertices
        {
            get
            {
                UpdateLocalVerticesIfNecessary();
                return _localVertices;
            }
        }

        public IReadOnlyList<Vector2> WorldVertices
        {
            get
            {
                UpdateLocalVerticesIfNecessary();
                UpdateWorldVerticesAndNormalsIfNecessary();
                return _worldVertices;
            }
        }

        public IReadOnlyList<Vector2> WorldNormals
        {
            get
            {
                UpdateLocalVerticesIfNecessary();
                UpdateWorldVerticesAndNormalsIfNecessary();
                return _worldNormals;
            }
        }

        public Vector2 LocalCentroid { get; private set; }
        public Vector2 WorldCentroid { get; private set; }

        public Transform2D Tranform { get; }

        public event Action ShapeChanged;

        protected Polygon(Transform2D transform = null)
        {   
            Tranform = transform ?? new Transform2D();
            Tranform.BecameDirty += TranformBecameDirty;
        }

        protected Polygon(Vector2[] vertices, Transform2D transform = null)
            : this(transform)
        {
            SetVertices(vertices);
        }

        private void TranformBecameDirty()
        {
            _flags |= PolygonFlags.WorldVerticesAndNormalsAreDirty;
        }

        protected void SetVertices(Vector2[] vertices)
        {
            _localVertices = vertices;
            var verticesCount = vertices.Length;
            _worldVertices = new Vector2[verticesCount];
            _worldNormals = new Vector2[verticesCount];
            OnShapeChanged();
            ShapeChanged?.Invoke();
        }

        protected virtual void OnShapeChanged()
        {
            Vector2 localCentroid;
            CalculateLocalCentroid(out localCentroid);
            LocalCentroid = localCentroid;

            _flags |= PolygonFlags.WorldVerticesAndNormalsAreDirty;
        }

        protected abstract void CalculateLocalCentroid(out Vector2 centroid);
        protected abstract void UpdateLocalVertices(Vector2[] localVertices);

        public void Invalidate()
        {
            _flags |= PolygonFlags.All;
        }

        private void UpdateLocalVerticesIfNecessary()
        {
            if ((_flags & PolygonFlags.LocalVerticesAreDirty) != 0)
            {
                UpdateLocalVertices(_localVertices);
                OnShapeChanged();
                ShapeChanged?.Invoke();
                _flags &= ~PolygonFlags.LocalVerticesAreDirty;
            }
        }

        private void UpdateWorldVerticesAndNormalsIfNecessary()
        {
            if ((_flags & PolygonFlags.WorldVerticesAndNormalsAreDirty) != 0)
            {
                var worldMatrix = Tranform.WorldMatrix;
                UpdateWorldVerticesAndNormals(ref worldMatrix);
                WorldCentroid = worldMatrix.Transform(LocalCentroid);
                _flags &= ~PolygonFlags.WorldVerticesAndNormalsAreDirty;
            }
        }

        private void UpdateWorldVerticesAndNormals(ref Matrix2D transformMatrix)
        {
            var verticesCount = _localVertices.Length;
            var previousVertex = _worldVertices[verticesCount - 1] = transformMatrix.Transform(_localVertices[verticesCount - 1]);

            for (var i = 0; i < verticesCount; i++)
            {
                var vertex = _worldVertices[i] = transformMatrix.Transform(_localVertices[i]);
                var edge = previousVertex - vertex;
                _worldNormals[i] = edge.PerpendicularCounterClockwise();
                _worldNormals[i].Normalize();
                previousVertex = vertex;
            }
        }
    }
}
