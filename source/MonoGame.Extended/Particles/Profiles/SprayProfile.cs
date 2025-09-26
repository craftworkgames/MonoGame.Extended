// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// A profile that emits particles from a single point in a directional cone pattern.
/// </summary>
/// <remarks>
/// The <see cref="SprayProfile"/> positions all particles exactly at the emitter's position,
/// like <see cref="PointProfile"/>, but instead of random directions in all directions, it constrains the particle
/// headings to a cone-shaped area defined by a central direction and spread angle.
/// </remarks>
public sealed class SprayProfile : Profile
{
    /// <summary>
    /// Gets or sets the central direction vector of the spray.
    /// </summary>
    public Vector2 Direction { get; set; }

    /// <summary>
    /// Gets or sets the angular spread of the spray cone (in radians).
    /// </summary>
    /// <remarks>
    /// This value determines how wide the spray cone is.  For example:
    ///
    /// <list type="bullet">
    ///     <item>A value of 0 will emit all particles in exactly the same direction.</item>
    ///     <item>A value of π (Pi) will create a 180-degree fan.</item>
    ///     <item>A value of 2π will emit in all directions (similar to <see cref="PointProfile"/>).</item>
    /// </list>
    /// </remarks>
    public float Spread { get; set; }

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
        offset->X = offset->Y = 0.0f;

        float angle = MathF.Atan2(Direction.Y, Direction.X);
        angle = FastRandom.Shared.NextSingle(angle - Spread * 0.5f, angle + Spread * 0.5f);

        heading->X = MathF.Cos(angle);
        heading->Y = MathF.Sin(angle);
    }
}
