// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests
{
    public sealed class BoundingCircleTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor()
        {
            var center = new Vector2(5, 10);
            var radius = 15.0f;

            var circle = new BoundingCircle2D(center, radius);

            Assert.Equal(center, circle.Center);
            Assert.Equal(radius, circle.Radius);
        }

        #endregion

        #region Computed Property Tests

        [Fact]
        public void RadiusSquared_ReturnsSquaredValue()
        {
            var circle = new BoundingCircle2D(new Vector2(0, 0), 5.0f);

            float radiusSquared = circle.RadiusSquared;

            Assert.Equal(25.0f, radiusSquared, Collision2D.Epsilon);
        }

        [Fact]
        public void Diameter_ReturnsTwiceRadius()
        {
            var circle = new BoundingCircle2D(new Vector2(0, 0), 5.0f);

            float diameter = circle.Diameter;

            Assert.Equal(10.0f, diameter, Collision2D.Epsilon);
        }

        [Fact]
        public void Area_ReturnsPiTimesRadiusSquared()
        {
            var circle = new BoundingCircle2D(new Vector2(0, 0), 5.0f);

            float area = circle.Area;

            Assert.Equal(MathF.PI * 25.0f, area, Collision2D.Epsilon);
        }

        #endregion

        #region Factory Method Tests

        [Fact]
        public void CreateFromPoints_SinglePoint()
        {
            var points = new[] { new Vector2(5, 5) };

            var circle = BoundingCircle2D.CreateFromPoints(points);

            Assert.Equal(new Vector2(5, 5), circle.Center);
            Assert.Equal(0.0f, circle.Radius, Collision2D.Epsilon);
        }

        [Fact]
        public void CreateFromPoints_TwoPoints()
        {
            var points = new[] { new Vector2(0, 0), new Vector2(10, 0) };

            var circle = BoundingCircle2D.CreateFromPoints(points);

            Assert.Equal(new Vector2(5, 0), circle.Center);
            Assert.Equal(5.0f, circle.Radius, Collision2D.Epsilon);
        }

        [Fact]
        public void CreateFromPoints_MultiplePoints()
        {
            var points = new[]
            {
                new Vector2(0, 0),
                new Vector2(10, 0),
                new Vector2(0, 10),
                new Vector2(10, 10)
            };

            var circle = BoundingCircle2D.CreateFromPoints(points);

            foreach (var point in points)
            {
                float distance = Vector2.Distance(circle.Center, point);
                Assert.True(distance <= circle.Radius + Collision2D.Epsilon);
            }
        }

        [Fact]
        public void CreateFromPoints_ThrowsWhenNull()
        {
            Assert.Throws<ArgumentNullException>(() => BoundingCircle2D.CreateFromPoints(null));
        }

        [Fact]
        public void CreateFromPoints_ThrowsWhenEmpty()
        {
            Assert.Throws<ArgumentException>(() => BoundingCircle2D.CreateFromPoints(new Vector2[0]));
        }

        [Fact]
        public void CreateFromBoundingBox2D()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 10));

            var circle = BoundingCircle2D.CreateFromBoundingBox2D(box);

            Assert.True(circle.Contains(new Vector2(0, 0)) != ContainmentType.Disjoint);
            Assert.True(circle.Contains(new Vector2(10, 10)) != ContainmentType.Disjoint);
        }

        [Fact]
        public void CreateFromBoundingCapsule2D_HorizontalCapsule()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 5), new Vector2(10, 5), 3.0f);

            var circle = BoundingCircle2D.CreateFromBoundingCapsule2D(capsule);

            Assert.True(circle.Contains(new Vector2(-3, 5)) != ContainmentType.Disjoint);
            Assert.True(circle.Contains(new Vector2(13, 5)) != ContainmentType.Disjoint);
        }

        [Fact]
        public void CreateFromBoundingCapsule2D_VerticalCapsule()
        {
            var capsule = new BoundingCapsule2D(new Vector2(5, 0), new Vector2(5, 10), 3.0f);

            var circle = BoundingCircle2D.CreateFromBoundingCapsule2D(capsule);

            Assert.True(circle.Contains(new Vector2(5, -3)) != ContainmentType.Disjoint);
            Assert.True(circle.Contains(new Vector2(5, 13)) != ContainmentType.Disjoint);
        }

        [Fact]
        public void CreateFromBoundingCapsule2D_DegenerateCapsule()
        {
            var capsule = new BoundingCapsule2D(new Vector2(5, 5), new Vector2(5, 5), 3.0f);

            var circle = BoundingCircle2D.CreateFromBoundingCapsule2D(capsule);

            Assert.Equal(new Vector2(5, 5), circle.Center);
            Assert.Equal(3.0f, circle.Radius, Collision2D.Epsilon);
        }

        [Fact]
        public void CreateFromBoundingCapsule2D_ContainsOriginal()
        {
            var capsule = new BoundingCapsule2D(new Vector2(0, 0), new Vector2(10, 10), 2.0f);

            var circle = BoundingCircle2D.CreateFromBoundingCapsule2D(capsule);

            Assert.Equal(ContainmentType.Contains, circle.Contains(capsule));
        }

        [Fact]
        public void CreateMerged_NonOverlapping()
        {
            var circle1 = new BoundingCircle2D(new Vector2(0, 0), 5.0f);
            var circle2 = new BoundingCircle2D(new Vector2(20, 0), 5.0f);

            var merged = BoundingCircle2D.CreateMerged(circle1, circle2);

            Assert.Equal(ContainmentType.Contains, merged.Contains(circle1));
            Assert.Equal(ContainmentType.Contains, merged.Contains(circle2));
        }

        [Fact]
        public void CreateMerged_OneContainsOther()
        {
            var circle1 = new BoundingCircle2D(new Vector2(0, 0), 10.0f);
            var circle2 = new BoundingCircle2D(new Vector2(2, 2), 3.0f);

            var merged = BoundingCircle2D.CreateMerged(circle1, circle2);

            Assert.Equal(circle1.Center, merged.Center);
            Assert.Equal(circle1.Radius, merged.Radius, 0.1f);
        }

        [Fact]
        public void CreateMerged_PartiallyOverlapping()
        {
            var circle1 = new BoundingCircle2D(new Vector2(0, 0), 5.0f);
            var circle2 = new BoundingCircle2D(new Vector2(8, 0), 5.0f);

            var merged = BoundingCircle2D.CreateMerged(circle1, circle2);

            Assert.Equal(ContainmentType.Contains, merged.Contains(circle1));
            Assert.Equal(ContainmentType.Contains, merged.Contains(circle2));
        }

        #endregion

        #region Transform Tests

        [Fact]
        public void Transform_Translation()
        {
            var circle = new BoundingCircle2D(new Vector2(0, 0), 5.0f);
            var matrix = Matrix.CreateTranslation(10, 20, 0);

            var transformed = circle.Transform(matrix);

            Assert.Equal(new Vector2(10, 20), transformed.Center);
            Assert.Equal(5.0f, transformed.Radius, Collision2D.Epsilon);
        }

        [Fact]
        public void Transform_UniformScale()
        {
            var circle = new BoundingCircle2D(new Vector2(0, 0), 5.0f);
            var matrix = Matrix.CreateScale(2.0f);

            var transformed = circle.Transform(matrix);

            Assert.Equal(new Vector2(0, 0), transformed.Center);
            Assert.Equal(10.0f, transformed.Radius, Collision2D.Epsilon);
        }

        [Fact]
        public void Transform_NonUniformScale()
        {
            var circle = new BoundingCircle2D(new Vector2(0, 0), 5.0f);
            var matrix = Matrix.CreateScale(2.0f, 3.0f, 1.0f);

            var transformed = circle.Transform(matrix);

            Assert.True(transformed.Radius >= 10.0f);
        }

        [Fact]
        public void Transform_Rotation()
        {
            var circle = new BoundingCircle2D(new Vector2(5, 0), 3.0f);
            var matrix = Matrix.CreateRotationZ(MathHelper.PiOver2);

            var transformed = circle.Transform(matrix);

            Assert.Equal(0, transformed.Center.X, Collision2D.Epsilon);
            Assert.Equal(5, transformed.Center.Y, Collision2D.Epsilon);
            Assert.Equal(3.0f, transformed.Radius, Collision2D.Epsilon);
        }

        [Fact]
        public void Translate_OffsetsPosition()
        {
            var circle = new BoundingCircle2D(new Vector2(5, 5), 3.0f);
            var offset = new Vector2(10, 15);

            var translated = circle.Translate(offset);

            Assert.Equal(new Vector2(15, 20), translated.Center);
            Assert.Equal(3.0f, translated.Radius, Collision2D.Epsilon);
        }

        #endregion

        #region ContainsPoint Tests (Delegation Spot Check)

        [Fact]
        public void ContainsPoint_Inside()
        {
            var circle = new BoundingCircle2D(new Vector2(0, 0), 10.0f);
            var point = new Vector2(5, 0);

            var result = circle.Contains(point);

            Assert.Equal(ContainmentType.Contains, result);
        }

        [Fact]
        public void ContainsPoint_OnBoundary()
        {
            var circle = new BoundingCircle2D(new Vector2(0, 0), 10.0f);
            var point = new Vector2(10, 0);

            var result = circle.Contains(point);

            Assert.Equal(ContainmentType.Contains, result);
        }

        [Fact]
        public void ContainsPoint_Outside()
        {
            var circle = new BoundingCircle2D(new Vector2(0, 0), 10.0f);
            var point = new Vector2(15, 0);

            var result = circle.Contains(point);

            Assert.Equal(ContainmentType.Disjoint, result);
        }

        #endregion

        #region Deconstruct Test

        [Fact]
        public void Deconstruct()
        {
            var circle = new BoundingCircle2D(new Vector2(5, 10), 15.0f);

            var (center, radius) = circle;

            Assert.Equal(new Vector2(5, 10), center);
            Assert.Equal(15.0f, radius);
        }

        #endregion
    }
}
