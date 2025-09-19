using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// A modifier that creates vortex effects by applying rotated gravitational forces to particles.
/// </summary>
/// <remarks>
///     <para>
///         The <see cref="VortexModifier"/> generates spiral motion by rotating gravitational attraction vectors
///         around a central point. Unlike pure gravitational attraction, this creates swirling, orbital, and
///         spiral-out effects depending on the rotation angle applied to the force vectors.
///     </para>
///     <para>
///         Forces are strongest at the <see cref="InnerRadius"/> and weakest at the <see cref="OuterRadius"/>,
///         following a linear inverse relationship with distance.
///     </para>
/// </remarks>
public unsafe class VortexModifier : Modifier
{
    private float _rotationAngle;
    private float _cosAngle;
    private float _sinAngle;

    /// <summary>
    /// Gets or sets the position of the vortex center relative to particle emission points.
    /// </summary>
    public Vector2 Position { get; set; } = Vector2.Zero;

    /// <summary>
    /// Gets or sets the force strength applied to particles at the outer radius.
    /// </summary>
    /// <value>The acceleration in units per second squared applied at the <see cref="OuterRadius"/>.</value>
    /// <remarks>
    /// This value represents the acceleration magnitude applied to particles at the vortex edge.
    /// Particles closer to the center experience proportionally stronger forces. The scaling
    /// follows the formula: <c>actualForce = Strength × (OuterRadius / particleDistance)</c>.
    /// </remarks>
    public float Strength { get; set; }

    /// <summary>
    /// Gets or sets the maximum distance from the vortex center where forces are applied.
    /// </summary>
    /// <remarks>
    /// Particles beyond this radius are unaffected by the vortex. This distance also serves
    /// as the reference point for force strength calculations, where particles at this exact
    /// distance experience the base <see cref="Strength"/> value.
    /// </remarks>
    public float OuterRadius { get; set; }

    /// <summary>
    /// Gets or sets the minimum distance from the vortex center where forces are applied.
    /// </summary>
    /// <remarks>
    /// Creates a dead zone around the vortex center where particles are unaffected.
    /// Prevents extreme force magnitudes and simulation instability when particles
    /// get very close to the center point.
    /// </remarks>
    public float InnerRadius { get; set; }

    /// <summary>
    /// Gets or sets the maximum velocity magnitude that particles can reach under vortex influence.
    /// </summary>
    /// <value>The speed limit in units per second.</value>
    /// <remarks>
    /// Particle velocities are clamped to this magnitude after vortex forces are applied.
    /// Prevents runaway acceleration and maintains visual stability when particles
    /// accumulate high velocities through repeated vortex acceleration.
    /// </remarks>
    public float MaxVelocity { get; set; }

    /// <summary>
    /// Gets or sets the rotation angle, in radians, applied to gravitational force vectors.
    /// </summary>
    /// <remarks>
    /// This angle determines the motion pattern created by the vortex
    /// <list type="table">
    /// <listheader><term>Angle</term><description>description</description></listheader>
    /// <item><term>0°</term><description>Pure gravitational attraction (particles pulled straight inward)</description></item>
    /// <item><term>Small angles (5-20°)</term><description>Inward spirals and temporary orbital motion</description></item>
    /// <item><term>Medium angles (30-60°)</term><description>Wide deflection arcs around the vortex</description></item>
    /// <item><term>Large angles (90°+)</term><description>Particles to deflect around the vortex perimeter without entering</description></item>
    /// </list>
    /// Positive values create counterclockwise rotation, where as negative values create clockwise rotation.
    /// </remarks>
    public float RotationAngle
    {
        get => _rotationAngle;
        set
        {
            if (_rotationAngle == value)
            {
                return;
            }

            _rotationAngle = value;

            // Precalculate cos and sin angle so we're not doing it each
            // modifier loop.
            _cosAngle = MathF.Cos(_rotationAngle);
            _sinAngle = MathF.Sin(_rotationAngle);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VortexModifier"/> class.
    /// </summary>
    public VortexModifier()
    {
        RotationAngle = 0.0f;
    }

    /// <inheritdoc />
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) return;

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            float vortexX = particle->TriggeredPos[0] + Position.X;
            float vortexY = particle->TriggeredPos[1] + Position.Y;

            float dx = particle->Position[0] - vortexX;
            float dy = particle->Position[1] - vortexY;
            float distance = MathF.Sqrt(dx * dx + dy * dy);

            if (distance < InnerRadius || distance > OuterRadius)
            {
                continue;
            }

            float gravityX = -dx / distance;
            float gravityY = -dy / distance;

            float rotatedX = gravityX * _cosAngle - gravityY * _sinAngle;
            float rotatedY = gravityX * _sinAngle + gravityY * _cosAngle;

            // Strength is the force applied at the outer edge
            // Closer particles get proportionally stronger force
            float distanceRatio = OuterRadius / distance;
            float forceStrength = Strength * distanceRatio;

            particle->Velocity[0] += rotatedX * forceStrength * elapsedSeconds;
            particle->Velocity[1] += rotatedY * forceStrength * elapsedSeconds;

            // Clamp total velocity to max speed
            float velocityMagnitude = MathF.Sqrt(particle->Velocity[0] * particle->Velocity[0] +
                                               particle->Velocity[1] * particle->Velocity[1]);

            if (velocityMagnitude > MaxVelocity)
            {
                float scale = MaxVelocity / velocityMagnitude;
                particle->Velocity[0] *= scale;
                particle->Velocity[1] *= scale;
            }
        }
    }
}
