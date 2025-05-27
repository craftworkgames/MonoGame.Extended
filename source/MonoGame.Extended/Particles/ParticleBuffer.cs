// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles;

/// <summary>
/// Represents a circular memory buffer that efficiently stores and manages particles in contiguous unmanaged memory.
/// </summary>
/// <remarks>
/// The <see cref="ParticleBuffer"/> class provides high-performance memory management for particle systems by
/// allocating an unmanaged memory block to store particle data in a circular buffer arrangement. This implementation
/// uses head and tail pointers to manage particle allocation and deallocation without requiring memory copying
/// operations, making it more efficient than a linear buffer approach.
/// </remarks>
public unsafe class ParticleBuffer : IDisposable
{
    private readonly ParticleIterator _iterator;
    private readonly IntPtr _nativePointer;

    /// <summary>
    /// A pointer to the end of the allocated buffer memory, used for circular buffer bounds checking.
    /// </summary>
    protected readonly Particle* _bufferEnd;

    /// <summary>
    /// A pointer to the current tail position in the circular buffer where new particles are allocated.
    /// </summary>
    protected unsafe Particle* _tail;

    /// <summary>
    /// Gets the maximum number of particles that can be stored in this buffer.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Gets an iterator for traversing the active particles in the buffer.
    /// </summary>
    /// <remarks>
    /// The iterator is reset each time this property is accessed, starting from the current head position.
    /// Use this to safely iterate through all active particles in the correct order.
    /// </remarks>
    public ParticleIterator Iterator => _iterator.Reset();

    /// <summary>
    /// Gets a pointer to the current head position in the circular buffer where the oldest active particle is located.
    /// </summary>
    /// <remarks>
    /// This pointer should be used carefully and only within unsafe code blocks. The memory it points to is
    /// automatically freed when the <see cref="ParticleBuffer"/> is disposed.
    /// </remarks>
    public unsafe Particle* Head { get; private set; }

    /// <summary>
    /// Gets the number of additional particles that can be added to the buffer before it is full.
    /// </summary>
    public int Available => Size - Count;

    /// <summary>
    /// Gets the current number of active particles in the buffer.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets the total size of the buffer in bytes.
    /// </summary>
    /// <remarks>
    /// The size includes space for one additional particle beyond the specified capacity to facilitate
    /// circular buffer operations.
    /// </remarks>
    public int SizeInBytes => Particle.SizeInBytes * (Size + 1);

    /// <summary>
    /// Gets the size of the currently active portion of the buffer in bytes.
    /// </summary>
    public int ActiveSizeInBytes => Particle.SizeInBytes * Count;

    /// <summary>
    /// Gets a value indicating whether this <see cref="ParticleBuffer"/> has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleBuffer"/> class with the specified capacity.
    /// </summary>
    /// <param name="size">The maximum number of particles that can be stored in the buffer.</param>
    public unsafe ParticleBuffer(int size)
    {
        Size = size;
        _nativePointer = Marshal.AllocHGlobal(SizeInBytes);

        _bufferEnd = (Particle*)(_nativePointer + SizeInBytes);
        Head = (Particle*)_nativePointer;
        _tail = (Particle*)_nativePointer;

        _iterator = new ParticleIterator(this);

        GC.AddMemoryPressure(SizeInBytes);
    }

    /// <summary/>
    ~ParticleBuffer() => Dispose();

    /// <summary>
    /// Allocates space in the circular buffer for a specified number of particles to be released.
    /// </summary>
    /// <param name="releaseQuantity">The number of particles to allocate space for.</param>
    /// <returns>
    /// A <see cref="ParticleIterator"/> positioned at the start of the newly allocated particles,
    /// allowing iteration over the allocated particle slots.
    /// </returns>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this method is called after the buffer has been disposed.
    /// </exception>
    public unsafe ParticleIterator Release(int releaseQuantity)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        int numToRelease = Math.Min(releaseQuantity, Available);

        int prevCount = Count;
        Count += numToRelease;

        _tail += numToRelease;

        if (_tail >= _bufferEnd)
        {
            _tail -= Size + 1;
        }

        return Iterator.Reset(prevCount);
    }

    /// <summary>
    /// Removes a specified number of particles from the beginning of the circular buffer.
    /// </summary>
    /// <param name="number">The number of particles to remove from the buffer.</param>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this method is called after the buffer has been disposed.
    /// </exception>
    public unsafe void Reclaim(int number)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        Count -= number;

        Head += number;

        if (Head >= _bufferEnd)
        {
            Head -= Size + 1;
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="ParticleBuffer"/>.
    /// </summary>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        Marshal.FreeHGlobal(_nativePointer);
        GC.RemoveMemoryPressure(SizeInBytes);
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Provides functionality for iterating through particles in the circular buffer.
    /// </summary>
    /// <remarks>
    /// The <see cref="ParticleIterator"/> class enables safe traversal of active particles in the buffer,
    /// automatically handling the circular nature of the buffer and wrapping around boundaries as needed.
    /// </remarks>
    public class ParticleIterator
    {
        private readonly ParticleBuffer _buffer;
        private unsafe Particle* _current;

        /// <summary>
        /// Gets the total number of particles that can be iterated over.
        /// </summary>
        public int Total;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleIterator"/> class.
        /// </summary>
        /// <param name="buffer">The particle buffer to iterate over.</param>
        public ParticleIterator(ParticleBuffer buffer)
        {
            _buffer = buffer;
        }

        /// <summary>
        /// Gets a value indicating whether there are more particles to iterate over.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if there are more particles available; otherwise, <see langword="false"/>.
        /// </value>
        public unsafe bool HasNext => _current != _buffer._tail;

        /// <summary>
        /// Resets the iterator to the beginning of the active particles in the buffer.
        /// </summary>
        /// <returns>This <see cref="ParticleIterator"/> instance for method chaining.</returns>
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
        /// <returns>This <see cref="ParticleIterator"/> instance for method chaining.</returns>
        internal unsafe ParticleIterator Reset(int offset)
        {
            Total = _buffer.Count;

            _current = _buffer.Head + offset;

            if (_current >= _buffer._bufferEnd)
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
            Particle* p = _current;
            _current++;
            if (_current == _buffer._bufferEnd)
            {
                _current = (Particle*)_buffer._nativePointer;
            }

            return p;
        }
    }
}
