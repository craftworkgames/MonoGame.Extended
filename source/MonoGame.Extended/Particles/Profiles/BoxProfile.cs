// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// A profile that distributes particles along the edges of a rectangular boundary.
/// </summary>
/// <remarks>
/// The <see cref="BoxProfile"/> randomly positions new particles on one of the four sides of a rectangular area
/// centered at the emitter's position. Each side has an equal probability of being selected. Particles are given random
/// unit vector headings, allowing them to move in any direction regardless of their starting edge.
/// </remarks>
public sealed class BoxProfile : Profile
{
    /// <summary>
    /// Gets or sets the width of the rectangular perimeter.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the rectangular perimeter.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Computes the offset and heading for a new particle.
    /// </summary>
    /// <param name="offset">A pointer to the Vector2 where the offset from the emitter position will be stored.</param>
    /// <param name="heading">A pointer to the Vector2 where the unit direction vector will be stored.</param>
    public override unsafe void GetOffsetAndHeading(Vector2* offset, Vector2* heading)
    {
        switch (FastRandom.Shared.Next(4))
        {
            case 0: // Left
                offset->X = Width * -0.5f;
                offset->Y = FastRandom.Shared.NextSingle(Height * -0.5f, Height * 0.5f);
                break;

            case 1: // Top
                offset->X = FastRandom.Shared.NextSingle(Width * -0.5f, Width * 0.5f);
                offset->Y = Height * -0.5f;
                break;

            case 2: // Right
                offset->X = Width * 0.5f;
                offset->Y = FastRandom.Shared.NextSingle(Height * -0.5f, Height * 0.5f);
                break;

            default: // Bottom
                offset->X = FastRandom.Shared.NextSingle(Width * -0.5f, Width * 0.5f);
                offset->Y = Height * 0.5f;
                break;
        }

        FastRandom.Shared.NextUnitVector(heading);
    }
}
