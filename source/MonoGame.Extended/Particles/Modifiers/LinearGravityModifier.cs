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
    public Vector2 Direction;

    /// <summary>
    /// Gets or sets the strength of the gravitational force, in units per second squared.
    /// </summary>
    /// <remarks>
    /// This value scales the overall magnitude of the force. Higher values create
    /// stronger acceleration effects, causing particles to change velocity more rapidly.
    /// </remarks>
    public float Strength;

    /// <summary>
    /// Updates all particles by applying a linear gravitational force.
    /// </summary>
    /// <param name="elapsedSeconds">The elapsed time, in seconds, since the last update.</param>
    /// <param name="particle">A pointer to the first particle to update.</param>
    /// <param name="count">The number of particles to update.</param>
    public override unsafe void Update(float elapsedSeconds, Particle* particle, int count)
    {
        Vector2 vector = Direction * (Strength * elapsedSeconds);

        while (count-- > 0)
        {
            particle->Velocity[0] = particle->Velocity[0] + vector.X * particle->Mass;
            particle->Velocity[1] = particle->Velocity[1] + vector.Y * particle->Mass;

            particle++;
        }
    }
}
