// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests
{
    public sealed class BoundingCapsule2DTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor()
        {
            var pointA = new Vector2(0, 0);
            var pointB = new Vector2(10, 0);
            var radius = 5.0f;

            var capsule = new BoundingCapsule2D(pointA, pointB, radius);

            Assert.Equal(pointA, capsule.PointA);
            Assert.Equal(pointB, capsule.PointB);
            Assert.Equal(radius, capsule.Radius);
        }

        #endregion

        #region Computed Property Tests

        [Fact]
        public void Center_ReturnsMidpoint()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 5.0f);

            var center = capsule.Center;

            Assert.Equal(new Vector2(5, 0), center);
        }

        [Fact]
        public void Length_ReturnsDistance()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(3, 4), 5.0f);

            float length = capsule.Length;

            Assert.Equal(5.0f, length, Collision2D.Epsilon);
        }

        [Fact]
        public void LengthSquared_ReturnsSquaredDistance()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(3, 4), 5.0f);

            float lengthSquared = capsule.LengthSquared;

            Assert.Equal(25.0f, lengthSquared, Collision2D.Epsilon);
        }

        [Fact]
        public void Direction_ReturnsUnitVector()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(3, 4), 5.0f);

            var direction = capsule.Direction;

            Assert.Equal(1.0f, direction.Length(), Collision2D.Epsilon);
            Assert.Equal(0.6f, direction.X, Collision2D.Epsilon);
            Assert.Equal(0.8f, direction.Y, Collision2D.Epsilon);
        }

        [Fact]
        public void Direction_DegenerateCapsule()
        {
            var capsule = new BoundingCapsule2D(new Vector2(5, 5), new Vector2(5, 5), 3.0f);

            var direction = capsule.Direction;

            Assert.Equal(Vector2.Zero, direction);
        }

        [Fact]
        public void Area_CalculatesCorrectly()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 5.0f);

            float area = capsule.Area;

            float expected = 100.0f + MathF.PI * 25.0f;
            Assert.Equal(expected, area, Collision2D.Epsilon);
        }

        #endregion

        #region Factory Method Tests

        [Fact]
        public void CreateFromCenterAndDirection()
        {
            var center = new Vector2(5, 5);
            var direction = new Vector2(1, 0);
            var length = 10.0f;
            var radius = 3.0f;

            var capsule = BoundingCapsule2D.CreateFromCenterAndDirection(center, direction, length, radius);

            Assert.Equal(new Vector2(0, 5), capsule.PointA);
            Assert.Equal(new Vector2(10, 5), capsule.PointB);
            Assert.Equal(radius, capsule.Radius);
        }

        [Fact]
        public void CreateFromCenterAndDirection_UnnormalizedDirection()
        {
            var center = new Vector2(5, 5);
            var direction = new Vector2(3, 4);
            var length = 10.0f;
            var radius = 3.0f;

            var capsule = BoundingCapsule2D.CreateFromCenterAndDirection(center, direction, length, radius);

            float distance = Vector2.Distance(capsule.PointA, capsule.PointB);
            Assert.Equal(10.0f, distance, Collision2D.Epsilon);
        }

        [Fact]
        public void CreateFromSegment()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));
            var radius = 3.0f;

            var capsule = BoundingCapsule2D.CreateFromSegment(segment, radius);

            Assert.Equal(segment.Start, capsule.PointA);
            Assert.Equal(segment.End, capsule.PointB);
            Assert.Equal(radius, capsule.Radius);
        }

        [Fact]
        public void CreateMerged_NonOverlapping()
        {
            var capsule1 = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(5, 0), 2.0f);
            var capsule2 = new BoundingCapsule2D(new Vector2(20, 0), new Vector2(25, 0), 2.0f);

            var merged = BoundingCapsule2D.CreateMerged(capsule1, capsule2);

            Assert.Equal(ContainmentType.Contains, merged.Contains(capsule1));
            Assert.Equal(ContainmentType.Contains, merged.Contains(capsule2));
        }

        [Fact]
        public void CreateMerged_OneContainsOther()
        {
            var capsule1 = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(20, 0), 5.0f);
            var capsule2 = new BoundingCapsule2D(new Vector2(8, 0), new Vector2(12, 0), 2.0f);

            var merged = BoundingCapsule2D.CreateMerged(capsule1, capsule2);

            Assert.Equal(capsule1.PointA.X, merged.PointA.X, Collision2D.Epsilon);
            Assert.Equal(capsule1.PointA.Y, merged.PointA.Y, Collision2D.Epsilon);
            Assert.Equal(capsule1.PointB.X, merged.PointB.X, Collision2D.Epsilon);
            Assert.Equal(capsule1.PointB.Y, merged.PointB.Y, Collision2D.Epsilon);
        }

        [Fact]
        public void CreateMerged_PartiallyOverlapping()
        {
            var capsule1 = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 3.0f);
            var capsule2 = new BoundingCapsule2D(new Vector2(8, 0), new Vector2(18, 0), 3.0f);

            var merged = BoundingCapsule2D.CreateMerged(capsule1, capsule2);

            Assert.Equal(ContainmentType.Contains, merged.Contains(capsule1));
            Assert.Equal(ContainmentType.Contains, merged.Contains(capsule2));
        }

        #endregion

        #region Transform Tests

        [Fact]
        public void Transform_Translation()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 3.0f);
            var matrix = Matrix.CreateTranslation(5, 10, 0);

            var transformed = capsule.Transform(matrix);

            Assert.Equal(new Vector2(5, 10), transformed.PointA);
            Assert.Equal(new Vector2(15, 10), transformed.PointB);
            Assert.Equal(3.0f, transformed.Radius, Collision2D.Epsilon);
        }

        [Fact]
        public void Transform_UniformScale()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 3.0f);
            var matrix = Matrix.CreateScale(2.0f);

            var transformed = capsule.Transform(matrix);

            Assert.Equal(new Vector2(0, 0), transformed.PointA);
            Assert.Equal(new Vector2(20, 0), transformed.PointB);
            Assert.Equal(6.0f, transformed.Radius, Collision2D.Epsilon);
        }

        [Fact]
        public void Transform_NonUniformScale()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 3.0f);
            var matrix = Matrix.CreateScale(2.0f, 3.0f, 1.0f);

            var transformed = capsule.Transform(matrix);

            Assert.Equal(new Vector2(0, 0), transformed.PointA);
            Assert.Equal(new Vector2(20, 0), transformed.PointB);
            Assert.True(transformed.Radius >= 6.0f);
        }

        [Fact]
        public void Transform_Rotation()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 3.0f);
            var matrix = Matrix.CreateRotationZ(MathHelper.PiOver2);

            var transformed = capsule.Transform(matrix);

            Assert.Equal(0, transformed.PointA.X, Collision2D.Epsilon);
            Assert.Equal(0, transformed.PointA.Y, Collision2D.Epsilon);
            Assert.Equal(0, transformed.PointB.X, Collision2D.Epsilon);
            Assert.Equal(10, transformed.PointB.Y, Collision2D.Epsilon);
            Assert.Equal(3.0f, transformed.Radius, Collision2D.Epsilon);
        }

        [Fact]
        public void Translate_OffsetsPosition()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 3.0f);
            var offset = new Vector2(5, 10);

            var translated = capsule.Translate(offset);

            Assert.Equal(new Vector2(5, 10), translated.PointA);
            Assert.Equal(new Vector2(15, 10), translated.PointB);
            Assert.Equal(3.0f, translated.Radius, Collision2D.Epsilon);
        }

        #endregion

        #region ContainsPoint Tests (Delegation Spot Check)

        [Fact]
        public void ContainsPoint_Inside()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 5.0f);
            var point = new Vector2(5, 2);

            var result = capsule.Contains(point);

            Assert.Equal(ContainmentType.Contains, result);
        }

        [Fact]
        public void ContainsPoint_OnBoundary()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 5.0f);
            var point = new Vector2(5, 5);

            var result = capsule.Contains(point);

            Assert.Equal(ContainmentType.Contains, result);
        }

        [Fact]
        public void ContainsPoint_Outside()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 5.0f);
            var point = new Vector2(5, 10);

            var result = capsule.Contains(point);

            Assert.Equal(ContainmentType.Disjoint, result);
        }

        [Fact]
        public void ContainsPoint_AtEndCap()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 5.0f);
            var point = new Vector2(-3, 4);

            var result = capsule.Contains(point);

            Assert.Equal(ContainmentType.Contains, result);
        }

        #endregion

        #region TryGetCollision Tests

        [Fact]
        public void TryGetCollision_WithCircle_ReturnsReceiverMinimumTranslationVector()
        {
            BoundingCapsule2D capsule = new BoundingCapsule2D(new Vector2(3.0f, -2.0f), new Vector2(3.0f, 2.0f), 2.0f);
            BoundingCircle2D circle = new BoundingCircle2D(Vector2.Zero, 2.0f);

            bool intersects = capsule.TryGetCollision(circle, out CollisionResult2D result);

            Assert.True(intersects);
            Assert.True(result.Intersects);
            Assert.Equal(Vector2.UnitX, result.Normal);
            Assert.Equal(1.0f, result.PenetrationDepth);
            Assert.Equal(new Vector2(1.0f, 0.0f), result.MinimumTranslationVector);
        }

        [Fact]
        public void TryGetCollision_WithSeparatedCircle_ReturnsFalseAndNone()
        {
            BoundingCapsule2D capsule = new BoundingCapsule2D(new Vector2(4.0f, -2.0f), new Vector2(4.0f, 2.0f), 1.0f);
            BoundingCircle2D circle = new BoundingCircle2D(Vector2.Zero, 1.0f);

            bool intersects = capsule.TryGetCollision(circle, out CollisionResult2D result);

            Assert.False(intersects);
            Assert.False(result.Intersects);
            Assert.Equal(CollisionResult2D.None, result);
        }

        #endregion

        #region Deconstruct Test

        [Fact]
        public void Deconstruct()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 0), 5.0f);

            var (pointA, pointB, radius) = capsule;

            Assert.Equal(new Vector2(0, 0), pointA);
            Assert.Equal(new Vector2(10, 0), pointB);
            Assert.Equal(5.0f, radius);
        }

        #endregion
    }
}
