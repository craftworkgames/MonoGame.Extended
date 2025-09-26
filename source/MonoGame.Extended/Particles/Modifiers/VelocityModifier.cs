// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers.Interpolators;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// A modifier that applies interpolators to particles based on their velocity magnitude.
/// </summary>
/// <remarks>
/// The <see cref="VelocityModifier"/> controls how particle properties change based on their speed
/// by applying a collection of <see cref="Interpolator"/> objects to each particle. The intensity
/// of the effect is determined by comparing the particle's velocity magnitude to a threshold value.
/// </remarks>
public class VelocityModifier : Modifier
{
    /// <summary>
    /// Gets or sets the collection of interpolators that will be applied to particles.
    /// </summary>
    public List<Interpolator> Interpolators { get; set; } = new List<Interpolator>();

    /// <summary>
    /// Gets or sets the velocity magnitude at which particles reach the maximum interpolation effect.
    /// </summary>
    /// <remarks>
    /// This value defines the speed threshold that determines when a particle should
    /// receive the full interpolation effect (amount = 1.0). Particles moving slower than this
    /// threshold will receive a proportionally reduced effect based on their velocity magnitude.
    /// </remarks>
    public float VelocityThreshold { get; set; }

    /// <summary>
    /// Updates all particles by applying interpolators with an amount based on each particle's velocity.
    /// </summary>
    /// <inheritdoc />
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
                for (int j = 0; j < Interpolators.Count; j++)
                {
                    Interpolator interpolator = Interpolators[j];
                    interpolator.Update(1, particle);
                }
            }
            else
            {
                float t = (float)Math.Sqrt(velocitySquared) / VelocityThreshold;

                for (int j = 0; j < Interpolators.Count; j++)
                {
                    Interpolator interpolator = Interpolators[j];
                    interpolator.Update(t, particle);
                }
            }
        }
    }
}
