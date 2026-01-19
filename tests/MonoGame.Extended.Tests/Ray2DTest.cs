// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests
{
    public sealed class Ray2DTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor()
        {
            var origin = new Vector2(1, 2);
            var direction = new Vector2(3, 4);

            var ray = new Ray2D(origin, direction);

            Assert.Equal(origin, ray.Origin);
            Assert.Equal(direction, ray.Direction);
        }

        #endregion

        #region Factory Method Tests

        [Fact]
        public void CreateFromPoints()
        {
            var start = new Vector2(0, 0);
            var through = new Vector2(10, 0);

            var ray = Ray2D.CreateFromPoints(start, through);

            Assert.Equal(start, ray.Origin);
            Assert.Equal(new Vector2(1, 0), ray.Direction);
        }

        [Fact]
        public void CreateFromPoints_ThrowsWhenPointsTooClose()
        {
            var start = new Vector2(0, 0);
            var through = new Vector2(1e-7f, 0);

            Assert.Throws<ArgumentException>(() => Ray2D.CreateFromPoints(start, through));
        }

        #endregion

        #region GetPoint Tests (Type-Specific Utility)

        [Fact]
        public void GetPoint_AtOrigin()
        {
            var ray = new Ray2D(new Vector2(5, 5), new Vector2(1, 0));

            var point = ray.GetPoint(0);

            Assert.Equal(new Vector2(5, 5), point);
        }

        [Fact]
        public void GetPoint_ForwardDirection()
        {
            var ray = new Ray2D(new Vector2(0, 0), new Vector2(1, 0));

            var point = ray.GetPoint(10);

            Assert.Equal(new Vector2(10, 0), point);
        }

        [Fact]
        public void GetPoint_NegativeDistance()
        {
            var ray = new Ray2D(new Vector2(10, 0), new Vector2(1, 0));

            var point = ray.GetPoint(-5);

            Assert.Equal(new Vector2(5, 0), point);
        }

        #endregion

        #region Distance and Projection Tests (Delegation Spot Checks)

        [Fact]
        public void ClosestPoint_PointAhead()
        {
            var ray = new Ray2D(new Vector2(0, 0), new Vector2(1, 0));
            var point = new Vector2(5, 3);

            var closest = ray.ClosestPoint(point, out float distanceAlongRay);

            Assert.Equal(new Vector2(5, 0), closest);
        }

        [Fact]
        public void ClosestPoint_PointBehind()
        {
            var ray = new Ray2D(new Vector2(10, 0), new Vector2(1, 0));
            var point = new Vector2(5, 3);

            var closest = ray.ClosestPoint(point, out float distanceAlongRay);

            Assert.Equal(new Vector2(10, 0), closest);
        }

        [Fact]
        public void DistanceToPoint_PerpendicularDistance()
        {
            var ray = new Ray2D(new Vector2(0, 0), new Vector2(1, 0));
            var point = new Vector2(5, 3);

            float distance = ray.DistanceToPoint(point);

            Assert.Equal(3.0f, distance, Collision2D.Epsilon);
        }

        [Fact]
        public void DistanceSquaredToPoint_AvoidsSqrt()
        {
            var ray = new Ray2D(new Vector2(0, 0), new Vector2(1, 0));
            var point = new Vector2(5, 3);

            float distanceSquared = ray.DistanceSquaredToPoint(point);

            Assert.Equal(9.0f, distanceSquared, Collision2D.Epsilon);
        }

        #endregion

        #region Normalize Tests

        [Fact]
        public void Normalize_Static_CreatesUnitDirection()
        {
            var ray = new Ray2D(new Vector2(0, 0), new Vector2(3, 4));

            var normalized = Ray2D.Normalize(ray);

            Assert.Equal(1.0f, normalized.Direction.Length(), Collision2D.Epsilon);
            Assert.Equal(ray.Origin, normalized.Origin);
        }

        [Fact]
        public void Normalize_Instance_ModifiesInPlace()
        {
            var ray = new Ray2D(new Vector2(5, 5), new Vector2(3, 4));

            ray.Normalize();

            Assert.Equal(1.0f, ray.Direction.Length(), Collision2D.Epsilon);
            Assert.Equal(new Vector2(5, 5), ray.Origin);
        }

        #endregion

        #region Deconstruct Test

        [Fact]
        public void Deconstruct()
        {
            var ray = new Ray2D(new Vector2(1, 2), new Vector2(3, 4));

            var (origin, direction) = ray;

            Assert.Equal(new Vector2(1, 2), origin);
            Assert.Equal(new Vector2(3, 4), direction);
        }

        #endregion
    }
}
