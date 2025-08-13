// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// Defines the radiation pattern for particles when using a <see cref="CircleProfile"/>.
/// </summary>
/// <remarks>
/// This enumeration determines how a particle's initial position within a circle affects its movement direction
/// (heading). Different radiation patterns can create varied visual effects such as explosions, implosions, or
/// randomized dispersion.
/// </remarks>
public enum CircleRadiation
{
    /// <summary>
    /// Particles move in random directions unrelated to their position.
    /// </summary>
    /// <remarks>
    /// In this mode, the initial heading of particles is completely random and has no relationship to their position
    /// within the circle. This creates a chaotic dispersion effect with no discernible pattern of movement.
    /// </remarks>
    None,

    /// <summary>
    /// Particles move toward the center of the circle.
    /// </summary>
    /// <remarks>
    /// In this mode, particles are given initial headings that point directly toward the center of the circle from
    /// their starting position. This creates an implosion or suction effect, as if particles are being drawn inward.
    /// </remarks>
    In,

    /// <summary>
    /// Particles move away from the center of the circle.
    /// </summary>
    /// <remarks>
    /// In this mode, particles are given initial headings that point directly away from the center of the circle,
    /// extending their starting position outward. This creates an explosion or burst effect, as if particles are
    /// emanating from a central point.
    /// </remarks>
    Out
}
