// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// Provides an abstract base class for particle emission profiles.
/// </summary>
/// <remarks>
/// A profile determines how particles are initially positioned and directed when they are emitted. Different profiles
/// create different distribution patterns, such as points, lines, circles, or boxes.
/// </remarks>
public abstract class Profile
{
    /// <summary>
    /// Computes the offset and heading for a new particle.
    /// </summary>
    /// <param name="offset">A pointer to the Vector2 where the offset from the emitter position will be stored.</param>
    /// <param name="heading">A pointer to the Vector2 where the unit direction vector will be stored.</param>
    public abstract unsafe void GetOffsetAndHeading(Vector2* offset, Vector2* heading);

    /// <summary>
    /// Creates a <see cref="PointProfile"/> that emits particles from a single point.
    /// </summary>
    /// <returns>A new <see cref="PointProfile"/> instance.</returns>
    public static Profile Point()
    {
        return new PointProfile();
    }

    /// <summary>
    /// Creates a <see cref="LineProfile"/> that emits particles along a line segment.
    /// </summary>
    /// <param name="axis">The direction vector of the line.</param>
    /// <param name="length">The length of the line segment.</param>
    /// <returns>A new <see cref="LineProfile"/> instance.</returns>
    public static Profile Line(Vector2 axis, float length)
    {
        return new LineProfile { Axis = axis, Length = length };
    }

    /// <summary>
    /// Creates a <see cref="LineProfile"/> that emits particles along a line segment.
    /// </summary>
    /// <param name="axis">The direction vector of the line.</param>
    /// <param name="length">The length of the line segment.</param>
    /// <param name="radiate">The radiation mode that determines how particle headings are calculated.</param>
    /// <returns>A new <see cref="LineProfile"/> instance.</returns>
    public static Profile Line(Vector2 axis, float length, LineRadiation radiate)
    {
        return new LineProfile { Axis = axis, Length = length, Radiate = radiate, Direction = Vector2.Zero };
    }

    /// <summary>
    /// Creates a <see cref="LineProfile"/> that emits particles along a line segment.
    /// </summary>
    /// <param name="axis">The direction vector of the line.</param>
    /// <param name="length">The length of the line segment.</param>
    /// <param name="radiate">The radiation mode that determines how particle headings are calculated.</param>
    /// <param name="direction">
    /// The emission direction vector used when <paramref name="radiate"/> is <see cref="LineRadiation.Directional"/>
    /// or as a scale factor when <paramref name="radiate"/> is <see cref="LineRadiation.NormalUp"/> or
    /// <see cref="LineRadiation.NormalDown"/>.
    /// </param>
    /// <returns>A new <see cref="LineProfile"/> instance.</returns>
    public static Profile Line(Vector2 axis, float length, LineRadiation radiate, Vector2 direction)
    {
        return new LineProfile { Axis = axis, Length = length, Radiate = radiate, Direction = direction };
    }

    /// <summary>
    /// Creates a <see cref="RingProfile"/> that emits particles from the perimeter of a circle.
    /// </summary>
    /// <param name="radius">The radius of the ring.</param>
    /// <param name="radiate">The radiation pattern for particle headings.</param>
    /// <returns>A new <see cref="RingProfile"/> instance.</returns>
    public static Profile Ring(float radius, CircleRadiation radiate)
    {
        return new RingProfile { Radius = radius, Radiate = radiate };
    }

    /// <summary>
    /// Creates a <see cref="BoxProfile"/> that emits particles from the perimeter of a rectangle.
    /// </summary>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <returns>A new <see cref="BoxProfile"/> instance.</returns>
    public static Profile Box(float width, float height)
    {
        return new BoxProfile { Width = width, Height = height };
    }

    /// <summary>
    /// Creates a <see cref="BoxFillProfile"/> that emits particles from within a rectangular area.
    /// </summary>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <returns>A new <see cref="BoxFillProfile"/> instance.</returns>
    public static Profile BoxFill(float width, float height)
    {
        return new BoxFillProfile { Width = width, Height = height };
    }

    /// <summary>
    /// Creates a <see cref="BoxUniformProfile"/> that emits particles from the perimeter of a rectangle with uniform density.
    /// </summary>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <returns>A new <see cref="BoxUniformProfile"/> instance.</returns>
    public static Profile BoxUniform(float width, float height)
    {
        return new BoxUniformProfile { Width = width, Height = height };
    }

    /// <summary>
    /// Creates a <see cref="CircleProfile"/> that emits particles from within a circular area.
    /// </summary>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="radiate">The radiation pattern for particle headings.</param>
    /// <returns>A new <see cref="CircleProfile"/> instance.</returns>
    public static Profile Circle(float radius, CircleRadiation radiate)
    {
        return new CircleProfile { Radius = radius, Radiate = radiate };
    }

    /// <summary>
    /// Creates a <see cref="SprayProfile"/> that emits particles in a directional cone.
    /// </summary>
    /// <param name="direction">The central direction of the spray.</param>
    /// <param name="spread">The angular spread of the spray, in radians.</param>
    /// <returns>A new <see cref="SprayProfile"/> instance.</returns>
    public static Profile Spray(Vector2 direction, float spread)
    {
        return new SprayProfile { Direction = direction, Spread = spread };
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return GetType().Name;
    }
}
