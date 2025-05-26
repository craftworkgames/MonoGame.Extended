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
    public float RotationRate;

    /// <summary>
    /// Updates all particles by applying rotation based on the elapsed time.
    /// </summary>
    /// <param name="elapsedSeconds">The elapsed time, in seconds, since the last update.</param>
    /// <param name="particle">A pointer to the first particle to update.</param>
    /// <param name="count">The number of particles to update.</param>
    public override unsafe void Update(float elapsedSeconds, Particle* particle, int count)
    {
        float rotationRateDelta = RotationRate * elapsedSeconds;

        while (count-- > 0)
        {
            particle->Rotation += rotationRateDelta;
            particle++;
        }
    }
}
