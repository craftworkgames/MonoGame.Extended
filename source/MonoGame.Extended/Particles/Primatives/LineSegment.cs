// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace MonoGame.Extended.Particles.Primitives;

/// <summary>
/// Represents a line segment defined by two points in 2D space.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct LineSegment : IEquatable<LineSegment>
{
    /// <summary>
    /// The first point of the line segment.
    /// </summary>
    internal readonly Vector2 _point1;

    /// <summary>
    /// The second point of the line segment.
    /// </summary>
    internal readonly Vector2 _point2;

    /// <summary>
    /// Gets the origin point of the line segment.
    /// </summary>
    /// <value>The first point of the line segment.</value>
    public readonly Vector2 Origin
    {
        get
        {
            return _point1;
        }
    }

    /// <summary>
    /// Gets the direction vector of the line segment.
    /// </summary>
    /// <value>A vector from the first point to the second point.</value>
    public readonly Vector2 Direction
    {
        get
        {
            return _point2 - _point1;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSegment"/> struct with the specified points.
    /// </summary>
    /// <param name="point1">The first point of the line segment.</param>
    /// <param name="point2">The second point of the line segment.</param>
    public LineSegment(Vector2 point1, Vector2 point2)
    {
        _point1 = point1;
        _point2 = point2;
    }

    /// <summary>
    /// Returns a new line segment that is a translated version of this line segment.
    /// </summary>
    /// <param name="vector">The vector by which to translate the line segment.</param>
    /// <returns>A new <see cref="LineSegment"/> that is offset by the specified vector.</returns>
    public LineSegment Translate(Vector2 vector)
    {
        return new LineSegment(_point1 + vector, _point2 + vector);
    }

    /// <summary>
    /// Converts the line segment to a vector representing its direction and magnitude.
    /// </summary>
    /// <returns>
    /// A <see cref="Vector2"/> representing the direction and length of the line segment, calculated as the second
    /// point minus the first point.
    /// </returns>
    public Vector2 ToVector2()
    {
        return _point2 - _point1;
    }

    /// <summary>
    /// Creates a new line segment from two points.
    /// </summary>
    /// <param name="point1">The first point of the line segment.</param>
    /// <param name="point2">The second point of the line segment.</param>
    /// <returns>A new <see cref="LineSegment"/> defined by the two points.</returns>
    public static LineSegment FromPoints(Vector2 point1, Vector2 point2)
    {
        return new LineSegment(point1, point2);
    }

    /// <summary>
    /// Creates a new line segment from an origin point and a direction vector.
    /// </summary>
    /// <param name="origin">The starting point of the line segment.</param>
    /// <param name="vector">The direction and length vector of the line segment.</param>
    /// <returns>
    /// A new <see cref="LineSegment"/> starting at the origin and extending by the specified vector.
    /// </returns>
    public static LineSegment FromOrigin(Vector2 origin, Vector2 vector)
    {
        return new LineSegment(origin, origin + vector);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current line segment.
    /// </summary>
    /// <param name="obj">The object to compare with the current line segment.</param>
    /// <returns>
    /// <see langword="true"/> if the specified object is a <see cref="LineSegment"/> and is equal to the
    /// current line segment; otherwise, <see langword="false"/>.
    /// </returns>
    public override readonly bool Equals([NotNullWhen(true)] object obj)
    {
        return obj is LineSegment other && Equals(other);
    }

    /// <summary>
    /// Determines whether the specified line segment is equal to the current line segment.
    /// </summary>
    /// <param name="other">The line segment to compare with the current line segment.</param>
    /// <returns>
    /// <see langword="true"/> if the specified line segment is equal to the current line segment;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public readonly bool Equals(LineSegment other)
    {
        return _point1.Equals(other._point1) && _point2.Equals(other._point2);
    }

    /// <summary>
    /// Returns the hash code for this line segment.
    /// </summary>
    /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(_point1, _point2);
    }

    /// <summary>
    /// Returns a string representation of this line segment.
    /// </summary>
    /// <returns>
    /// A string containing the coordinates of both points in the format:
    /// "(x1:y1,x2:y2)".
    /// </returns>
    public override readonly string ToString()
    {
        return $"({_point1:x}:{_point1:y},{_point2:x}:{_point2:y})";
    }

    /// <summary>
    /// Determines whether two line segments are equal.
    /// </summary>
    /// <param name="lhs">The first line segment to compare.</param>
    /// <param name="rhs">The second line segment to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the line segments are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(LineSegment lhs, LineSegment rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>
    /// Determines whether two line segments are not equal.
    /// </summary>
    /// <param name="lhs">The first line segment to compare.</param>
    /// <param name="rhs">The second line segment to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the line segments are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(LineSegment lhs, LineSegment rhs)
    {
        return !lhs.Equals(rhs);
    }
}
