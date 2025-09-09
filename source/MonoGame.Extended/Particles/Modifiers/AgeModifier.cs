// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers.Interpolators;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// A modifier that applies multiple interpolators to particles based on their age.
/// </summary>
/// <remarks>
/// The <see cref="AgeModifier"/> controls how particle properties change over their lifetime
/// by applying a collection of <see cref="Interpolator"/> objects to each particle. Each interpolator
/// in the collection operates on a different property of the particle (such as color, scale, or opacity),
/// creating complex, time-based transformations.
///
/// Unlike other modifiers that apply incremental changes each frame, interpolators directly compute
/// the target property values based on the particle's current age as a fraction of its total lifespan.
/// This provides more predictable and consistent results regardless of frame rate.
/// </remarks>
public class AgeModifier : Modifier
{
    /// <summary>
    /// Gets or sets the collection of interpolators that will be applied to particles.
    /// </summary>
    public List<Interpolator> Interpolators { get; set; } = new List<Interpolator>();

    /// <summary>
    /// Updates all particles by applying each interpolator in the collection to each particle.
    /// </summary>
    /// <inheritdoc/>
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) { return; }

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            for (int j = 0; j < Interpolators.Count; j++)
            {
                Interpolators[j].Update(particle->Age, particle);
            }
        }
    }
}
