// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests
{
    public sealed class Line2DTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor()
        {
            var normal = new Vector2(0, 1);
            var distance = 5.0f;

            var line = new Line2D(normal, distance);

            Assert.Equal(normal, line.Normal);
            Assert.Equal(distance, line.Distance);
        }

        #endregion

        #region Factory Method Tests

        [Fact]
        public void CreateFromPointAndNormal()
        {
            var point = new Vector2(0, 5);
            var normal = new Vector2(0, 1);

            var line = Line2D.CreateFromPointAndNormal(point, normal);

            Assert.Equal(new Vector2(0, 1), line.Normal);
            Assert.Equal(5.0f, line.Distance, Collision2D.Epsilon);
        }

        [Fact]
        public void CreateFromTwoPoints()
        {
            var p1 = new Vector2(0, 0);
            var p2 = new Vector2(10, 0);

            var line = Line2D.CreateFromTwoPoints(p1, p2);

            Assert.Equal(0, MathF.Abs(line.Normal.X), Collision2D.Epsilon);
            Assert.Equal(1, MathF.Abs(line.Normal.Y), Collision2D.Epsilon);
        }

        [Fact]
        public void CreateFromTwoPoints_ThrowsWhenPointsTooClose()
        {
            var p1 = new Vector2(0, 0);
            var p2 = new Vector2(1e-7f, 0);

            Assert.Throws<ArgumentException>(() => Line2D.CreateFromTwoPoints(p1, p2));
        }

        [Fact]
        public void CreateFromPointAndDirection()
        {
            var point = new Vector2(5, 0);
            var direction = new Vector2(1, 0);

            var line = Line2D.CreateFromPointAndDirection(point, direction);

            Assert.Equal(0, MathF.Abs(line.Normal.X), Collision2D.Epsilon);
            Assert.Equal(1, MathF.Abs(line.Normal.Y), Collision2D.Epsilon);
        }

        #endregion

        #region Distance and Projection Tests (Delegation Spot Checks)

        [Fact]
        public void DistanceToPoint_PointOnLine()
        {
            var line = new Line2D(new Vector2(0, 1), 5);
            var point = new Vector2(0, 5);

            float distance = line.DistanceToPoint(point);

            Assert.Equal(0.0f, distance, Collision2D.Epsilon);
        }

        [Fact]
        public void DistanceToPoint_PointOffLine()
        {
            var line = new Line2D(new Vector2(0, 1), 5);
            var point = new Vector2(0, 8);

            float distance = line.DistanceToPoint(point);

            Assert.Equal(3.0f, distance, Collision2D.Epsilon);
        }

        [Fact]
        public void ClosestPoint_ReturnsProjection()
        {
            var line = new Line2D(new Vector2(0, 1), 5);
            var point = new Vector2(3, 8);

            var closest = line.ClosestPoint(point, out float distanceAlongLine);

            Assert.Equal(new Vector2(3, 5), closest);
        }

        #endregion

        #region Normalize Tests

        [Fact]
        public void Normalize_Static_CreatesUnitNormal()
        {
            var line = new Line2D(new Vector2(3, 4), 10);

            var normalized = Line2D.Normalize(line);

            Assert.Equal(1.0f, normalized.Normal.Length(), Collision2D.Epsilon);
        }

        [Fact]
        public void Normalize_StaticRef_CreatesUnitNormal()
        {
            var line = new Line2D(new Vector2(3, 4), 10);

            Line2D.Normalize(ref line, out var normalized);

            Assert.Equal(1.0f, normalized.Normal.Length(), Collision2D.Epsilon);
        }

        [Fact]
        public void Normalize_Instance_ModifiesInPlace()
        {
            var line = new Line2D(new Vector2(3, 4), 10);

            line.Normalize();

            Assert.Equal(1.0f, line.Normal.Length(), Collision2D.Epsilon);
        }

        [Fact]
        public void Normalize_AlreadyNormalized_RemainsUnchanged()
        {
            var line = new Line2D(new Vector2(0, 1), 5);
            var original = line;

            line.Normalize();

            Assert.Equal(original.Normal, line.Normal);
            Assert.Equal(original.Distance, line.Distance);
        }

        #endregion

        #region Deconstruct Test

        [Fact]
        public void Deconstruct()
        {
            var line = new Line2D(new Vector2(0, 1), 5);

            var (normal, distance) = line;

            Assert.Equal(new Vector2(0, 1), normal);
            Assert.Equal(5.0f, distance);
        }

        #endregion
    }
}
