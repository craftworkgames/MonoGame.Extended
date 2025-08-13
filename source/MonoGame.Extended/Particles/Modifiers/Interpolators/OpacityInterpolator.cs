// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators;

/// <summary>
/// An interpolator that gradually changes the opacity of particles over their lifetime.
/// </summary>
/// <remarks>
///     <para>
///         The <see cref="OpacityInterpolator"/> transitions a particle's opacity (alpha) value from the inherited
///         <see cref="Interpolator{T}.StartValue"/> to <see cref="Interpolator{T}.EndValue"/> based on the
///         provided interpolation amount.
///     </para>
///     <para>
///         Valid opacity values range from 0.0 (completely transparent) to 1.0 (completely opaque).
///     </para>
/// </remarks>
public class OpacityInterpolator : Interpolator<float>
{
    /// <summary>
    /// Updates a particle's opacity by interpolating between the start and end values.
    /// </summary>
    /// <param name="amount">The normalized interpolation amount (from 0.0 to 1.0).</param>
    /// <param name="particle">A pointer to the particle to update.</param>
    public override unsafe void Update(float amount, Particle* particle)
    {
        if (!Enabled) { return; }

        particle->Opacity = StartValue + (EndValue - StartValue) * amount;
    }
}
