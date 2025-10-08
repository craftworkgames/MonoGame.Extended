// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Primitives;
using MonoGame.Extended.Particles.Profiles;

namespace MonoGame.Extended.Particles;

/// <summary>
/// Represents a particle emitter that creates, manages, and updates particles within a particle system.
/// </summary>
/// <remarks>
/// The <see cref="ParticleEmitter"/> class is the core component of the particle system. It handles particle
/// creation (triggering), lifecycle management, and application of modifiers according to defined profiles
/// and parameters. Each emitter operates independently and can be configured with different behaviors, appearances,
/// and physical properties.
/// </remarks>
public sealed unsafe class ParticleEmitter : IDisposable
{
    private float _totalSeconds;
    private float _secondsSinceLastReclaim;
    private ParticleBuffer _buffer;

    /// <summary>
    /// Gets the buffer that stores and manages the particles for this emitter.
    /// </summary>
    public ParticleBuffer Buffer => _buffer;

    /// <summary>
    /// Gets or sets the name of this emitter, used for identification and debugging.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets the maximum number of particles that this emitter can manage.
    /// </summary>
    /// <value>The size of the underlying <see cref="ParticleBuffer"/>.</value>
    public int Capacity
    {
        get
        {
            return Buffer.Size;
        }
    }

    /// <summary>
    /// Gets the current number of active particles in this emitter.
    /// </summary>
    /// <value>The count of particles in the underlying <see cref="ParticleBuffer"/>.</value>
    public int ActiveParticles
    {
        get
        {
            return Buffer.Count;
        }
    }

    /// <summary>
    /// Gets or sets the lifespan of particles emitted by this emitter, in seconds.
    /// </summary>
    /// <remarks>
    /// After a particle's age exceeds this value, it will be automatically reclaimed during the next cleanup cycle.
    /// </remarks>
    public float LifeSpan { get; set; }

    /// <summary>
    /// Gets or sets the position offset applied to this emitter.
    /// </summary>
    /// <remarks>
    /// This offset is applied to the emitter's position when triggering particles, allowing for fine adjustment
    /// of the emission point without changing the overall position passed to the <see cref="Update"/> method.
    /// </remarks>
    public Vector2 Offset { get; set; }

    /// <summary>
    /// Gets or sets the default layer depth for particles emitted by this emitter.
    /// </summary>
    /// <remarks>
    /// This value determines the rendering order of particles relative to other sprites and particles.
    /// Values range from 0.0 (front) to 1.0 (back).
    /// </remarks>
    public float LayerDepth { get; set; }

    /// <summary>
    /// Gets or sets the frequency, in times per second, at which expired particles are reclaimed.
    /// </summary>
    /// <remarks>
    /// Higher values result in more frequent cleanup of expired particles, potentially improving memory
    /// utilization at the cost of slightly increased CPU usage.
    /// </remarks>
    public float ReclaimFrequency { get; set; }

    /// <summary>
    /// Gets or sets the parameters that control the physical and visual properties of emitted particles.
    /// </summary>
    /// <remarks>
    /// These parameters include properties such as initial speed, color, opacity, scale, rotation, and mass.
    /// </remarks>
    public ParticleReleaseParameters Parameters { get; set; }

    /// <summary>
    /// Gets or sets the strategy used to execute modifiers on particles.
    /// </summary>
    /// <remarks>
    /// This determines whether modifiers are executed serially (single-threaded) or in parallel (multi-threaded),
    /// affecting performance characteristics based on the system's capabilities and the number of particles.
    /// </remarks>
    public ModifierExecutionStrategy ModifierExecutionStrategy { get; set; }

    /// <summary>
    /// Gets or sets the list of modifiers that affect particles emitted by this emitter.
    /// </summary>
    /// <remarks>
    /// Modifiers alter particle properties over time, creating effects such as gravity, color changes,
    /// rotation, and containment within boundaries.
    /// </remarks>
    public List<Modifier> Modifiers { get; set; }

    /// <summary>
    /// Gets or sets the profile that determines the initial position and heading of emitted particles.
    /// </summary>
    /// <remarks>
    /// Profiles define the emission pattern, such as points, lines, rings, or areas from which particles originate.
    /// </remarks>
    public Profile Profile { get; set; }

    /// <summary>
    /// The <see cref="Texture2DRegion"/> to use when rendering particles from this emitter.
    /// </summary>
    public Texture2DRegion TextureRegion { get; set; }

    /// <summary>
    /// Gets or sets the order in which particles are rendered within this emitter.
    /// </summary>
    /// <remarks>
    /// This property determines whether particles are drawn front-to-back or back-to-front,
    /// affecting how they visually overlap when using alpha blending.
    /// </remarks>
    public ParticleRenderingOrder RenderingOrder { get; set; }

    /// <summary>
    /// Get or sets a value that indicates whether the particles emitted by this emitter are visible.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="ParticleEmitter"/> has been disposed.
    /// </summary>
    /// <value><see langword="true"/> if the emitter has been disposed; otherwise, <see langword="false"/>.</value>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleEmitter"/> class with default capacity.
    /// </summary>
    /// <remarks>
    /// Creates an emitter with a capacity of 1000 particles and default settings.
    /// </remarks>
    public ParticleEmitter() : this(1000) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleEmitter"/> class with the specified capacity.
    /// </summary>
    /// <param name="initialCapacity">The maximum number of particles this emitter can manage.</param>
    /// <remarks>
    /// This constructor initializes the emitter with default settings but allows for specifying
    /// the maximum number of particles it can handle.
    /// </remarks>
    public ParticleEmitter(int initialCapacity)
    {
        LifeSpan = 1.0f;
        Name = nameof(ParticleEmitter);
        TextureRegion = null;
        _buffer = new ParticleBuffer(initialCapacity);
        Profile = Profile.Point();
        Modifiers = new List<Modifier>();
        ModifierExecutionStrategy = ModifierExecutionStrategy.Serial;
        Parameters = new ParticleReleaseParameters();
        ReclaimFrequency = 60.0f;
        Offset = Vector2.Zero;
        LayerDepth = 0.0f;
        Visible = true;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ParticleEmitter"/> class.
    /// </summary>
    ~ParticleEmitter()
    {
        Dispose(false);
    }

    /// <summary>
    /// Changes the maximum capacity of this emitter.
    /// </summary>
    /// <param name="size">The new maximum number of particles this emitter can manage.</param>
    /// <remarks>
    /// This method disposes the old buffer and creates a new one with the specified capacity.
    /// Any existing particles are lost during this operation.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this method is called after the emitter has been disposed.
    /// </exception>
    public void ChangeCapacity(int size)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, typeof(ParticleBuffer));

        if (Capacity == size)
        {
            return;
        }

        if (Buffer is ParticleBuffer oldBuffer)
        {
            oldBuffer.Dispose();
        }

        _buffer = new ParticleBuffer(size);
    }

    /// <summary>
    /// Updates the state of all particles managed by this emitter.
    /// </summary>
    /// <param name="elapsedSeconds">The elapsed time, in seconds, since the last update.</param>
    /// <param name="position">The current position of the emitter in 2D space.</param>
    /// <remarks>
    /// This method handles automatic triggering of particle emissions, updates the positions of all active
    /// particles based on their velocities, applies all registered modifiers, and reclaims expired particles.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this method is called after the emitter has been disposed.
    /// </exception>
    public void Update(float elapsedSeconds, Vector2 position = default)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, typeof(ParticleBuffer));

        _totalSeconds += elapsedSeconds;
        _secondsSinceLastReclaim += elapsedSeconds;

        if (Buffer.Count == 0)
        {
            return;
        }

        if (_secondsSinceLastReclaim > (1.0f / ReclaimFrequency))
        {
            ReclaimExpiredParticles();
            _secondsSinceLastReclaim -= (1.0f / ReclaimFrequency);
        }

        if (Buffer.Count > 0)
        {
            ParticleIterator iterator = Buffer.Iterator;
            while (iterator.HasNext)
            {
                Particle* particle = iterator.Next();
                particle->Age = (_totalSeconds - particle->Inception) / LifeSpan;
                particle->Position[0] += particle->Velocity[0] * elapsedSeconds;
                particle->Position[1] += particle->Velocity[1] * elapsedSeconds;
            }

            ModifierExecutionStrategy.ExecuteModifiers(Modifiers, elapsedSeconds, iterator);
        }
    }

    /// <summary>
    /// Triggers the emission of particles at the specified position.
    /// </summary>
    /// <param name="position">The position in 2D space from which to emit particles.</param>
    /// <param name="layerDepth">The layer depth at which to render the emitted particles.</param>
    /// <remarks>
    /// This method creates a burst of particles according to the configured <see cref="Parameters"/>.
    /// The number of particles released is determined by the <see cref="ParticleReleaseParameters.Quantity"/> property.
    /// </remarks>
    public void Trigger(Vector2 position, float layerDepth = 0)
    {
        int numToRelease = Parameters.Quantity.Value;
        Release(position, numToRelease, layerDepth);
    }

    /// <summary>
    /// Triggers the emission of particles along a line segment.
    /// </summary>
    /// <param name="line">The line segment along which to distribute emitted particles.</param>
    /// <param name="layerDepth">The layer depth at which to render the emitted particles.</param>
    /// <remarks>
    /// This method creates particles at random positions along the specified line segment.
    /// The number of particles released is determined by the <see cref="ParticleReleaseParameters.Quantity"/> property.
    /// </remarks>
    public void Trigger(LineSegment line, float layerDepth = 0)
    {
        int numToRelease = Parameters.Quantity.Value;
        Vector2 lineVector = line.ToVector2();

        for (int i = 0; i < numToRelease; i++)
        {
            Vector2 offset = lineVector * FastRandom.Shared.NextSingle();
            Release(line.Origin + offset, 1, layerDepth);
        }
    }

    /// <summary>
    /// Releases a specified number of particles at the given position.
    /// </summary>
    /// <param name="position">The position in 2D space from which to emit particles.</param>
    /// <param name="numToRelease">The number of particles to release.</param>
    /// <param name="layerDepth">The layer depth at which to render the emitted particles.</param>
    /// <remarks>
    /// This method initializes newly created particles with properties based on the emitter's
    /// <see cref="Profile"/> and <see cref="Parameters"/>.
    /// </remarks>
    private void Release(Vector2 position, int numToRelease, float layerDepth)
    {
        ParticleIterator iterator = Buffer.Release(numToRelease);

        while (iterator.HasNext)
        {
            Particle* particle = iterator.Next();

            Profile.GetOffsetAndHeading((Vector2*)particle->Position, (Vector2*)particle->Velocity);

            particle->Age = 0.0f;
            particle->Inception = _totalSeconds;

            particle->Position[0] += position.X;
            particle->Position[1] += position.Y;

            particle->TriggeredPos[0] = position.X;
            particle->TriggeredPos[1] = position.Y;

            float speed = Parameters.Speed.Value;

            particle->Velocity[0] *= speed;
            particle->Velocity[1] *= speed;

            Vector3 color = Parameters.Color.Value;
            particle->Color[0] = color.X;
            particle->Color[1] = color.Y;
            particle->Color[2] = color.Z;

            particle->Opacity = Parameters.Opacity.Value;

            Vector2 scale = Parameters.Scale.Value;
            particle->Scale[0] = scale.X;
            particle->Scale[1] = scale.Y;

            particle->Rotation = Parameters.Rotation.Value;
            particle->Mass = Parameters.Mass.Value;
            particle->LayerDepth = layerDepth;
        }
    }

    /// <summary>
    /// Reclaims particles that have exceeded their lifespan.
    /// </summary>
    /// <remarks>
    /// This method removes expired particles from the beginning of the buffer and compacts
    /// the remaining particles to maintain efficient memory usage.
    /// </remarks>
    private void ReclaimExpiredParticles()
    {
        int expired = 0;
        ParticleIterator iterator = Buffer.Iterator;
        while (iterator.HasNext)
        {
            Particle* particle = iterator.Next();

            if ((_totalSeconds - particle->Inception) < LifeSpan)
            {
                break;
            }
            expired++;
        }

        if (expired != 0)
        {
            Buffer.Reclaim(expired);
        }
    }

    /// <summary>
    /// Returns a string that represents the current emitter.
    /// </summary>
    /// <returns>The <see cref="Name"/> of this emitter.</returns>
    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="ParticleEmitter"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="ParticleEmitter"/>.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
    /// <see langword="false"/> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (IsDisposed) { return; }

        if (disposing)
        {
            //  No managed objects
        }

        Buffer.Dispose();
        IsDisposed = true;
    }
}
