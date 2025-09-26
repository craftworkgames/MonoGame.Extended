// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// A profile that randomly distributes particles throughout a rectangular area.
/// </summary>
/// <remarks>
/// The <see cref="BoxFillProfile"/> generates random positions within a rectangle centered the emitter's position, with
/// random unit vector headings. This creates a uniform distribution of particles across the defined area, with
/// particles moving in all directions.
/// </remarks>
public sealed class BoxFillProfile : Profile
{
    /// <summary>
    /// Gets or sets the width of the rectangular area.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the rectangular area.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Computes the offset and heading for a new particle.
    /// </summary>
    /// <param name="offset">A pointer to the Vector2 where the offset from the emitter position will be stored.</param>
    /// <param name="heading">A pointer to the Vector2 where the unit direction vector will be stored.</param>
    public override unsafe void GetOffsetAndHeading(Vector2* offset, Vector2* heading)
    {
        offset->X = FastRandom.Shared.NextSingle(Width * -0.5f, Width * 0.5f);
        offset->Y = FastRandom.Shared.NextSingle(Height * -0.5f, Height * 0.5f);
        FastRandom.Shared.NextUnitVector(heading);
    }
}
