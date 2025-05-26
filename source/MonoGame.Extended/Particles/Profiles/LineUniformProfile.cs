// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// A profile that distributes particles uniformly along a line segment with a fixed heading direction.
/// </summary>
/// <remarks>
/// The <see cref="LineUniformProfile"/> positions particles randomly along a line segment centered at the emitter
/// position and defined by an axis direction and length. Unlike other profiles, this profile uses a fixed heading
/// direction for all particles, perpendicular to the line.
/// </remarks>
public sealed class LineUniformProfile : Profile
{
    /// <summary>
    /// The direction vector of the line axis.
    /// </summary>
    public Vector2 Axis;

    /// <summary>
    /// The length of the line segment.
    /// </summary>
    public float Length;

    /// <summary>
    /// The fixed heading direction for all particles spawned from this profile.
    /// </summary>
    public Vector2 PerpendicularDirection;

    /// <summary>
    /// Computes the offset and heading for a new particle.
    /// </summary>
    /// <param name="offset">A pointer to the Vector2 where the offset from the emitter position will be stored.</param>
    /// <param name="heading">A pointer to the Vector2 where the unit direction vector will be stored.</param>
    public override unsafe void GetOffsetAndHeading(Vector2* offset, Vector2* heading)
    {
        // 1. Spawn the particle at a random point on the line axis
        float value = FastRandom.Shared.NextSingle(Length * -0.5f, Length * 0.5f);
        offset->X = Axis.X * value;
        offset->Y = Axis.Y * value;

        // 2. Set the heading to the perpendicular direction
        *heading = PerpendicularDirection;
    }

    /// <summary>
    /// Sets the <see cref="PerpendicularDirection"/> property to a normalized version of the specified vector.
    /// </summary>
    /// <param name="direction">The direction vector to normalize and use as the perpendicular direction.</param>
    public void SetPerpendicularDirection(Vector2 direction)
    {
        PerpendicularDirection = Vector2.Normalize(direction);
    }
}
