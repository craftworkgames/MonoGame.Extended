// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers.Containers;

/// <summary>
/// A modifier that constrains particles within a rectangular boundary.
/// </summary>
/// <remarks>
/// The <see cref="RectangleContainerModifier"/> keeps particles inside a rectangular area, reflecting them off the
/// boundaries based on a configurable restitution coefficient. The rectangle is centered at each particle's trigger
/// position (where it was emitted), creating local containment areas.
/// </remarks>
public sealed class RectangleContainerModifier : Modifier
{
    /// <summary>
    /// Gets or sets the width of the rectangular container.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the rectangular container, in units.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the coefficient of restitution (bounciness) for particle
    /// collisions with the boundary.
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
    public float RestitutionCoefficient { get; set; } = 1.0f;

    /// <summary>
    /// Updates all particles by constraining them to the rectangular boundary.
    /// </summary>
    /// <inheritdoc/>
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) { return; }

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            float left = particle->TriggeredPos[0] + Width * -0.5f;
            float right = particle->TriggeredPos[0] + Width * 0.5f;
            float top = particle->TriggeredPos[1] + Height * -0.5f;
            float bottom = particle->TriggeredPos[1] + Height * 0.5f;

            float xPos = particle->Position[0];
            float xVel = particle->Velocity[0];
            float yPos = particle->Position[1];
            float yVel = particle->Velocity[1];

            if ((int)particle->Position[0] < left)
            {
                xPos = left + (left - xPos);
                xVel = -xVel * RestitutionCoefficient;
            }
            else
            {
                if (particle->Position[0] > right)
                {
                    xPos = right - (xPos - right);
                    xVel = -xVel * RestitutionCoefficient;
                }
            }

            if (particle->Position[1] < top)
            {
                yPos = top + (top - yPos);
                yVel = -yVel * RestitutionCoefficient;
            }
            else
            {
                if ((int)particle->Position[1] > bottom)
                {
                    yPos = bottom - (yPos - bottom);
                    yVel = -yVel * RestitutionCoefficient;
                }
            }

            particle->Position[0] = xPos;
            particle->Position[1] = yPos;
            particle->Velocity[0] = xVel;
            particle->Velocity[1] = yVel;
        }
    }
}
