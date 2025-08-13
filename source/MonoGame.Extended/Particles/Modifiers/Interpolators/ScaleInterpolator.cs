// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators;

/// <summary>
/// An interpolator that gradually changes the size of particles over their lifetime.
/// </summary>
/// <remarks>
/// The <see cref="ScaleInterpolator"/> transitions a particle's scale factor from the inherited
/// <see cref="Interpolator{T}.StartValue"/> to <see cref="Interpolator{T}.EndValue"/> based on the
/// provided interpolation amount (typically representing the particle's normalized age).
/// </remarks>
public class ScaleInterpolator : Interpolator<Vector2>
{
    /// <summary>
    /// Updates a particle's scale by interpolating between the start and end values.
    /// </summary>
    /// <param name="amount">The normalized interpolation amount (from 0.0 to 1.0).</param>
    /// <param name="particle">A pointer to the particle to update.</param>
    public override unsafe void Update(float amount, Particle* particle)
    {
        if (!Enabled) { return; }

        particle->Scale[0] = StartValue.X + (EndValue.X - StartValue.X) * amount;
        particle->Scale[1] = StartValue.Y + (EndValue.Y - StartValue.Y) * amount;
    }
}
