// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// A modifier that applies a constant rotational velocity to particles.
/// </summary>
/// <remarks>
/// The <see cref="RotationModifier"/> changes the orientation of particles over time
/// by applying a continuous rotation at a specified rate.
///
/// The rotation is applied uniformly to all particles, but can be combined with other modifiers
/// to create more complex behaviors. For non-uniform rotation, consider using multiple particle
/// emitters with different rotation rates or implementing a custom modifier.
/// </remarks>
public class RotationModifier : Modifier
{
    /// <summary>
    /// Gets or sets the rate at which particles rotate, in radians per second.
    /// </summary>
    /// <remarks>
    /// Positive values cause clockwise rotation, while negative values cause
    /// counter-clockwise rotation.
    /// </remarks>
    public float RotationRate { get; set; }

    /// <summary>
    /// Updates all particles by applying rotation based on the elapsed time.
    /// </summary>
    /// <inheritdoc/>
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) { return; }

        float rotationRateDelta = RotationRate * elapsedSeconds;

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            particle->Rotation += rotationRateDelta;
        }
    }
}
