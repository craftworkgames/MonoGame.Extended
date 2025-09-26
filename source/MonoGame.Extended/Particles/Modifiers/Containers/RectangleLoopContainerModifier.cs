// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers.Containers;

/// <summary>
/// A modifier that constrains particles within a rectangular boundary by wrapping them around to the opposite side.
/// </summary>
/// <remarks>
/// The <see cref="RectangleLoopContainerModifier"/> creates a looping effect by teleporting particles that exit the
/// boundary to the opposite side, similar to classic arcade games where objects wrap around the screen edges. The
/// rectangle is centered at each particle's trigger position (where it was emitted), creating local containment areas.
/// </remarks>
public class RectangleLoopContainerModifier : Modifier
{
    /// <summary>
    /// Gets or sets the width of the rectangular container, in units.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the rectangular container, in units.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Updates all particles by wrapping them around to the opposite side when they cross the rectangular boundary.
    /// </summary>
    /// <inheritdoc/>
    protected internal override unsafe void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount)
    {
        if (!Enabled) { return; }

        for (int i = 0; i < particleCount && iterator.HasNext; i++)
        {
            Particle* particle = iterator.Next();

            var left = particle->TriggeredPos[0] + Width * -0.5f;
            var right = particle->TriggeredPos[0] + Width * 0.5f;
            var top = particle->TriggeredPos[1] + Height * -0.5f;
            var bottom = particle->TriggeredPos[1] + Height * 0.5f;

            var xPos = particle->Position[0];
            var yPos = particle->Position[1];

            if ((int)particle->Position[0] < left)
            {
                xPos = particle->Position[0] + Width;
            }
            else
            {
                if ((int)particle->Position[0] > right)
                {
                    xPos = particle->Position[0] - Width;
                }
            }

            if ((int)particle->Position[1] < top)
            {
                yPos = particle->Position[1] + Height;
            }
            else
            {
                if ((int)particle->Position[1] > bottom)
                {
                    yPos = particle->Position[1] - Height;
                }
            }

            particle->Position[0] = xPos;
            particle->Position[1] = yPos;
        }
    }
}
