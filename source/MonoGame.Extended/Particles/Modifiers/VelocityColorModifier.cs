// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// A modifier that changes particle colors based on their velocity.
/// </summary>
/// <remarks>
/// The <see cref="VelocityColorModifier"/> adjusts particle colors dynamically according to their
/// movement speed. Particles can transition smoothly between two defined colors:
/// <list type="bullet">
///   <item>A color for stationary or slow-moving particles (<see cref="StationaryColor"/>)</item>
///   <item>A color for fast-moving particles (<see cref="VelocityColor"/>)</item>
/// </list>
///
/// The color values are represented in HSL (Hue, Saturation, Lightness) format as a Vector3.
/// </remarks>
public class VelocityColorModifier : Modifier
{
    /// <summary>
    /// Gets or sets the color for particles that are stationary or moving slowly.
    /// </summary>
    /// <remarks>
    /// This color is applied to particles with zero velocity and serves as the starting
    /// point for color interpolation.
    /// </remarks>
    public HslColor StationaryColor { get; set; }

    /// <summary>
    /// Gets or sets the color for particles that have reached or exceeded the velocity threshold.
    /// </summary>
    /// <remarks>
    /// This color is applied to fast-moving particles and serves as the end point
    /// for color interpolation.
    /// </remarks>
    public HslColor VelocityColor { get; set; }

    /// <summary>
    /// Gets or sets the velocity magnitude at which particles fully transition to the velocity color.
    /// </summary>
    /// <remarks>
    /// This value defines the speed threshold that determines when a particle should
    /// display the full <see cref="VelocityColor"/>. Particles moving slower than this
    /// threshold will display a color interpolated between <see cref="StationaryColor"/> and
    /// <see cref="VelocityColor"/> based on their speed relative to this threshold.
    /// </remarks>
    public float VelocityThreshold { get; set; }

    /// <summary>
    /// Updates all particles by changing their colors based on their current velocity.
    /// </summary>
    /// <inheritdoc/>
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) { return; }

        float velocityThreshold2 = VelocityThreshold * VelocityThreshold;

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            float velocitySquared = particle->Velocity[0] * particle->Velocity[0] +
                                    particle->Velocity[1] * particle->Velocity[1];

            if (velocitySquared >= velocityThreshold2)
            {
                particle->Color[0] = VelocityColor.H;
                particle->Color[1] = VelocityColor.S;
                particle->Color[2] = VelocityColor.L;
            }
            else
            {
                HslColor deltaColor = VelocityColor - StationaryColor;
                float t = MathF.Sqrt(velocitySquared) / VelocityThreshold;

                float h = deltaColor.H * t + StationaryColor.H;
                float s = deltaColor.S * t + StationaryColor.S;
                float l = deltaColor.L * t + StationaryColor.L;

                particle->Color[0] = h;
                particle->Color[1] = s;
                particle->Color[2] = l;
            }
        }
    }
}
