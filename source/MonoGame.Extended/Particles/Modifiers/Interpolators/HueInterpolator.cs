// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators;

/// <summary>
/// An interpolator that gradually changes only the hue component of particle colors over their lifetime.
/// </summary>
/// <remarks>
/// Unlike <see cref="ColorInterpolator"/> which changes all HSL components, this interpolator
/// affects only the hue component, preserving the particle's existing saturation and lightness values.
/// This allows for color cycling effects while maintaining consistent saturation and brightness.
/// </remarks>
public class HueInterpolator : Interpolator<float>
{
    /// <summary>
    /// Updates a particle's hue by interpolating between the start and end values.
    /// </summary>
    /// <param name="amount">The normalized interpolation amount (from 0.0 to 1.0).</param>
    /// <param name="particle">A pointer to the particle to update.</param>
    public override unsafe void Update(float amount, Particle* particle)
    {
        if (!Enabled) { return; }

        float h = StartValue + (EndValue - StartValue) * amount;
        particle->Color[0] = h;
    }
}
