using System;

namespace MonoGame.Extended;

/// <summary>
/// Represents a closed mathematical interval [Min, Max] defined by comparable bounds.
/// </summary>
/// <typeparam name="T">
/// The type of values contained in the interval.
/// </typeparam>
public readonly struct Interval<T> : IEquatable<Interval<T>> where T : IComparable<T>
{
    private readonly T _min;
    private readonly T _max;

    /// <summary>
    /// Gets the minimum bound of the interval.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the interval is empty.</exception>
    public T Min
    {
        get
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Cannot access bounds of an empty interval");
            }
            return _min;
        }
    }

    /// <summary>
    /// Gets the maximum bound of the interval.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the interval is empty.</exception>
    public T Max
    {
        get
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Cannot access bounds of an empty interval");
            }
            return _max;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the interval contains no values.
    /// </summary>
    public bool IsEmpty { get; }

    /// <summary>
    /// Gets a value indicating whether the interval contains exactly one value.
    /// </summary>
    public bool IsDegenerate => !IsEmpty && _min.Equals(_max);

    /// <summary>
    /// Gets a value indicating whether the interval contains more than one value.
    /// </summary>
    public bool IsProper => !IsEmpty && !_min.Equals(_max);

    /// <summary>
    /// Gets an empty interval.
    /// </summary>
    public static Interval<T> Empty => new(true);

    /// <summary>
    /// Initializes an interval with the specified bounds.
    /// </summary>
    /// <param name="min">The minimum bound of the interval.</param>
    /// <param name="max">The maximum bound of the interval.</param>
    /// <exception cref="ArgumentException">Thrown when min is greater than max.</exception>
    public Interval(T min, T max)
    {
        if (min.CompareTo(max) > 0)
        {
            throw new ArgumentException("Minimum bounds cannot be greater than maximum bounds");
        }

        _min = min;
        _max = max;
        IsEmpty = false;
    }

    /// <summary>
    /// Initializes a degenerate interval containing only the specified value.
    /// </summary>
    /// <param name="value">The single value to be contained in the interval.</param>
    public Interval(T value) : this(value, value) { }

    private Interval(bool isEmpty)
    {
        _min = default;
        _max = default;
        IsEmpty = true;
    }

    /// <summary>
    /// Determines whether the interval contains the specified value.
    /// </summary>
    /// <param name="value">The value to test for containment.</param>
    /// <returns>true if the value is within the interval bounds; otherwise, false.</returns>
    public bool Contains(T value)
    {
        if (IsEmpty)
        {
            return false;
        }

        return value.CompareTo(_min) >= 0 && value.CompareTo(_max) <= 0;
    }

    /// <summary>
    /// Determines whether this interval completely contains another interval.
    /// </summary>
    /// <param name="other">The interval to test for containment.</param>
    /// <returns>true if this interval completely contains the other interval; otherwise, false.</returns>
    public bool Contains(Interval<T> other)
    {
        if (other.IsEmpty)
        {
            return true;
        }

        if (IsEmpty)
        {
            return false;
        }

        return _min.CompareTo(other._min) <= 0 && _max.CompareTo(other._max) >= 0;
    }

    /// <summary>
    /// Determines whether this interval shares any values with another interval.
    /// </summary>
    /// <param name="other">The interval to test for overlap.</param>
    /// <returns>true if the intervals have any values in common; otherwise, false.</returns>
    public bool Overlap(Interval<T> other)
    {
        if (IsEmpty || other.IsEmpty)
        {
            return false;
        }

        return _min.CompareTo(other._max) <= 0 && _max.CompareTo(other._min) >= 0;
    }

    /// <summary>
    /// Computes the intersection of this interval with another interval.
    /// </summary>
    /// <param name="other">The interval to intersect with.</param>
    /// <returns>An interval containing only values present in both intervals, or empty if no intersection exists.</returns>
    public Interval<T> Intersect(Interval<T> other)
    {
        if (IsEmpty || other.IsEmpty || !Overlap(other))
        {
            return Empty;
        }

        T minBound = _min.CompareTo(other._min) >= 0 ? _min : other._min;
        T maxBound = _max.CompareTo(other._max) <= 0 ? _max : other._max;

        return new Interval<T>(minBound, maxBound);
    }

    /// <summary>
    /// Computes the smallest interval containing both this interval and another interval.
    /// </summary>
    /// <param name="other">The interval to compute the hull with.</param>
    /// <returns>The smallest interval that contains both intervals.</returns>
    public Interval<T> Hull(Interval<T> other)
    {
        if (IsEmpty)
        {
            return other;
        }

        if (other.IsEmpty)
        {
            return this;
        }

        T minBound = _min.CompareTo(other._min) <= 0 ? _min : other._min;
        T maxBound = _max.CompareTo(other._max) >= 0 ? _max : other._max;

        return new Interval<T>(minBound, maxBound);
    }

    /// <summary>
    /// Creates an interval that spans both specified values.
    /// </summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns>An interval from the minimum to maximum of the two values.</returns>
    public static Interval<T> Hull(T a, T b)
    {
        if (a.CompareTo(b) <= 0)
        {
            return new Interval<T>(a, b);
        }
        else
        {
            return new Interval<T>(b, a);
        }
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is Interval<T> other && Equals(other);

    /// <inheritdoc />
    public bool Equals(Interval<T> other)
    {
        if (IsEmpty && other.IsEmpty)
        {
            return true;
        }

        if (IsEmpty && !other.IsEmpty)
        {
            return false;
        }

        return _min.Equals(other._min) && _max.Equals(other._max);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (IsEmpty)
        {
            return 0;
        }

        return HashCode.Combine(_min, _max);
    }

    /// <inheritdoc />
    public static bool operator ==(Interval<T> left, Interval<T> right) => left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(Interval<T> left, Interval<T> right) => !left.Equals(right);

    /// <inheritdoc />
    public static implicit operator Interval<T>(T value) => new(value);

    /// <inheritdoc />
    public override string ToString()
    {
        if (IsEmpty)
        {
            return "∅";
        }

        if (IsDegenerate)
        {
            return $"[{_min}]";
        }

        return $"[{_min}, {_max}]";
    }
}
