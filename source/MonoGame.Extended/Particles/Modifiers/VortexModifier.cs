// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// A modifier that creates a gravitational vortex effect, pulling particles toward a central point.
/// </summary>
/// <remarks>
/// The <see cref="VortexModifier"/> simulates a gravitational attraction between a central point and
/// each particle. The strength of the attraction is based on gravitational principles, where the force
/// is proportional to the masses and inversely proportional to the square of the distance.
///
/// The actual acceleration applied to each particle is clamped to prevent particles from moving
/// too quickly when they get close to the center of the vortex.
/// </remarks>
public unsafe class VortexModifier : Modifier
{
    private const float GRAVITY = 100000f;

    /// <summary>
    /// Gets or sets the center position of the vortex in 2D space.
    /// </summary>
    /// <remarks>
    /// Particles will be attracted toward this point.
    /// </remarks>
    public Vector2 Position;

    /// <summary>
    /// Gets or sets the mass of the vortex center.
    /// </summary>
    /// <remarks>
    /// Higher mass values create stronger attraction forces.
    /// </remarks>
    public float Mass;

    /// <summary>
    /// Gets or sets the maximum speed that the vortex can impart on particles.
    /// </summary>
    /// <remarks>
    /// This value limits how quickly particles can be accelerated by the vortex,
    /// preventing extreme velocities when particles get very close to the center.
    /// </remarks>
    public float MaxSpeed;

    /// <summary>
    /// Updates all particles by applying a gravitational force towards the vortex center.
    /// </summary>
    /// <inheritdoc/>
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) { return; }

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            float distx = Position.X - particle->Position[0];
            float disty = Position.Y - particle->Position[1];

            float distance2 = (distx * distx) + (disty * disty);
            float distance = (float)Math.Sqrt(distance2);

            float m = (GRAVITY * Mass * particle->Mass) / distance2;
            m = Math.Max(Math.Min(m, MaxSpeed), -MaxSpeed) * elapsedSeconds;

            distx = (distx / distance) * m;
            disty = (disty / distance) * m;

            particle->Velocity[0] += distx;
            particle->Velocity[1] += disty;
        }
    }
}
