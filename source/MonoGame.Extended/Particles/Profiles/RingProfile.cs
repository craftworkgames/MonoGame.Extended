// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// A profile that distributes particles along the perimeter of a circle with controllable radiation patterns.
/// </summary>
/// <remarks>
///     <para>
///         The <see cref="RingProfile"/> positions new particles exclusively on the circumference of a circle centered
///         at the emitter's position. Unlike <see cref="CircleProfile"/> which distributes particles throughout the
///         circular area, this profile places particles only on the edge.
///     </para>
///     <para>
///         The movement direction (heading) of each particle can be configured to radiate inward toward the center,
///         outward from the center, or in random directions unrelated to their position.
///     </para>
/// </remarks>
public sealed class RingProfile : Profile
{
    /// <summary>
    /// Gets or sets the radius if the ring.
    /// </summary>
    public float Radius { get; set; }

    /// <summary>
    /// Gets or sets the radiation mode that determines how particle headings are calculated.
    /// </summary>
    public CircleRadiation Radiate { get; set; }

    /// <summary>
    /// Computes the offset and heading for a new particle.
    /// </summary>
    /// <param name="offset">A pointer to the Vector2 where the offset from the emitter position will be stored.</param>
    /// <param name="heading">A pointer to the Vector2 where the unit direction vector will be stored.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <see cref="Radiate"/> contains an unsupported value.
    /// </exception>
    public override unsafe void GetOffsetAndHeading(Vector2* offset, Vector2* heading)
    {
        FastRandom.Shared.NextUnitVector(heading);

        switch (Radiate)
        {
            case CircleRadiation.In:
                offset->X = -heading->X * Radius;
                offset->Y = -heading->Y * Radius;
                break;

            case CircleRadiation.Out:
                offset->X = heading->X * Radius;
                offset->Y = heading->Y * Radius;
                break;

            case CircleRadiation.None:
                offset->X = heading->X * Radius;
                offset->Y = heading->Y * Radius;
                FastRandom.Shared.NextUnitVector(heading);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(Radiate), Radiate, "Unsupported radiation mode");
        }
    }
}
