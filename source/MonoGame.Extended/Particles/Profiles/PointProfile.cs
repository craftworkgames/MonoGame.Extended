// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// A profile that emits all particles from a single point with random headings.
/// </summary>
/// <remarks>
/// The <see cref="PointProfile"/> is the simplest emission profile, where all particles originate exactly at the
/// emitter position with no offset. Each particle is given a random heading in any direction, creating a radial
/// dispersion pattern.
/// </remarks>
public sealed class PointProfile : Profile
{
    /// <summary>
    /// Computes the offset and heading for a new particle.
    /// </summary>
    /// <param name="offset">A pointer to the Vector2 where the offset from the emitter position will be stored.</param>
    /// <param name="heading">A pointer to the Vector2 where the unit direction vector will be stored.</param>
    /// <remarks>
    /// The offset is always set to (0,0), meaning particles will spawn exactly at the emitter position.
    /// </remarks>
    public override unsafe void GetOffsetAndHeading(Vector2* offset, Vector2* heading)
    {
        offset->X = 0.0f;
        offset->Y = 0.0f;
        FastRandom.Shared.NextUnitVector(heading);
    }
}
