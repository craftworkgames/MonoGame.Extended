// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// A profile that distributes particles along the edges of a rectangular boundary with uniform density.
/// </summary>
/// <remarks>
///     <para>
///         The <see cref="BoxUniformProfile"/> positions new particles on the perimeter of a rectangle centered at the
///         emitter's position. Unlike <see cref="BoxProfile"/> which gives equal probability to each side, this profile
///         allocates probability proportional to the length of each side, ensuring a uniform distribution of particles
///         around the entire perimeter.
///     </para>
///     <para>
///         This means longer sides will receive more particles than shorter sides, creating a visually balanced
///         distribution regardless of the rectangle's dimensions.
///     </para>
///     <para>
///         Particles are given random unit vector headings, allowing them to move in any direction regardless of their
///         starting edge.
///     </para>
/// </remarks>
public class BoxUniformProfile : Profile
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
        int perimeter = (int)(2 * Width + 2 * Height);
        int value = FastRandom.Shared.Next(perimeter);

        switch (value)
        {
            //  Top
            case var _ when value < Width:
                offset->X = FastRandom.Shared.NextSingle(Width * -0.5f, Width * 0.5f);
                offset->Y = Height * -0.5f;
                break;

            //  Bottom
            case var _ when value < 2 * Width:
                offset->X = FastRandom.Shared.NextSingle(Width * -0.5f, Width * 0.5f);
                offset->Y = Height * 0.5f;
                break;

            //  Left
            case var _ when value < 2 * Width + Height:
                offset->X = Width * -0.5f;
                offset->Y = FastRandom.Shared.NextSingle(Height * -0.5f, Height * 0.5f);
                break;

            // Right
            default:
                offset->X = Width * 0.5f;
                offset->Y = FastRandom.Shared.NextSingle(Height * -0.5f, Height * 0.5f);
                break;
        }

        FastRandom.Shared.NextUnitVector(heading);
    }
}
