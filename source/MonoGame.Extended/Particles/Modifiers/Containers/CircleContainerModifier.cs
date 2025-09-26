// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers.Containers;

/// <summary>
/// A modifier that constrains particles within or outside of a circular boundary.
/// </summary>
/// <remarks>
/// The <see cref="CircleContainerModifier"/> keeps particles either inside or outside a circular area, reflecting them
/// at the boundary based on a configurable restitution coefficient. The circle is centered at each particle's trigger
/// position (where it was emitted), creating local containment areas.
/// </remarks>
public class CircleContainerModifier : Modifier
{
    /// <summary>
    /// Gets or sets the radius of the circular container.
    /// </summary>
    public float Radius { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether particles should be contained inside the circle.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///         When <see langword="true"/>, particles are kept inside the circle (bouncing inward at the boundary).
    ///     </item>
    ///     <item>
    ///         When <see langword="false"/>, particles are kept outside the circle (bouncing outward at the boundary).
    ///     </item>
    /// </list>
    ///
    /// The default value is <see langword="true"/>.
    /// </remarks>
    public bool Inside { get; set; } = true;

    /// <summary>
    /// Gets or sets the coefficient of restitution (bounciness) for particle collisions with the boundary.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///         A value of 1.0 creates a perfectly elastic collision where particles maintain their energy.
    ///     </item>
    ///     <item>
    ///         Values less than 1.0 create inelastic collisions where particles lose energy with each bounce.
    ///     </item>
    ///     <item>
    ///         Values greater than 1.0 create super-elastic collisions where particles gain energy.
    ///     </item>
    /// </list>
    ///
    /// The default value is 1.0.
    /// </remarks>
    public float RestitutionCoefficient { get; set; } = 1;

    /// <summary>
    /// Updates all particles by constraining them to the circular boundary.
    /// </summary>
    /// <inheritdoc/>
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) { return; }

        float radiusSq = Radius * Radius;

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            Vector2 localPos;
            localPos.X = particle->Position[0] - particle->TriggeredPos[0];
            localPos.Y = particle->Position[1] - particle->TriggeredPos[1];

            float distSq = localPos.LengthSquared();
            Vector2 normal = Vector2.Normalize(localPos);

            if (Inside)
            {
                if (distSq < radiusSq) { continue; }
                SetReflected(distSq, particle, normal);
            }
            else
            {
                if (distSq > radiusSq) { continue; }
                SetReflected(distSq, particle, -normal);
            }
        }
    }

    private unsafe void SetReflected(float distSq, Particle* particle, Vector2 normal)
    {
        float dist = MathF.Sqrt(distSq);
        float d = dist - Radius;

        float twoRestDot = 2 * RestitutionCoefficient *
                           Vector2.Dot(new Vector2(particle->Velocity[0], particle->Velocity[1]), normal);

        particle->Velocity[0] -= twoRestDot * normal.X;
        particle->Velocity[1] -= twoRestDot * normal.Y;

        // exact computation requires sqrt or goniometrics
        particle->Position[0] -= normal.X * d;
        particle->Position[1] -= normal.Y * d;
    }
}
