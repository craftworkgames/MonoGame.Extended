// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Particles.Primitives;

namespace MonoGame.Extended.Particles;

/// <summary>
/// Represents a complete particle effect composed of multiple emitters.
/// </summary>
/// <remarks>
/// The <see cref="ParticleEffect"/> class serves as a container for one or more <see cref="ParticleEmitter"/> instances,
/// allowing for complex visual effects that combine different types of particle behaviors and appearances.
///
/// Effects can be positioned, rotated, and scaled as a single unit, with all contained emitters being affected
/// by these transformations. Effects also provide methods for triggering all emitters simultaneously.
/// </remarks>
public class ParticleEffect : IDisposable
{
    private float _nextAutoTrigger;

    /// <summary>
    /// Gets or sets the name of this effect, used for identification and debugging.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the position of this effect in 2D space.
    /// </summary>
    /// <remarks>
    /// This position is used as the reference point for all emitters in the effect.
    /// When the effect is updated, this position is passed to each emitter's update method.
    /// </remarks>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the rotation of this effect, in radians.
    /// </summary>
    /// <remarks>
    /// This property can be used to rotate the entire effect around its position.
    /// Note that rotation is not automatically applied to emitters and must be handled by the rendering system.
    /// </remarks>
    public float Rotation { get; set; }

    /// <summary>
    /// Gets or sets the scale factor of this effect.
    /// </summary>
    /// <remarks>
    /// This property can be used to uniformly or non-uniformly scale the entire effect.
    /// Note that scaling is not automatically applied to emitters and must be handled by the rendering system.
    /// </remarks>
    public Vector2 Scale { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this particle effect should automatically trigger its particle emitters.
    /// </summary>
    /// <remarks>
    /// When <see langword="true"/>, all emitters of this <see cref="ParticleEffect"/> will be triggered  at the same
    /// based on the <see cref="AutoTriggerFrequency"/>.  When <see langword="false"/>, users will need to manually call
    /// the <see cref="Trigger()"/> method to trigger emitters.
    /// </remarks>
    public bool AutoTrigger { get; set; }

    /// <summary>
    /// Gets or sets the frequency, in seconds, at which this <see cref="ParticleEffect"/> automatically triggers emitters.
    /// </summary>
    /// <remarks>
    /// If <see cref="AutoTrigger"/> is <see langword="false"/>, this value is ignored.
    /// </remarks>
    public float AutoTriggerFrequency { get; set; }

    /// <summary>
    /// Gets or sets the collection of emitters that compose this effect.
    /// </summary>
    /// <remarks>
    /// Each emitter in this collection contributes to the overall visual appearance of the effect,
    /// potentially with different behaviors, textures, and particle properties.
    /// </remarks>
    public List<ParticleEmitter> Emitters { get; set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="ParticleEffect"/> has been disposed.
    /// </summary>
    /// <value><see langword="true"/> if the effect has been disposed; otherwise, <see langword="false"/>.</value>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets the total number of active particles across all emitters in this effect.
    /// </summary>
    /// <value>The sum of <see cref="ParticleEmitter.ActiveParticles"/> for all emitters in the effect.</value>
    public int ActiveParticles
    {
        get
        {
            return Emitters.Sum(t => t.ActiveParticles);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleEffect"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the effect, used for identification and debugging.</param>
    /// <remarks>
    /// This constructor initializes the effect with default position, rotation, and scale,
    /// and creates an empty collection of emitters.
    /// </remarks>
    public ParticleEffect(string name)
    {
        Name = name;
        Position = Vector2.Zero;
        Rotation = 0.0f;
        Scale = Vector2.One;
        Emitters = new List<ParticleEmitter>();
        AutoTrigger = true;
        AutoTriggerFrequency = 1.0f;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ParticleEffect"/> class.
    /// </summary>
    ~ParticleEffect()
    {
        Dispose(false);
    }

    /// <summary>
    /// Advances the effect's state rapidly to simulate it having been active for a period of time.
    /// </summary>
    /// <param name="position">The position at which to simulate the effect.</param>
    /// <param name="seconds">The total time, in seconds, to simulate.</param>
    /// <param name="triggerPeriod">The time interval, in seconds, between simulated triggers.</param>
    /// <remarks>
    /// This method is useful for pre-filling a scene with particles that appear to have been emitted
    /// over time, rather than starting with an empty effect.
    /// </remarks>
    public void FastForward(Vector2 position, float seconds, float triggerPeriod)
    {
        float time = 0.0f;
        while (time < seconds)
        {
            Update(triggerPeriod);
            Trigger(position);
            time += triggerPeriod;
        }
    }

    /// <summary>
    /// Updates the state of all emitters in this effect.
    /// </summary>
    /// <param name="gameTime">The timing values for the current update cycle.</param>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this method is called after the effect has been disposed.
    /// </exception>
    public void Update(GameTime gameTime)
    {
        Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    /// <summary>
    /// Updates the state of all emitters in this effect.
    /// </summary>
    /// <param name="elapsedSeconds">The elapsed time, in seconds, since the last update.</param>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this method is called after the effect has been disposed.
    /// </exception>
    public void Update(float elapsedSeconds)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, typeof(ParticleBuffer));

        if (AutoTrigger)
        {
            _nextAutoTrigger -= elapsedSeconds;

            if (_nextAutoTrigger <= 0)
            {
                Trigger();
                _nextAutoTrigger += AutoTriggerFrequency;
            }
        }

        for (int i = 0; i < Emitters.Count; i++)
        {
            Emitters[i].Update(elapsedSeconds, Position);
        }
    }

    /// <summary>
    /// Triggers all emitters in this effect at the effect's current position.
    /// </summary>
    public void Trigger()
    {
        Trigger(Position);
    }

    /// <summary>
    /// Triggers all emitters in this effect at the specified position.
    /// </summary>
    /// <param name="position">The position in 2D space at which to trigger the emitters.</param>
    /// <param name="layerDepth">The layer depth at which to render the emitted particles.</param>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this method is called after the effect has been disposed.
    /// </exception>
    public void Trigger(Vector2 position, float layerDepth = 0)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, typeof(ParticleBuffer));

        for (int i = 0; i < Emitters.Count; i++)
        {
            Emitters[i].Trigger(position, layerDepth);
        }
    }

    /// <summary>
    /// Triggers all emitters in this effect along a line segment.
    /// </summary>
    /// <param name="line">The line segment along which to distribute triggered particles.</param>
    /// <param name="layerDepth">The layer depth at which to render the emitted particles.</param>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this method is called after the effect has been disposed.
    /// </exception>
    public void Trigger(LineSegment line, float layerDepth)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, typeof(ParticleBuffer));

        for (int i = 0; i < Emitters.Count; i++)
        {
            Emitters[i].Trigger(line, layerDepth);
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ParticleEffect"/> class from a file.
    /// </summary>
    /// <param name="path">The path to the file containing the serialized effect data.</param>
    /// <param name="content">The content manager used to load graphical assets.</param>
    /// <returns>A new <see cref="ParticleEffect"/> instance with properties and emitters as defined in the file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is <see langword="null"/> or empty.</exception>
    public static ParticleEffect FromFile(string path, ContentManager content)
    {
        return ParticleEffectSerializer.Deserialize(path, content);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ParticleEffect"/> class from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="content">The <see cref="ContentManager"/> to use for loading textures.</param>
    /// <param name="baseDirectory">The base directory to use for resolving relative texture paths. If null, uses the ContentManager's RootDirectory.</param>
    /// <returns>A new <see cref="ParticleEffect"/> instance with properties and emitters as defined in the stream.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="content"/> is <see langword="null"/></exception>
    public static ParticleEffect FromStream(Stream stream, ContentManager content, string baseDirectory)
    {
        return ParticleEffectSerializer.Deserialize(stream, content, baseDirectory);
    }

    /// <summary>
    /// Releases all resources used by the <see cref="ParticleEffect"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources used by the <see cref="ParticleEffect"/>.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
    /// <see langword="false"/> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (IsDisposed) { return; }

        if (disposing)
        {
            for (int i = 0; i < Emitters.Count; i++)
            {
                Emitters[i].Dispose();
            }
        }

        IsDisposed = true;
    }

    /// <summary>
    /// Returns a string that represents the current effect.
    /// </summary>
    /// <returns>The <see cref="Name"/> of this effect.</returns>
    public override string ToString()
    {
        return Name;
    }
}
