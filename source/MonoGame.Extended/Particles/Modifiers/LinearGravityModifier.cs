// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// A modifier that applies a constant directional force to particles, simulating gravity or wind.
/// </summary>
/// <remarks>
/// The <see cref="LinearGravityModifier"/> applies a uniform acceleration in a specified direction
/// to all particles, creating effects such as gravity, wind, or other constant forces. The force
/// is applied proportionally to each particle's mass, simulating realistic physical behavior.
///
/// Note that this modifier only changes particle velocities; the actual position changes
/// occur during the standard particle update cycle.
/// </remarks>
public class LinearGravityModifier : Modifier
{
    /// <summary>
    /// Gets or sets the direction vector of the gravitational force.
    /// </summary>
    /// <remarks>
    /// This vector defines both the direction and the relative magnitude of the force.
    /// </remarks>
    public Vector2 Direction { get; set; }

    /// <summary>
    /// Gets or sets the strength of the gravitational force, in units per second squared.
    /// </summary>
    /// <remarks>
    /// This value scales the overall magnitude of the force. Higher values create
    /// stronger acceleration effects, causing particles to change velocity more rapidly.
    /// </remarks>
    public float Strength { get; set; }

    /// <summary>
    /// Updates all particles by applying a linear gravitational force.
    /// </summary>
    /// <inheritdoc/>
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) { return; }

        Vector2 vector = Direction * (Strength * elapsedSeconds);

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            particle->Velocity[0] = particle->Velocity[0] + vector.X * particle->Mass;
            particle->Velocity[1] = particle->Velocity[1] + vector.Y * particle->Mass;
        }
    }
}
