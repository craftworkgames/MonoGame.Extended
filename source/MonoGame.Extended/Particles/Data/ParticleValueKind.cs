// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace MonoGame.Extended.Particles.Data;

/// <summary>
/// Defines how particle property values are determined during particle creation and simulation.
/// </summary>
/// <remarks>
/// This enum is used throughout the particle system to specify whether values should be constant or randomly generated
/// within a specified range.
/// </remarks>
public enum ParticleValueKind
{
    /// <summary>
    /// Indicates that a particle property should maintain a constant value.
    /// </summary>
    /// <remarks>
    /// WHen a particle property uses this kind, all particles will have the same value for that property, which remains
    /// unchanged unless explicitly modified.
    /// </remarks>
    Constant,

    /// <summary>
    /// Indicates that a particle property should be randomly generated within a specified range.
    /// </summary>
    /// <remarks>
    /// When a particle property uses this kind, each particle will receive a unique random value within the defined
    /// minimum and maximum bounds when the particle is created.
    /// </remarks>
    Random
}
