// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// A modifier that applies fluid resistance (drag) to particles, gradually reducing their velocity.
/// </summary>
/// <remarks>
/// The <see cref="DragModifier"/> simulates the effect of particles moving through a fluid medium
/// such as air, water, or another substance. This creates a damping effect that slows particles over time,
/// with the slowdown proportional to their velocity, mass, and the properties of the medium.
/// </remarks>
public class DragModifier : Modifier
{
    /// <summary>
    /// Gets or sets the drag coefficient, representing the aerodynamic or hydrodynamic properties of particles.
    /// </summary>
    /// <remarks>
    /// The drag coefficient is a dimensionless quantity used in fluid dynamics to model
    /// the resistance of an object moving through a fluid. Higher values create stronger
    /// drag effects, causing particles to slow down more quickly.
    ///
    /// For reference to approximate real-world drag cooeficients, see
    /// <see href="https://en.wikipedia.org/wiki/Drag_coefficient"/>.
    ///
    /// The default value is 0.47.
    /// </remarks>
    public float DragCoefficient  { get; set; } = 0.47f;

    /// <summary>
    /// Gets or sets the density of the fluid medium, affecting the strength of the drag force.
    /// </summary>
    /// <remarks>
    /// This value represents the density of the fluid through which particles are moving.
    /// Higher values create stronger drag effects, simulating denser media like water or oil.
    ///
    /// For reference to approximate real-world density values for various fluids, see
    /// <see href="https://en.wikipedia.org/wiki/Density#Various_materials"/>.
    ///
    /// The default value is 0.5.
    /// </remarks>
    public float Density  { get; set; } = .5f;

    /// <summary>
    /// Updates all particles by applying drag forces based on their velocity.
    /// </summary>
    /// <inheritdoc/>
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) { return; }

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            var drag = -DragCoefficient * Density * particle->Mass * elapsedSeconds;

            particle->Velocity[0] = particle->Velocity[0] + particle->Velocity[0] * drag;
            particle->Velocity[1] = particle->Velocity[1] + particle->Velocity[1] * drag;
        }
    }
}
