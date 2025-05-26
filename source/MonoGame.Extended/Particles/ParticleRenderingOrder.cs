// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace MonoGame.Extended.Particles;

/// <summary>
/// Specifies the order in which particles are rendered within the particle system.
/// </summary>
/// <remarks>
/// This enumeration defines the rendering order strategies that can be applied to particles to control how they are
/// layered visually. The chosen rendering order affects which particles appear in front of others when they overlap.
/// </remarks>
public enum ParticleRenderingOrder
{
    /// <summary>
    /// Particles are rendered from front to back.
    /// </summary>
    FrontToBack,

    /// <summary>
    /// Particles are rendered from back to front.
    /// </summary>
    BackToFront
}
