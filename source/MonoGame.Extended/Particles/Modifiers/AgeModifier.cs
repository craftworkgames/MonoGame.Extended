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
    /// <param name="elapsedSeconds">The elapsed time, in seconds, since the last update.</param>
    /// <param name="particle">A pointer to the first particle to update.</param>
    /// <param name="count">The number of particles to update.</param>
    /// <remarks>
    /// This method iterates through all particles and applies each interpolator to every particle,
    /// passing the particle's current age as the interpolation amount. The interpolators then
    /// determine how to transform their respective particle properties based on this age value.
    /// </remarks>
    public override unsafe void Update(float elapsedSeconds, Particle* particle, int count)
    {
        while (count-- > 0)
        {
            for (var i = 0; i < Interpolators.Count; i++)
            {
                Interpolator interpolator = Interpolators[i];
                interpolator.Update(particle->Age, particle);
            }

            particle++;
        }
    }
}
