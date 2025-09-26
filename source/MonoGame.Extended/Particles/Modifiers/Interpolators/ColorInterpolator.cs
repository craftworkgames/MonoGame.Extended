// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators;

/// <summary>
/// An interpolator that gradually changes particle color properties over their lifetime.
/// </summary>
/// <remarks>
///     <para>
///         The <see cref="ColorInterpolator"/> transitions a particle's color from the inherited
///         <see cref="Interpolator{T}.StartValue"/> to <see cref="Interpolator{T}.EndValue"/> based on the
///         provided interpolation amount
///     </para>
///     <para>
///         Color values are represented in the HSL (Hue, Saturation, Lightness) color space as a Vector3.
///     </para>
/// </remarks>
public sealed class ColorInterpolator : Interpolator<HslColor>
{
    /// <summary>
    /// Updates a particle's color by interpolating between the start and end values.
    /// </summary>
    /// <param name="amount">The normalized interpolation amount (from 0.0 to 1.0).</param>
    /// <param name="particle">A pointer to the particle to update.</param>
    public override unsafe void Update(float amount, Particle* particle)
    {
        if (!Enabled) { return; }

        float h = StartValue.H + (EndValue.H - StartValue.H) * amount;
        float s = StartValue.S + (EndValue.S - StartValue.S) * amount;
        float l = StartValue.L + (EndValue.L - StartValue.L) * amount;

        particle->Color[0] = h;
        particle->Color[1] = s;
        particle->Color[2] = l;
    }
}
