// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// Represents a base class for all particle modifiers.
/// </summary>
/// <remarks>
/// Particle modifiers are used to alter the behavior or properties of particles during their lifetime.
/// Each modifier applies changes to particles at a configurable frequency, optimizing performance by
/// spreading updates across frames when appropriate.
/// Custom modifiers should inherit from this class and implement the <see cref="Update"/> method.
/// </remarks>
public abstract class Modifier
{
    private const float DEFAULT_MODIFIER_FREQUENCY = 60.0f;

    private int _particlesUpdatedThisCycle;

    /// <summary>
    /// Gets or sets the display name of this modifier.
    /// </summary>
    public string Name;

    /// <summary>
    /// Gets or sets the update frequency of this modifier.
    /// </summary>
    /// <remarks>
    /// This value defines how often, in times per second, the modifier attempts to update
    /// the entire particle buffer. For example, a value of 60.0f means that all particles
    /// will be updated collectively approximately 60 times per second.
    ///
    /// To improve performance, updates are distributed across frames. Rather than updating
    /// every particle in every frame, the modifier mathematically distributes updates by
    /// processing a portion of the particles each frame based on the elapsed time and the
    /// desired frequency. Over time, this results in all particles being updated at the
    /// specified frequency on average, regardless of the actual frame rate.
    ///
    /// Higher values result in more frequent updates and smoother particle behavior, at the
    /// cost of performance. Lower values reduce CPU usage but may make particle changes appear
    /// less fluid.
    /// </remarks>
    public float Frequency;

    /// <summary>
    /// Initializes a new instance of the <see cref="Modifier"/> class.
    /// </summary>
    /// <remarks>
    /// The default constructor sets the <see cref="Name"/> property to the name of the derived class
    /// and initializes <see cref="Frequency"/> to <see cref="DEFAULT_MODIFIER_FREQUENCY"/>.
    /// </remarks>
    protected Modifier()
    {
        Name = GetType().Name;
        Frequency = DEFAULT_MODIFIER_FREQUENCY;
    }

    /// <summary>
    /// Updates the properties of particles according to this modifier's specific behavior.
    /// </summary>
    /// <param name="elapsedSeconds">The elapsed time, in seconds, since the last update.</param>
    /// <param name="particle">A pointer to the first particle to update in the buffer.</param>
    /// <param name="count">The number of particles to update.</param>
    /// <remarks>
    /// This method is called by <see cref="InternalUpdate"/> with an appropriate subset of particles.
    /// Derived classes must implement this method to define how particles are modified.
    /// The method operates directly on memory for performance reasons and should be implemented carefully.
    /// </remarks>
    public abstract unsafe void Update(float elapsedSeconds, Particle* particle, int count);

    /// <summary>
    /// Manages the update cycle for particles based on the modifier's frequency.
    /// </summary>
    /// <param name="elapsedSeconds">The elapsed time, in seconds, since the last update.</param>
    /// <param name="buffer">A pointer to the beginning of the particle buffer.</param>
    /// <param name="count">The total number of particles in the buffer.</param>
    /// <remarks>
    /// This method implements the frequency-based update distribution algorithm. Instead of
    /// updating all particles every frame, it calculates how many particles should be
    /// processed during the current frame based on:
    ///
    /// 1. The desired update frequency (<see cref="Frequency"/>)
    /// 2. The elapsed time since the last frame
    /// 3. The total number of particles
    ///
    /// This approach ensures that over time, each particle is updated at the specified
    /// frequency regardless of the actual frame rate, while spreading the computational
    /// load across multiple frames. The method tracks which particles have been updated
    /// in the current cycle using <see cref="_particlesUpdatedThisCycle"/> and resets
    /// once all particles have been processed.
    /// </remarks>
    internal unsafe void InternalUpdate(float elapsedSeconds, Particle* buffer, int count)
    {
        float cycleTime = 1.0f / Frequency;
        int particlesRemaining = count - _particlesUpdatedThisCycle;
        int particlesToUpdate = Math.Min(particlesRemaining, (int)Math.Ceiling((elapsedSeconds / cycleTime) * count));

        if (particlesToUpdate > 0)
        {
            Update(cycleTime, buffer + _particlesUpdatedThisCycle, particlesToUpdate);
            _particlesUpdatedThisCycle += particlesToUpdate;
        }

        if (_particlesUpdatedThisCycle >= count)
        {
            _particlesUpdatedThisCycle = 0;
        }
    }
}
