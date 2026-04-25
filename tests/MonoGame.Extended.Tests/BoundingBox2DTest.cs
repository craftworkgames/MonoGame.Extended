// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests
{
    public sealed class BoundingBox2DTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor()
        {
            var min = new Vector2(1, 2);
            var max = new Vector2(10, 20);

            var box = new BoundingBox2D(min, max);

            Assert.Equal(min, box.Min);
            Assert.Equal(max, box.Max);
        }

        #endregion

        #region Computed Property Tests

        [Fact]
        public void Center_ReturnsMiddlePoint()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 20));

            var center = box.Center;

            Assert.Equal(new Vector2(5, 10), center);
        }

        [Fact]
        public void Size_ReturnsWidthAndHeight()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 20));

            var size = box.Size;

            Assert.Equal(new Vector2(10, 20), size);
        }

        [Fact]
        public void HalfExtents_ReturnsHalfSize()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 20));

            var halfExtents = box.HalfExtents;

            Assert.Equal(new Vector2(5, 10), halfExtents);
        }

        [Fact]
        public void Width_ReturnsHorizontalExtent()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 20));

            float width = box.Width;

            Assert.Equal(10.0f, width, Collision2D.Epsilon);
        }

        [Fact]
        public void Height_ReturnsVerticalExtent()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 20));

            float height = box.Height;

            Assert.Equal(20.0f, height, Collision2D.Epsilon);
        }

        [Fact]
        public void Area_ReturnsWidthTimesHeight()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 20));

            float area = box.Area;

            Assert.Equal(200.0f, area, Collision2D.Epsilon);
        }

        #endregion

        #region Factory Method Tests

        [Fact]
        public void CreateFromMinMax()
        {
            var min = new Vector2(1, 2);
            var max = new Vector2(10, 20);

            var box = BoundingBox2D.CreateFromMinMax(min, max);

            Assert.Equal(min, box.Min);
            Assert.Equal(max, box.Max);
        }

        [Fact]
        public void CreateFromCenterAndExtents()
        {
            var center = new Vector2(5, 10);
            var halfExtents = new Vector2(5, 10);

            var box = BoundingBox2D.CreateFromCenterAndExtents(center, halfExtents);

            Assert.Equal(new Vector2(0, 0), box.Min);
            Assert.Equal(new Vector2(10, 20), box.Max);
        }

        [Fact]
        public void CreateFromPositionAndSize()
        {
            var position = new Vector2(1, 2);
            var size = new Vector2(9, 18);

            var box = BoundingBox2D.CreateFromPositionAndSize(position, size);

            Assert.Equal(new Vector2(1, 2), box.Min);
            Assert.Equal(new Vector2(10, 20), box.Max);
        }

        [Fact]
        public void CreateFromPoints_SinglePoint()
        {
            var points = new[] { new Vector2(5, 5) };

            var box = BoundingBox2D.CreateFromPoints(points);

            Assert.Equal(new Vector2(5, 5), box.Min);
            Assert.Equal(new Vector2(5, 5), box.Max);
        }

        [Fact]
        public void CreateFromPoints_MultiplePoints()
        {
            var points = new[]
            {
                new Vector2(0, 0),
                new Vector2(10, 5),
                new Vector2(5, 15),
                new Vector2(-5, 3)
            };

            var box = BoundingBox2D.CreateFromPoints(points);

            Assert.Equal(new Vector2(-5, 0), box.Min);
            Assert.Equal(new Vector2(10, 15), box.Max);
        }

        [Fact]
        public void CreateFromPoints_ThrowsWhenNull()
        {
            Assert.Throws<ArgumentNullException>(() => BoundingBox2D.CreateFromPoints(null));
        }

        [Fact]
        public void CreateFromPoints_ThrowsWhenEmpty()
        {
            Assert.Throws<ArgumentException>(() => BoundingBox2D.CreateFromPoints(new Vector2[0]));
        }

        [Fact]
        public void CreateMerged_EnclosesBoth()
        {
            var box1 = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 10));
            var box2 = new BoundingBox2D(new Vector2(5, 5), new Vector2(15, 15));

            var merged = BoundingBox2D.CreateMerged(box1, box2);

            Assert.Equal(new Vector2(0, 0), merged.Min);
            Assert.Equal(new Vector2(15, 15), merged.Max);
        }

        [Fact]
        public void CreateMerged_OneBoxContainsOther()
        {
            var box1 = new BoundingBox2D(new Vector2(0, 0), new Vector2(20, 20));
            var box2 = new BoundingBox2D(new Vector2(5, 5), new Vector2(15, 15));

            var merged = BoundingBox2D.CreateMerged(box1, box2);

            Assert.Equal(box1.Min, merged.Min);
            Assert.Equal(box1.Max, merged.Max);
        }

        #endregion

        #region GetCorners Tests (Type-Specific Method)

        [Fact]
        public void GetCorners_ReturnsArray()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 20));

            var corners = box.GetCorners();

            Assert.Equal(4, corners.Length);
            Assert.Contains(new Vector2(0, 0), corners);
            Assert.Contains(new Vector2(10, 0), corners);
            Assert.Contains(new Vector2(10, 20), corners);
            Assert.Contains(new Vector2(0, 20), corners);
        }

        [Fact]
        public void GetCorners_FillsExistingArray()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 20));
            var corners = new Vector2[4];

            box.GetCorners(corners);

            Assert.Contains(new Vector2(0, 0), corners);
            Assert.Contains(new Vector2(10, 0), corners);
            Assert.Contains(new Vector2(10, 20), corners);
            Assert.Contains(new Vector2(0, 20), corners);
        }

        [Fact]
        public void GetCorners_ThrowsWhenArrayNull()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 20));

            Assert.Throws<ArgumentNullException>(() => box.GetCorners(null));
        }

        [Fact]
        public void GetCorners_ThrowsWhenArrayTooSmall()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 20));
            var corners = new Vector2[3];

            Assert.Throws<ArgumentException>(() => box.GetCorners(corners));
        }

        #endregion

        #region Transform Tests

        [Fact]
        public void Transform_Translation()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 10));
            var matrix = Matrix.CreateTranslation(5, 10, 0);

            var transformed = box.Transform(matrix);

            Assert.Equal(new Vector2(5, 10), transformed.Min);
            Assert.Equal(new Vector2(15, 20), transformed.Max);
        }

        [Fact]
        public void Transform_Scale()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 10));
            var matrix = Matrix.CreateScale(2.0f);

            var transformed = box.Transform(matrix);

            Assert.Equal(new Vector2(0, 0), transformed.Min);
            Assert.Equal(new Vector2(20, 20), transformed.Max);
        }

        [Fact]
        public void Transform_Rotation()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 0));
            var matrix = Matrix.CreateRotationZ(MathHelper.PiOver2);

            var transformed = box.Transform(matrix);

            Assert.Equal(0, transformed.Min.X, Collision2D.Epsilon);
            Assert.Equal(0, transformed.Min.Y, Collision2D.Epsilon);
            Assert.Equal(0, transformed.Max.X, Collision2D.Epsilon);
            Assert.Equal(10, transformed.Max.Y, Collision2D.Epsilon);
        }

        [Fact]
        public void Translate_OffsetsPosition()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 10));
            var offset = new Vector2(5, 10);

            var translated = box.Translate(offset);

            Assert.Equal(new Vector2(5, 10), translated.Min);
            Assert.Equal(new Vector2(15, 20), translated.Max);
        }

        #endregion

        #region ContainsPoint Tests (Delegation Spot Check)

        [Fact]
        public void ContainsPoint_Inside()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 10));
            var point = new Vector2(5, 5);

            var result = box.Contains(point);

            Assert.Equal(ContainmentType.Contains, result);
        }

        [Fact]
        public void ContainsPoint_OnBoundary()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 10));
            var point = new Vector2(10, 5);

            var result = box.Contains(point);

            Assert.Equal(ContainmentType.Contains, result);
        }

        [Fact]
        public void ContainsPoint_Outside()
        {
            var box = new BoundingBox2D(new Vector2(0, 0), new Vector2(10, 10));
            var point = new Vector2(15, 5);

            var result = box.Contains(point);

            Assert.Equal(ContainmentType.Disjoint, result);
        }

        #endregion

        #region TryGetCollision Tests

        [Fact]
        public void TryGetCollision_WithOverlappingBox_ReturnsReceiverMinimumTranslationVector()
        {
            BoundingBox2D box = new BoundingBox2D(new Vector2(-2.0f, -2.0f), new Vector2(2.0f, 2.0f));
            BoundingBox2D other = new BoundingBox2D(new Vector2(1.0f, -2.0f), new Vector2(5.0f, 2.0f));

            bool intersects = box.TryGetCollision(other, out CollisionResult2D result);

            Assert.True(intersects);
            Assert.True(result.Intersects);
            Assert.Equal(-Vector2.UnitX, result.Normal);
            Assert.Equal(1.0f, result.PenetrationDepth);
            Assert.Equal(new Vector2(-1.0f, 0.0f), result.MinimumTranslationVector);
        }

        [Fact]
        public void TryGetCollision_WithSeparatedBox_ReturnsFalseAndNone()
        {
            BoundingBox2D box = new BoundingBox2D(new Vector2(-1.0f, -1.0f), new Vector2(1.0f, 1.0f));
            BoundingBox2D other = new BoundingBox2D(new Vector2(4.0f, -1.0f), new Vector2(6.0f, 1.0f));

            bool intersects = box.TryGetCollision(other, out CollisionResult2D result);

            Assert.False(intersects);
            Assert.False(result.Intersects);
            Assert.Equal(CollisionResult2D.None, result);
        }

        [Fact]
        public void TryGetCollision_WithCircle_ReturnsReceiverMinimumTranslationVector()
        {
            BoundingBox2D box = new BoundingBox2D(new Vector2(-2.0f, -2.0f), new Vector2(2.0f, 2.0f));
            BoundingCircle2D circle = new BoundingCircle2D(new Vector2(3.0f, 0.0f), 2.0f);

            bool intersects = box.TryGetCollision(circle, out CollisionResult2D result);

            Assert.True(intersects);
            Assert.True(result.Intersects);
            Assert.Equal(-Vector2.UnitX, result.Normal);
            Assert.Equal(1.0f, result.PenetrationDepth);
            Assert.Equal(new Vector2(-1.0f, 0.0f), result.MinimumTranslationVector);
        }

        [Fact]
        public void TryGetCollision_WithOrientedBox_ReturnsReceiverMinimumTranslationVector()
        {
            BoundingBox2D box = new BoundingBox2D(new Vector2(-2.0f, -2.0f), new Vector2(2.0f, 2.0f));
            OrientedBoundingBox2D obb = new OrientedBoundingBox2D(
                new Vector2(3.0f, 0.0f),
                Vector2.UnitX,
                Vector2.UnitY,
                new Vector2(2.0f, 2.0f));

            bool intersects = box.TryGetCollision(obb, out CollisionResult2D result);

            Assert.True(intersects);
            Assert.True(result.Intersects);
            Assert.Equal(-Vector2.UnitX, result.Normal);
            Assert.Equal(1.0f, result.PenetrationDepth);
            Assert.Equal(new Vector2(-1.0f, 0.0f), result.MinimumTranslationVector);
        }

        [Fact]
        public void TryGetCollision_WithPolygon_ReturnsReceiverMinimumTranslationVector()
        {
            BoundingBox2D box = new BoundingBox2D(new Vector2(-2.0f, -2.0f), new Vector2(2.0f, 2.0f));
            Vector2[] vertices =
            {
                new Vector2(1.0f, -2.0f),
                new Vector2(5.0f, -2.0f),
                new Vector2(5.0f, 2.0f),
                new Vector2(1.0f, 2.0f)
            };
            Vector2[] normals =
            {
                -Vector2.UnitY,
                Vector2.UnitX,
                Vector2.UnitY,
                -Vector2.UnitX
            };
            BoundingPolygon2D polygon = new BoundingPolygon2D(vertices, normals);

            bool intersects = box.TryGetCollision(polygon, out CollisionResult2D result);

            Assert.True(intersects);
            Assert.True(result.Intersects);
            Assert.Equal(-Vector2.UnitX, result.Normal);
            Assert.Equal(1.0f, result.PenetrationDepth);
            Assert.Equal(new Vector2(-1.0f, 0.0f), result.MinimumTranslationVector);
        }

        #endregion

        #region Deconstruct Test

        [Fact]
        public void Deconstruct()
        {
            var box = new BoundingBox2D(new Vector2(1, 2), new Vector2(10, 20));

            var (min, max) = box;

            Assert.Equal(new Vector2(1, 2), min);
            Assert.Equal(new Vector2(10, 20), max);
        }

        #endregion
    }
}
