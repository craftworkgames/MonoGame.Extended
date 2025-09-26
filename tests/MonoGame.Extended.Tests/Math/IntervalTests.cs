using System;
using Xunit;

namespace MonoGame.Extended.Tests;

public class IntervalTests
{
    [Fact]
    public void Constructor_WithMinLessThanMax_CreatesProperInterval()
    {
        Interval<int> interval = new(10, 20);

        Assert.Equal(10, interval.Min);
        Assert.Equal(20, interval.Max);
        Assert.True(interval.IsProper);
        Assert.False(interval.IsDegenerate);
        Assert.False(interval.IsEmpty);
    }

    [Fact]
    public void Constructor_WithEqualBounds_CreatesDegenerateInterval()
    {
        Interval<int> interval = new(10, 10);

        Assert.Equal(10, interval.Min);
        Assert.Equal(10, interval.Max);
        Assert.False(interval.IsProper);
        Assert.True(interval.IsDegenerate);
        Assert.False(interval.IsEmpty);
    }

    [Fact]
    public void Constructor_WithInvalidBounds_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Interval<int>(20, 10));
    }

    [Fact]
    public void Constructor_SingleValue_CreatesDegenerate()
    {
        Interval<int> interval = new(10);
        Assert.Equal(10, interval.Min);
        Assert.Equal(10, interval.Max);
        Assert.True(interval.IsDegenerate);
        Assert.False(interval.IsProper);
        Assert.False(interval.IsEmpty);
    }

    [Fact]
    public void Empty_CreatesEmptyInterval()
    {
        Interval<int> empty = Interval<int>.Empty;
        Assert.True(empty.IsEmpty);
        Assert.False(empty.IsDegenerate);
        Assert.False(empty.IsProper);
    }

    [Fact]
    public void Empty_AccessingBounds_ThrowsInvalidOperationException()
    {
        Interval<int> empty = Interval<int>.Empty;

        Assert.Throws<InvalidOperationException>(() => empty.Min);
        Assert.Throws<InvalidOperationException>(() => empty.Max);
    }

    [Fact]
    public void Contains_Value_ReturnsTrueForBoundaryAndInteriorValues()
    {
        Interval<int> interval = new(10, 15);

        for (int i = 10; i <= 15; i++)
        {
            Assert.True(interval.Contains(i));
        }
    }

    [Fact]
    public void Contains_Value_ReturnsFalseForValuesOutsideBounds()
    {
        Interval<int> interval = new(10, 20);

        Assert.False(interval.Contains(9));
        Assert.False(interval.Contains(21));
    }

    [Fact]
    public void Contains_Value_EmptyInterval_ReturnsFalse()
    {
        Interval<int> empty = Interval<int>.Empty;
        Assert.False(empty.Contains(10));
    }

    [Fact]
    public void Contains_Interval_ReturnsTrueForCompletelyContainedInterval()
    {
        Interval<int> outer = new(10, 30);
        Interval<int> inner = new(15, 25);

        Assert.True(outer.Contains(inner));
    }

    [Fact]
    public void Contains_Interval_ReturnsFalseForPartiallyOverlappingInterval()
    {
        Interval<int> interval = new(10, 20);
        Interval<int> overlapping = new(15, 25);

        Assert.False(interval.Contains(overlapping));
    }

    [Fact]
    public void Contains_Interval_ReturnsFalseForSeparateInterval()
    {
        Interval<int> interval = new(10, 20);
        Interval<int> separate = new(30, 40);

        Assert.False(interval.Contains(separate));
    }

    [Fact]
    public void Contains_EmptyInterval_ReturnsTrue()
    {
        Interval<int> interval = new(10, 20);
        Interval<int> empty = Interval<int>.Empty;

        Assert.True(interval.Contains(empty));
        Assert.True(empty.Contains(empty));
    }

    [Fact]
    public void Contains_Self_ReturnsTrue()
    {
        Interval<int> interval = new(10, 20);

        Assert.True(interval.Contains(interval));
    }

    [Fact]
    public void Overlap_ReturnsTrueForOverlappingIntervals()
    {
        Interval<int> a = new(10, 20);
        Interval<int> b = new(15, 25);

        Assert.True(a.Overlap(b));
        Assert.True(b.Overlap(a));
    }

    [Fact]
    public void Overlap_ReturnsFalseForSeparateIntervals()
    {
        Interval<int> a = new(10, 20);
        Interval<int> c = new(30, 40);

        Assert.False(a.Overlap(c));
        Assert.False(c.Overlap(a));
    }

    [Fact]
    public void Overlap_ReturnsTrueForIntervalsTouchingAtBoundary()
    {
        Interval<int> a = new(10, 20);
        Interval<int> d = new(20, 30);

        Assert.True(a.Overlap(d));
    }

    [Fact]
    public void Overlap_WithEmptyInterval_ReturnsFalse()
    {
        Interval<int> interval = new(10, 20);
        Interval<int> empty = Interval<int>.Empty;

        Assert.False(interval.Overlap(empty));
        Assert.False(empty.Overlap(empty));
    }

    [Fact]
    public void Overlap_Self_ReturnsTrue()
    {
        Interval<int> interval = new(10, 20);

        Assert.True(interval.Overlap(interval));
    }

    [Fact]
    public void Intersect_OverlappingIntervals_ReturnsCorrectBounds()
    {
        Interval<int> a = new(10, 20);
        Interval<int> b = new(15, 25);
        Interval<int> intersection = a.Intersect(b);

        Assert.Equal(15, intersection.Min);
        Assert.Equal(20, intersection.Max);
    }

    [Fact]
    public void Intersect_NoOverlap_ReturnsEmpty()
    {
        Interval<int> a = new(10, 20);
        Interval<int> b = new(30, 40);

        Assert.True(a.Intersect(b).IsEmpty);
    }

    [Fact]
    public void Intersect_WithEmptyInterval_ReturnsEmpty()
    {
        Interval<int> interval = new(10, 20);
        Interval<int> empty = Interval<int>.Empty;

        Assert.True(interval.Intersect(empty).IsEmpty);
        Assert.True(empty.Intersect(empty).IsEmpty);
    }

    [Fact]
    public void Intersect_Self_ReturnsOriginalInterval()
    {
        Interval<int> interval = new(10, 20);

        Assert.Equal(interval, interval.Intersect(interval));
    }

    [Fact]
    public void Hull_SeparateIntervals_ReturnsSpanningInterval()
    {
        Interval<int> a = new(10, 20);
        Interval<int> b = new(30, 40);
        Interval<int> hull = a.Hull(b);

        Assert.Equal(10, hull.Min);
        Assert.Equal(40, hull.Max);
    }

    [Fact]
    public void Hull_WithEmptyInterval_ReturnsNonEmptyInterval()
    {
        Interval<int> interval = new(10, 20);
        Interval<int> empty = Interval<int>.Empty;

        Assert.Equal(interval, interval.Hull(empty));
        Assert.Equal(interval, empty.Hull(interval));
        Assert.True(empty.Hull(empty).IsEmpty);
    }

    [Fact]
    public void Hull_Self_ReturnsOriginalInterval()
    {
        Interval<int> interval = new(10, 20);

        Assert.Equal(interval, interval.Hull(interval));
    }

    [Fact]
    public void Hull_StaticMethod_HandlesParameterOrderCorrectly()
    {
        Interval<int> hull1 = Interval<int>.Hull(20, 10);
        Interval<int> hull2 = Interval<int>.Hull(10, 20);

        Assert.Equal(10, hull1.Min);
        Assert.Equal(20, hull1.Max);
        Assert.Equal(hull1, hull2);
    }

    [Fact]
    public void Equality_SameIntervals_ReturnsTrue()
    {
        Interval<int> a = new(10, 20);
        Interval<int> b = new(10, 20);

        Assert.True(a == b);
        Assert.True(a.Equals(b));
        Assert.False(a != b);
    }

    [Fact]
    public void Equality_DifferentIntervals_ReturnsFalse()
    {
        Interval<int> a = new(10, 20);
        Interval<int> c = new(20, 30);

        Assert.False(a == c);
        Assert.False(a.Equals(c));
        Assert.True(a != c);
    }

    [Fact]
    public void Equality_EmptyIntervals_ReturnsTrue()
    {
        Interval<int> empty1 = Interval<int>.Empty;
        Interval<int> empty2 = Interval<int>.Empty;

        Assert.True(empty1 == empty2);
        Assert.True(empty1.Equals(empty2));
    }

    [Fact]
    public void Equality_EmptyVsNonEmpty_ReturnsFalse()
    {
        Interval<int> interval = new(10, 20);
        Interval<int> empty = Interval<int>.Empty;

        Assert.False(interval == empty);
        Assert.True(interval != empty);
    }

    [Fact]
    public void GetHashCode_EqualIntervals_ReturnsSameHashCode()
    {
        Interval<int> a = new(10, 20);
        Interval<int> b = new(10, 20);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesDegenerate()
    {
        Interval<int> interval = 10;

        Assert.Equal(10, interval.Min);
        Assert.Equal(10, interval.Max);
        Assert.True(interval.IsDegenerate);
    }

    [Fact]
    public void ToString_ShowsCorrectFormat()
    {
        Assert.Equal("[10, 20]", new Interval<int>(10, 20).ToString());
        Assert.Equal("[10]", new Interval<int>(10).ToString());
        Assert.Equal("∅", Interval<int>.Empty.ToString());
    }

    [Fact]
    public void StringInterval_WorksWithComparable()
    {
        Interval<string> interval = new("apple", "zebra");

        Assert.True(interval.Contains("banana"));
        Assert.False(interval.Contains("aardvark"));
    }
}
