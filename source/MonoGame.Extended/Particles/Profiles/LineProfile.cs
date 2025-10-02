// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// A profile that distributes particles uniformly along a line segment with random headings.
/// </summary>
/// <remarks>
/// The <see cref="LineProfile"/> positions particles randomly along a line segment centered at the emitter position and
/// defined by an axis direction and length.
/// </remarks>
public sealed class LineProfile : Profile
{
    /// <summary>
    /// Gets or sets the direction vector of the line axis.
    /// </summary>
    public Vector2 Axis { get; set; }

    /// <summary>
    /// Gets or sets the length of the line segment.
    /// </summary>
    public float Length { get; set; }

    /// <summary>
    /// Gets or sets the emission direction vector used when <see cref="Radiate"/> is
    /// <see cref="LineRadiation.Directional"/> or as a scale factor when <see cref="Radiate"/> is
    /// <see cref="LineRadiation.PerpendicularUp"/> or <see cref="LineRadiation.PerpendicularDown"/>.
    /// </summary>
    /// <remarks>
    /// For <see cref="LineRadiation.Directional"/>, this vector directly specifies the particle heading direction.
    /// For <see cref="LineRadiation.PerpendicularUp"/> and <see cref="LineRadiation.PerpendicularDown"/>, this vector's
    /// magnitude and sign control the normal emission behavior. Positive values emit in the specified normal direction,
    /// while negative values flip to the opposite direction.
    /// This property is ignored when <see cref="Radiate"/> is <see cref="LineRadiation.None"/>.
    /// </remarks>
    public Vector2 Direction { get; set; }= Vector2.UnitY;

    /// <summary>
    /// Gets or sets the radiation mode that determines how particle headings are calculated.
    /// </summary>
    public LineRadiation Radiate { get; set; } = LineRadiation.None;

    /// <summary>
    /// Computes the offset and heading for a new particle.
    /// </summary>
    /// <param name="offset">A pointer to the Vector2 where the offset from the emitter position will be stored.</param>
    /// <param name="heading">A pointer to the Vector2 where the unit direction vector will be stored.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="Radiate"/> contains an unsupported value.
    /// </exception>
    public override unsafe void GetOffsetAndHeading(Vector2* offset, Vector2* heading)
    {
        float value = FastRandom.Shared.NextSingle(Length * -0.5f, Length * 0.5f);
        Vector2 normalizedAxis = Vector2.Normalize(Axis);
        offset->X = normalizedAxis.X * value;
        offset->Y = normalizedAxis.Y * value;

        // Calculate heading based on radiation mode
        switch (Radiate)
        {
            case LineRadiation.None:
                FastRandom.Shared.NextUnitVector(heading);
                break;

            case LineRadiation.Directional:
                Vector2 normalizedDirection = Vector2.Normalize(Direction);
                heading->X = normalizedDirection.X;
                heading->Y = normalizedDirection.Y;
                break;

            case LineRadiation.PerpendicularUp:
                heading->X = normalizedAxis.Y;
                heading->Y = -normalizedAxis.X;
                break;

            case LineRadiation.PerpendicularDown:
                heading->X = -normalizedAxis.Y;
                heading->Y = normalizedAxis.X;
                break;

            default:
                throw new InvalidOperationException($"Unsupported radiation mode '{Radiate}'");
        }
    }
}
