// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;

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

    private float _frequency;
    private float _cycleTime;
    private int _particlesUpdatedThisCycle;

    /// <summary>
    /// Gets or sets the display name of this modifier.
    /// </summary>
    public string Name { get; set; }

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
    public float Frequency
    {
        get => _frequency;
        set
        {
            if (value <= 0.0f)
                throw new ArgumentOutOfRangeException(nameof(value), "Frequency must be greater than zero.");

            _frequency = value;
            _cycleTime = 1f / _frequency;
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether this modifier is enabled.
    /// </summary>
    /// <remarks>
    /// This value determines if this modifier is enabled.  When a modifier is disabled, the modifier is not applied
    /// to the particles.
    /// </remarks>
    public bool Enabled { get; set; }

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
        Enabled = true;
    }

    internal void InternalUpdate(float elapsedSeconds, ParticleIterator iterator)
    {
        if (!Enabled || iterator.Total == 0)
            return;

        var particlesRemaining = iterator.Total - _particlesUpdatedThisCycle;
        var particlesToUpdate = Math.Min(particlesRemaining, (int)Math.Ceiling((elapsedSeconds / _cycleTime) * iterator.Total));

        if (particlesToUpdate > 0)
        {
            // Create a new iterator starting from the offset position
            var offsetIterator = iterator.Reset(_particlesUpdatedThisCycle);

            Update(_cycleTime, offsetIterator, particlesToUpdate);

            _particlesUpdatedThisCycle += particlesToUpdate;
        }

        if (_particlesUpdatedThisCycle >= iterator.Total)
            _particlesUpdatedThisCycle = 0;
    }

    /// <summary>
    /// Updates the properties of particles according to this modifier's specific behavior.
    /// </summary>
    /// <param name="elapsedSeconds">The elapsed time, in seconds, since the last update.</param>
    /// <param name="iterator">The iterator used to iterate the particles ot update.</param>
    protected internal abstract void Update(float elapsedSeconds, ParticleIterator iterator, int particleCount);
}
