// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Data;

/// <summary>
/// Represents an vector parameter for particle properties that can be either a constant value or a
/// randomly generated value within a specified range.
/// </summary>
public struct ParticleVector2Parameter : IEquatable<ParticleVector2Parameter>
{
    /// <summary>
    /// The <see cref="ParticleValueKind"/> that determines whether this parameter uses a constant value or a randomly
    /// generated value.
    /// </summary>
    public ParticleValueKind Kind;

    /// <summary>
    /// The constant value when <see cref="Kind"/> is <see cref="ParticleValueKind.Constant"/>.
    /// </summary>
    public Vector2 Constant;

    /// <summary>
    /// The minimum value of the range when <see cref="Kind"/> is <see cref="ParticleValueKind.Random"/>.
    /// </summary>
    public Vector2 RandomMin;

    /// <summary>
    /// The maximum value of the range when <see cref="Kind"/> is <see cref="ParticleValueKind.Random"/>.
    /// </summary>
    public Vector2 RandomMax;

    /// <summary>
    /// Gets the current value of this parameter based on its <see cref="Kind"/>
    /// </summary>
    /// <remarks>
    /// If <see cref="Kind"/> is <see cref="ParticleValueKind.Constant"/>, returns <see cref="Constant"/>.
    /// If <see cref="Kind"/> is <see cref="ParticleValueKind.Random"/>, returns a random value between
    /// <see cref="RandomMin"/> and <see cref="RandomMax"/>.
    /// </remarks>
    public Vector2 Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (Kind == ParticleValueKind.Constant)
            {
                return Constant;
            }

            Vector2 v;
            v.X = FastRandom.Shared.NextSingle(RandomMin.X, RandomMax.X);
            v.Y = FastRandom.Shared.NextSingle(RandomMin.Y, RandomMax.Y);
            return v;
        }
    }

    /// <summary>
    /// Initializes a new <see cref="ParticleVector2Parameter"/> value with a constant value.
    /// </summary>
    /// <param name="value">The constant value for this parameter.</param>
    public ParticleVector2Parameter(Vector2 value)
    {
        Kind = ParticleValueKind.Constant;
        Constant = value;
        RandomMin = default;
        RandomMax = default;
    }

    /// <summary>
    /// Initializes a new <see cref="ParticleVector2Parameter"/> value with a random range.
    /// </summary>
    /// <param name="rangeStart">The minimum value of the random range.</param>
    /// <param name="rangeEnd">The maximum value of the random range.</param>
    public ParticleVector2Parameter(Vector2 rangeStart, Vector2 rangeEnd)
    {
        Kind = ParticleValueKind.Random;
        Constant = default;
        RandomMin = rangeStart;
        RandomMax = rangeEnd;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current parameter.
    /// </summary>
    /// <param name="obj">The object to compare with the current parameter.</param>
    /// <returns>
    /// <see langword="true"/> if the specified object is equal tot he current parameter;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public override readonly bool Equals([NotNullWhen(true)] object obj)
    {
        return obj is ParticleVector2Parameter other &&
               Equals(other);
    }

    /// <summary>
    /// Determines whether the specified parameter is equal to the current parameter.
    /// </summary>
    /// <param name="other">The parameter to compare with the current parameter.</param>
    /// <returns>
    /// <see langword="true"/> if the specified parameter is equal to the current parameter;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// When <see cref="Kind"/> is <see cref="ParticleValueKind.Constant"/> only the <see cref="Constant"/> values are
    /// compared.
    /// When <see cref="Kind"/> is <see cref="ParticleValueKind.Random"/>, both <see cref="RandomMin"/> and
    /// <see cref="RandomMax"/> values are compared.
    /// </remarks>
    public readonly bool Equals(ParticleVector2Parameter other)
    {
        if (Kind == ParticleValueKind.Constant)
        {
            return Constant.Equals(other.Constant);
        }

        return RandomMin.Equals(other.RandomMin) &&
               RandomMax.Equals(other.RandomMax);
    }

    /// <summary>
    /// Returns the hash code for this parameter.
    /// </summary>
    /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
    /// <remarks>
    /// When <see cref="Kind"/> is <see cref="ParticleValueKind.Constant"/>, returns the hash of <see cref="Constant"/>.
    /// When <see cref="Kind"/> is <see cref="ParticleValueKind.Random"/> returns the combined has of
    /// <see cref="RandomMin"/> and <see cref="RandomMax"/>.
    /// </remarks>
    public override readonly int GetHashCode()
    {
        base.GetHashCode();
        if (Kind == ParticleValueKind.Constant)
        {
            return Constant.GetHashCode();
        }

        return HashCode.Combine(RandomMin, RandomMax);
    }

    /// <summary>
    /// Returns a string representation of this parameter.
    /// </summary>
    /// <returns>
    /// When <see cref="Kind"/> is <see cref="ParticleValueKind.Constant"/>, returns the string representation of
    /// <see cref="Constant"/>.
    /// When <see cref="Kind"/> is <see cref="ParticleValueKind.Random"/>, returns a string in the format
    /// "MinValue, MaxValue".
    /// </returns>
    public override readonly string ToString()
    {
        if (Kind == ParticleValueKind.Constant)
        {
            return Constant.ToString();
        }

        return string.Format(NumberFormatInfo.InvariantInfo, "{0}{1}{2}", RandomMin, NumberFormatInfo.InvariantInfo.NumberGroupSeparator, RandomMax);
    }

    /// <summary>
    /// Determines whether two parameters are equal.
    /// </summary>
    /// <param name="lhs">The first parameter to compare.</param>
    /// <param name="rhs">The second parameter to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the parameters are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(ParticleVector2Parameter lhs, ParticleVector2Parameter rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>
    /// Determines whether two parameters are not equal.
    /// </summary>
    /// <param name="lhs">The first parameter to compare.</param>
    /// <param name="rhs">The second parameter to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the parameters are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(ParticleVector2Parameter lhs, ParticleVector2Parameter rhs)
    {
        return !lhs.Equals(rhs);
    }
}
