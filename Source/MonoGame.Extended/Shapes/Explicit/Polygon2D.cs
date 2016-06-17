using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public class Polygon2D
    {
        private readonly Vector2[] _localVertices;
        private readonly Vector2[] _transformedVertices;

        private Vector2 _position;
        private float _rotation;
        private Vector2 _scale = Vector2.One;

        public IReadOnlyList<Vector2> LocalVertices
        {
            get { return _localVertices; }
        }

        public IReadOnlyList<Vector2> TransformedVertices
        {
            get { return _transformedVertices; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                UpdateTransform();
            }
        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                UpdateTransform();
            }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                UpdateTransform();
            }
        }

        public Polygon2D(Vector2[] vertices)
        {
            _localVertices = vertices;
            _transformedVertices = new Vector2[vertices.Length];
            UpdateTransform();
        }

        protected void UpdateTransform()
        {
            var transformMatrix = Matrix.CreateRotationZ(_rotation) * Matrix.CreateScale(new Vector3(_scale, 1)) * Matrix.CreateTranslation(new Vector3(_position, 0));
            UpdateTransform(ref transformMatrix);
        }

        protected virtual void UpdateTransform(ref Matrix transformMatrix)
        {
            TransformVertices(ref transformMatrix);
        }

        protected void TransformVertices(ref Matrix transformMatrix)
        {
            for (var i = 0; i < _localVertices.Length; i++)
            {
                var vertex = _localVertices[i];
                Vector2.Transform(ref vertex, ref transformMatrix, out _transformedVertices[i]);
            }
        }

        public Vector2 Project(ref Vector2 axis)
        {
            var min = axis.Dot(_localVertices[0]);
            var max = min;

            for (var i = 1; i < _localVertices.Length; i++)
            {
                var dotProduct = axis.Dot(_localVertices[i]);

                if (dotProduct < min)
                {
                    min = dotProduct;
                }
                else if (dotProduct > max)
                {
                    max = dotProduct;
                }
            }

            return new Vector2(min, max);
        }
    }
}
