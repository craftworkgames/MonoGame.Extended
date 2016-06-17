using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes.Explicit;
using MonoGame.Extended.Shapes.BoundingVolumes;

namespace MonoGame.Extended.Collisions
{
    public partial class CollisionShape2D : Polygon2D
    {
        private AxisAlignedBoundingBox2D _boundingVolume;
        private readonly Vector2[] _localNormals;
        private readonly Vector2[] _transformedNormals;

        internal Action _update;

        public IReadOnlyList<Vector2> LocalNormals
        {
            get { return _localNormals; }
        }

        public IReadOnlyList<Vector2> TransformedNormals
        {
            get { return _transformedNormals; }
        }

        internal CollisionShape2D(Action update, Vector2[] vertices)
            : base(vertices)
        {
            _update = update;
            _boundingVolume = new AxisAlignedBoundingBox2D();
            _localNormals = new Vector2[vertices.Length];
            _transformedNormals = new Vector2[vertices.Length];
            UpdateTransform();
        }

        protected override void UpdateTransform(ref Matrix transformMatrix)
        {
            TransformVertices(ref transformMatrix);

            _boundingVolume.Compute(TransformedVertices);

            if (_localNormals == null)
            {
                return;
            }

            TransformNormals(ref transformMatrix);
        }

        private void TransformNormals(ref Matrix transformMatrix)
        {
            for (var i = 0; i < _localNormals.Length; i++)
            {
                var normal = _localNormals[i];
                Vector2.TransformNormal(ref normal, ref transformMatrix, out _transformedNormals[i]);
            }
        }
    }
}
