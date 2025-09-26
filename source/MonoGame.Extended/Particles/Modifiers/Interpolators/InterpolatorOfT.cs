// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace MonoGame.Extended.Particles.Modifiers.Interpolators;

/// <summary>
/// Represents a generic base class for particle interpolators that work with specific value types.
/// </summary>
/// <typeparam name="T">The type of value being interpolated. Must be a value type.</typeparam>
public abstract class Interpolator<T> : Interpolator where T : struct
{
    /// <summary>
    /// Gets or sets the starting value for the interpolation.
    /// </summary>
    public T StartValue  { get; set; }

    /// <summary>
    /// Gets or sets the ending value for the interpolation.
    /// </summary>
    public T EndValue  { get; set; }
}
