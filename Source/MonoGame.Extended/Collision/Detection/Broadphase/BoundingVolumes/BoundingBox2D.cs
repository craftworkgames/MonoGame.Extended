using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collision.Detection.Broadphase.BoundingVolumes
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.1; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 77-87
    public sealed class BoundingBox2D : BoundingVolume2D
    {
        // Use center-radius representation; cheaper to transform than min-max representation.
        private Vector2 _centre;
        private SizeF _radius;

        public Vector2 Centre
        {
            get { return _centre; }
            set { _centre = value; }
        }

        public SizeF Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public BoundingBox2D()
            : base(BoundingVolumeType2D.BoundingBox)
        {    
        }

        public BoundingBox2D(Vector2 centre, Vector2 radius)
            : this()
        {
            Centre = centre;
            Radius = radius;
        }

        public override bool Intersects(Ray ray, out float rayNearDistance, out float rayFarDistance)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.3; Basic Primitive Tests - Intersecting Lines, Rays, and (Directed Segments). pg 179-181

            var minimum = _centre - _radius;
            var maximum = _centre + _radius;
            var rayMinimumDistance = float.MinValue;
            var rayMaximumDistance = float.MaxValue;

            if (!RayHelper.RayIntersectsSlab(ray.Position.X, ray.Direction.X, minimum.X, maximum.X, ref rayMinimumDistance, ref rayMaximumDistance))
            {
                rayNearDistance = rayFarDistance = 0;
                return false;
            }

            if (!RayHelper.RayIntersectsSlab(ray.Position.Y, ray.Direction.Y, minimum.Y, maximum.Y, ref rayMinimumDistance, ref rayMaximumDistance))
            {
                rayNearDistance = rayFarDistance = 0;
                return false;
            }

            // Ray intersects all 3 slabs.
            rayNearDistance = rayMinimumDistance;
            rayFarDistance = rayMaximumDistance;
            return true;
        }

        public override bool Intersects(BoundingVolume2D boundingVolume)
        {
            var axisAlignedBoundingVolume = boundingVolume as BoundingBox2D;
            if (axisAlignedBoundingVolume != null)
            {
                return Intersects(axisAlignedBoundingVolume);
            }

            return false;
        }

        public bool Intersects(BoundingBox2D axisAlignedBoundingBox)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 80
            var distance = Centre - axisAlignedBoundingBox.Centre;
            var radius = new SizeF(_radius.Width + axisAlignedBoundingBox._radius.Width, _radius.Height + axisAlignedBoundingBox._radius.Height);
            return Math.Abs(distance.X) <= radius.Width && Math.Abs(distance.Y) <= radius.Height;
        }

        public override bool Contains(Vector2 point)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 78
            var distance = Centre - point;
            return Math.Abs(distance.X) <= _radius.Width && Math.Abs(distance.Y) <= _radius.Height;
        }

        public void UpdateFrom(Vector2 minimum, Vector2 maximum)
        {
            Centre = (maximum + minimum) * 0.5f;
            Radius = (maximum - minimum) * 0.5f;
        }

        public override void UpdateFrom(IReadOnlyList<Vector2> vertices)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 82-84
            // Alternate version for tight axis-aligned bounding box.

            var minimum = new Vector2(float.MaxValue);
            var maximum = new Vector2(float.MinValue);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < vertices.Count; ++index)
            {
                var vertex = vertices[index];
                Vector2.Min(ref minimum, ref vertex, out minimum);
                Vector2.Max(ref maximum, ref vertex, out maximum);
            }

            UpdateFrom(minimum, maximum);
        }

        public override void UpdateFrom(BoundingVolume2D boundingVolume, ref Matrix2D localToWorldMatrix)
        {
            switch (boundingVolume.Type)
            {
                case BoundingVolumeType2D.BoundingBox:
                    UpdateFrom((BoundingBox2D)boundingVolume, ref localToWorldMatrix);
                    return;
                default:
                    throw new InvalidOperationException();
            }
        }

        public void UpdateFrom(BoundingBox2D axisAlignedBoundingBox, ref Matrix2D localToWorldMatrix)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 86-87
            _centre.X = localToWorldMatrix.M31;
            _centre.Y = localToWorldMatrix.M32;

            var radius = axisAlignedBoundingBox._radius;
            var width = radius.Width * Math.Abs(localToWorldMatrix.M11) + radius.Height * Math.Abs(localToWorldMatrix.M12);
            var height = radius.Width * Math.Abs(localToWorldMatrix.M21) + radius.Height * Math.Abs(localToWorldMatrix.M22);
            _radius = new SizeF(width, height);
        }

        internal string DebugDisplayString => $"Centre = {Centre}, Radius = {Radius}";

        public override string ToString()
        {
            return $"{{Centre = {Centre}, Radius = {Radius}}}";
        }
    }
}
