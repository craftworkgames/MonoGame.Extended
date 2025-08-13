// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Data;

/// <summary>
/// Represents a color parameter for particle properties that can be either a constant color value
/// or a randomly generated color within a specified range.
/// </summary>
/// <remarks>
/// This struct uses <see cref="Vector3"/> to represent color values, the HSL color space.
/// </remarks>
public struct ParticleColorParameter : IEquatable<ParticleColorParameter>
{
    /// <summary>
    /// The <see cref="ParticleValueKind"/> that determines whether this parameter uses a constant value or a randomly
    /// generated value.
    /// </summary>
    public ParticleValueKind Kind;

    /// <summary>
    /// The constant color value when <see cref="Kind"/> is set to <see cref="ParticleValueKind.Constant"/>.
    /// </summary>
    public Vector3 Constant;

    /// <summary>
    /// The minimum color values of the range when <see cref="Kind"/> is set to <see cref="ParticleValueKind.Random"/>.
    /// </summary>
    public Vector3 RandomMin;

    /// <summary>
    /// The maximum color values of the range when <see cref="Kind"/> is set to <see cref="ParticleValueKind.Random"/>.
    /// </summary>
    public Vector3 RandomMax;

    /// <summary>
    /// Gets the current color value of this parameter based on its <see cref="Kind"/>.
    /// </summary>
    /// <remarks>
    /// If <see cref="Kind"/> is <see cref="ParticleValueKind.Constant"/>, returns <see cref="Constant"/>.
    /// If <see cref="Kind"/> is <see cref="ParticleValueKind.Random"/>, returns a random color where each component
    /// is a random value between the corresponding components of <see cref="RandomMin"/> and <see cref="RandomMax"/>.
    /// The vector components represent HSL.
    /// </remarks>
    public Vector3 Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (Kind == ParticleValueKind.Constant)
            {
                return Constant;
            }
            else
            {
                Vector3 hsl;
                hsl.X = FastRandom.Shared.NextSingle(RandomMin.X, RandomMax.X);
                hsl.Y = FastRandom.Shared.NextSingle(RandomMin.Y, RandomMax.Y);
                hsl.Z = FastRandom.Shared.NextSingle(RandomMin.Z, RandomMax.Z);
                return hsl;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleColorParameter"/> struct with a constant color value.
    /// </summary>
    /// <param name="value">The constant color value for this parameter.</param>
    public ParticleColorParameter(Vector3 value)
    {
        Kind = ParticleValueKind.Constant;

        Constant = value;

        RandomMin = default;
        RandomMax = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleColorParameter"/> struct with a random color range.
    /// </summary>
    /// <param name="rangeStart">The minimum color values of the random range.</param>
    /// <param name="rangeEnd">The maximum color values of the random range.</param>
    public ParticleColorParameter(Vector3 rangeStart, Vector3 rangeEnd)
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
        return obj is ParticleColorParameter other &&
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
    public readonly bool Equals(ParticleColorParameter other)
    {
        if (Kind == ParticleValueKind.Constant)
        {
            return Constant.Equals(other.Constant);
        }

        return RandomMin.Equals(other.RandomMin) && RandomMax.Equals(other.RandomMax);
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
        if (Kind == ParticleValueKind.Constant)
        {
            return Constant.GetHashCode();
        }

        return HashCode.Combine(RandomMin, RandomMax);
    }

    /// <summary>
    /// Determines whether two parameters are equal.
    /// </summary>
    /// <param name="lhs">The first parameter to compare.</param>
    /// <param name="rhs">The second parameter to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the parameters are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(ParticleColorParameter lhs, ParticleColorParameter rhs)
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
    public static bool operator !=(ParticleColorParameter lhs, ParticleColorParameter rhs)
    {
        return !lhs.Equals(rhs);
    }
}
