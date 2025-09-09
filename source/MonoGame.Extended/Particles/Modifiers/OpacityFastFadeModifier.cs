// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// A modifier that rapidly decreases particle opacity based on their age.
/// </summary>
/// <remarks>
/// The <see cref="OpacityFastFadeModifier"/> creates a linear fade-out effect where particles
/// become more transparent as they age.
///
/// Important notes:
/// <list type="bullet">
///   <item>
///     This modifier assumes particles have a standard lifespan of 1.0 second. For particles
///     with different lifespans, the fade effect may not complete before the particle is removed,
///     or may become fully transparent before the particle's actual end of life.
///   </item>
///   <item>
///     Unlike other modifiers that accumulate changes over time, this modifier directly sets
///     the opacity value each frame based solely on the particle's age.
///   </item>
/// </list>
/// </remarks>
public sealed class OpacityFastFadeModifier : Modifier
{
    /// <summary>
    /// Updates all particles by setting their opacity based on their age.
    /// </summary>
    /// <inheritdoc/>
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) { return; }

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            particle->Opacity = 1.0f - particle->Age;
        }
    }
}
