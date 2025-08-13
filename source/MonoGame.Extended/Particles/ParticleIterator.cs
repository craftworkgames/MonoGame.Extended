// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles;

/// <summary>
/// Provides functionality for iterating through particles in a circular buffer.
/// </summary>
/// <remarks>
/// The <see cref="ParticleIterator"/> class enables safe traversal of active particles in a
/// <see cref="ParticleBuffer"/>, automatically handling the circular nature of the buffer and wrapping around
/// boundaries as needed.
/// </remarks>
public sealed class ParticleIterator
{
    private readonly ParticleBuffer _buffer;
    private unsafe Particle* _current;

    /// <summary>
    /// Gets the total number of particles that can be iterated over.
    /// </summary>
    public int Total { get; private set; }

    /// <summary>
    /// Gets a value indicating whether there are more particles to iterate over.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if there are more particles available; otherwise, <see langword="false"/>.
    /// </value>
    public unsafe bool HasNext => _current != _buffer.Tail;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleIterator"/> class.
    /// </summary>
    /// <param name="buffer">The <see cref="ParticleBuffer"/> to iterate over.</param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException"><paramref name="buffer"/> has previously been disposed.</exception>
    public ParticleIterator(ParticleBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ObjectDisposedException.ThrowIf(buffer.IsDisposed, buffer);
        _buffer = buffer;
    }

    /// <summary>
    /// Resets the iterator to the beginning of the active particles in the buffer.
    /// </summary>
    /// <returns>This <see cref="ParticleIterator"/> instance.</returns>
    public unsafe ParticleIterator Reset()
    {
        _current = _buffer.Head;
        Total = _buffer.Count;
        return this;
    }

    /// <summary>
    /// Resets the iterator to a specific offset position within the active particles.
    /// </summary>
    /// <param name="offset">The number of particles to offset from the head position.</param>
    /// <returns>This <see cref="ParticleIterator"/> instance.</returns>
    internal unsafe ParticleIterator Reset(int offset)
    {
        Total = _buffer.Count;

        _current = _buffer.Head + offset;

        if (_current >= _buffer.BufferEnd)
        {
            _current -= _buffer.Size + 1;
        }

        return this;
    }

    /// <summary>
    /// Advances the iterator to the next particle and returns a pointer to the current particle.
    /// </summary>
    /// <returns>A pointer to the current particle before advancing the iterator.</returns>
    public unsafe Particle* Next()
    {
        Particle* particle = _current;

        _current++;

        if (_current == _buffer.BufferEnd)
        {
            _current = (Particle*)_buffer.NativePointer;
        }

        return particle;

    }




}
