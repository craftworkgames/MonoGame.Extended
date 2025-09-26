// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// A profile that distributes particles throughout a circular area with controllable radiation patterns.
/// </summary>
/// <remarks>
/// The <see cref="CircleProfile"/> randomly positions new particles within a circle centered at the emitter's position.
/// The movement direction (heading) of each particle can be configured to radiate inward toward the center, outward
/// from the center, or in random directions.
/// </remarks>
public sealed class CircleProfile : Profile
{
    /// <summary>
    /// Gets or sets the radius of the circular area.
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
        float distance = FastRandom.Shared.NextSingle(0f, Radius);

        FastRandom.Shared.NextUnitVector(heading);

        switch (Radiate)
        {
            case CircleRadiation.In:
                offset->X = -heading->X * distance;
                offset->Y = -heading->Y * distance;
                break;

            case CircleRadiation.Out:
                offset->X = heading->X * distance;
                offset->Y = heading->Y * distance;
                break;

            case CircleRadiation.None:
                offset->X = heading->X * distance;
                offset->Y = heading->Y * distance;
                FastRandom.Shared.NextUnitVector(heading);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(Radiate), Radiate, "Unsupported radiation mode");
        }
    }
}
