// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests
{
    public sealed class LineSegment2DTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor()
        {
            var start = new Vector2(1, 2);
            var end = new Vector2(3, 4);

            var segment = new LineSegment2D(start, end);

            Assert.Equal(start, segment.Start);
            Assert.Equal(end, segment.End);
        }

        #endregion

        #region Computed Property Tests

        [Fact]
        public void Direction_ReturnsVector()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(3, 4));

            var direction = segment.Direction;

            Assert.Equal(new Vector2(3, 4), direction);
        }

        [Fact]
        public void Midpoint_ReturnsCenterPoint()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 10));

            var midpoint = segment.Midpoint;

            Assert.Equal(new Vector2(5, 5), midpoint);
        }

        [Fact]
        public void Length_Horizontal()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));

            float length = segment.Length;

            Assert.Equal(10.0f, length, Collision2D.Epsilon);
        }

        [Fact]
        public void Length_Diagonal()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(3, 4));

            float length = segment.Length;

            Assert.Equal(5.0f, length, Collision2D.Epsilon);
        }

        [Fact]
        public void LengthSquared_AvoidsSqrt()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(3, 4));

            float lengthSquared = segment.LengthSquared;

            Assert.Equal(25.0f, lengthSquared, Collision2D.Epsilon);
        }

        #endregion

        #region GetPoint Tests (Type-Specific Utility)

        [Fact]
        public void GetPoint_AtStart()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));

            var point = segment.GetPoint(0.0f);

            Assert.Equal(new Vector2(0, 0), point);
        }

        [Fact]
        public void GetPoint_AtMidpoint()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));

            var point = segment.GetPoint(0.5f);

            Assert.Equal(new Vector2(5, 0), point);
        }

        [Fact]
        public void GetPoint_AtEnd()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));

            var point = segment.GetPoint(1.0f);

            Assert.Equal(new Vector2(10, 0), point);
        }

        [Fact]
        public void GetPoint_BeyondEnd()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));

            var point = segment.GetPoint(1.5f);

            Assert.Equal(new Vector2(15, 0), point);
        }

        #endregion

        #region GetBounds Tests (Type-Specific Method)

        [Fact]
        public void GetBounds_HorizontalSegment()
        {
            var segment = new LineSegment2D(new Vector2(2, 5), new Vector2(8, 5));

            var bounds = segment.GetBounds();

            Assert.Equal(new Vector2(2, 5), bounds.Min);
            Assert.Equal(new Vector2(8, 5), bounds.Max);
        }

        [Fact]
        public void GetBounds_VerticalSegment()
        {
            var segment = new LineSegment2D(new Vector2(5, 2), new Vector2(5, 8));

            var bounds = segment.GetBounds();

            Assert.Equal(new Vector2(5, 2), bounds.Min);
            Assert.Equal(new Vector2(5, 8), bounds.Max);
        }

        [Fact]
        public void GetBounds_DiagonalSegment()
        {
            var segment = new LineSegment2D(new Vector2(1, 2), new Vector2(9, 7));

            var bounds = segment.GetBounds();

            Assert.Equal(new Vector2(1, 2), bounds.Min);
            Assert.Equal(new Vector2(9, 7), bounds.Max);
        }

        [Fact]
        public void GetBounds_ReversedEndpoints()
        {
            var segment = new LineSegment2D(new Vector2(9, 7), new Vector2(1, 2));

            var bounds = segment.GetBounds();

            Assert.Equal(new Vector2(1, 2), bounds.Min);
            Assert.Equal(new Vector2(9, 7), bounds.Max);
        }

        [Fact]
        public void GetBounds_DegenerateSegment()
        {
            var segment = new LineSegment2D(new Vector2(5, 5), new Vector2(5, 5));

            var bounds = segment.GetBounds();

            Assert.Equal(new Vector2(5, 5), bounds.Min);
            Assert.Equal(new Vector2(5, 5), bounds.Max);
        }

        #endregion

        #region Distance and Projection Tests (Delegation Spot Checks)

        [Fact]
        public void ClosestPoint_OnSegment()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));
            var point = new Vector2(5, 3);

            var closest = segment.ClosestPoint(point, out float distanceAlongSegment);

            Assert.Equal(new Vector2(5, 0), closest);
            Assert.Equal(0.5f, distanceAlongSegment, Collision2D.Epsilon);
        }

        [Fact]
        public void ClosestPoint_BeforeStart()
        {
            var segment = new LineSegment2D(new Vector2(10, 0), new Vector2(20, 0));
            var point = new Vector2(5, 3);

            var closest = segment.ClosestPoint(point, out float distanceAlongSegment);

            Assert.Equal(new Vector2(10, 0), closest);
            Assert.Equal(0.0f, distanceAlongSegment, Collision2D.Epsilon);
        }

        [Fact]
        public void ClosestPoint_AfterEnd()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));
            var point = new Vector2(15, 3);

            var closest = segment.ClosestPoint(point, out float distanceAlongSegment);

            Assert.Equal(new Vector2(10, 0), closest);
        }

        [Fact]
        public void DistanceToPoint_PerpendicularDistance()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));
            var point = new Vector2(5, 3);

            float distance = segment.DistanceToPoint(point);

            Assert.Equal(3.0f, distance, Collision2D.Epsilon);
        }

        [Fact]
        public void DistanceSquaredToPoint_AvoidsSqrt()
        {
            var segment = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));
            var point = new Vector2(5, 3);

            float distanceSquared = segment.DistanceSquaredToPoint(point);

            Assert.Equal(9.0f, distanceSquared, Collision2D.Epsilon);
        }

        [Fact]
        public void DistanceToSegment_Parallel()
        {
            var segment1 = new LineSegment2D(new Vector2(0, 0), new Vector2(10, 0));
            var segment2 = new LineSegment2D(new Vector2(0, 5), new Vector2(10, 5));

            float distance = segment1.DistanceToSegment(segment2);

            Assert.Equal(5.0f, distance, Collision2D.Epsilon);
        }

        #endregion

        #region Deconstruct Test

        [Fact]
        public void Deconstruct()
        {
            var segment = new LineSegment2D(new Vector2(1, 2), new Vector2(3, 4));

            var (start, end) = segment;

            Assert.Equal(new Vector2(1, 2), start);
            Assert.Equal(new Vector2(3, 4), end);
        }

        #endregion
    }
}
