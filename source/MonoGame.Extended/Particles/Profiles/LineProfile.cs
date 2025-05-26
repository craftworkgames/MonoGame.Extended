// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// A profile that distributes particles uniformly along a line segment with random headings.
/// </summary>
/// <remarks>
/// The <see cref="LineProfile"/> positions particles randomly along a line segment centered at the emitter position and
/// defined by an axis direction and length. Unlike <see cref="LineUniformProfile"/>, this profile gives each particle a
/// random heading in any direction.
/// </remarks>
public sealed class LineProfile : Profile
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
    /// Computes the offset and heading for a new particle.
    /// </summary>
    /// <param name="offset">A pointer to the Vector2 where the offset from the emitter position will be stored.</param>
    /// <param name="heading">A pointer to the Vector2 where the unit direction vector will be stored.</param>
    public override unsafe void GetOffsetAndHeading(Vector2* offset, Vector2* heading)
    {
        float value = FastRandom.Shared.NextSingle(Length * -0.5f, Length * 0.5f);
        offset->X = Axis.X * value;
        offset->Y = Axis.Y * value;
        FastRandom.Shared.NextUnitVector(heading);
    }
}
