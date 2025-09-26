// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators;

/// <summary>
/// Represents a base class for all particle interpolators.
/// </summary>
/// <remarks>
/// Interpolators are specialized modifiers that gradually change particle properties
/// based on a normalized time value (between 0.0 and 1.0) over the particle's lifetime.
/// This enables smooth transitions between initial and final states, such as color fades,
/// size changes, or opacity adjustments.
/// </remarks>
public abstract class Interpolator
{
    /// <summary>
    /// Gets or sets the display name of this interpolator.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this interpolator is enabled.
    /// </summary>
    /// <remarks>
    /// This value determines if this interpolator is enabled.  When an interpolator is disabled, the interpolator is
    /// not applied to the modifier.
    /// </remarks>
    public bool Enabled { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Interpolator"/> class.
    /// </summary>
    protected Interpolator()
    {
        Name = GetType().Name;
        Enabled = true;
    }

    /// <summary>
    /// Updates a single particle property based on the interpolation amount.
    /// </summary>
    /// <param name="amount">The normalized interpolation amount (from 0.0 to 1.0).</param>
    /// <param name="particle">A pointer to the particle to update.</param>
    /// <remarks>
    /// This method is called for each particle during its lifetime. The <paramref name="amount"/>
    /// parameter represents the particle's age as a fraction of its total lifespan.
    /// </remarks>
    public abstract unsafe void Update(float amount, Particle* particle);
}
