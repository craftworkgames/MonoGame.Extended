// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators;

/// <summary>
/// An interpolator that gradually changes the rotation angle of particles over their lifetime.
/// </summary>
/// <remarks>
/// The <see cref="RotationInterpolator"/> transitions a particle's rotation value from the inherited
/// <see cref="Interpolator{T}.StartValue"/> to <see cref="Interpolator{T}.EndValue"/> based on the
/// provided interpolation amount.
/// </remarks>
public class RotationInterpolator : Interpolator<float>
{
    /// <summary>
    /// Updates a particle's rotation by interpolating between the start and end values.
    /// </summary>
    /// <param name="amount">The normalized interpolation amount (from 0.0 to 1.0).</param>
    /// <param name="particle">A pointer to the particle to update.</param>
    public override unsafe void Update(float amount, Particle* particle)
    {
        if (!Enabled) { return; }

        particle->Rotation = StartValue + (EndValue - StartValue) * amount;
    }
}
