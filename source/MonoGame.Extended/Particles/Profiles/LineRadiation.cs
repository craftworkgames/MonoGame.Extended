namespace MonoGame.Extended.Particles.Profiles;

/// <summary>
/// Defines the radiation pattern for particles when using a <see cref="LineProfile"/>.
/// </summary>
/// <remarks>
/// This enumeration determines how a particle's initial heading is calculated relative to the line segment.
/// Different radiation patterns enable various effects such as rain, fountains, or chaotic dispersion.
/// </remarks>
public enum LineRadiation
{
    /// <summary>
    /// Particles move in random directions unrelated to their position.
    /// </summary>
    /// <remarks>
    /// In this mode, the initial heading of particles is completely random and has no relationship to their position
    /// along the line.
    /// </remarks>
    None,

    /// <summary>
    /// Particles move in the direction specified by the <see cref="LineProfile.Direction"/> property.
    /// </summary>
    /// <remarks>
    /// In this mode, all particles are given the same initial heading as specified by the Direction vector. This allows
    /// complete control over particle movement direction regardless of the line's orientation.
    /// </remarks>
    Directional,

    /// <summary>
    /// Particles move perpendicular to the line axis in the upward screen direction.
    /// </summary>
    /// <remarks>
    /// In this mode, particles are given initial headings perpendicular to the line's axis, pointing upward in screen
    /// coordinates (negative Y direction).
    /// </remarks>
    PerpendicularUp,

    /// <summary>
    /// Particles move perpendicular to the line axis in the downward screen direction.
    /// </summary>
    /// <remarks>
    /// In this mode, particles are given initial headings perpendicular to the line's axis, pointing downward in screen
    /// coordinates (positive Y direction).
    /// </remarks>
    PerpendicularDown
}
