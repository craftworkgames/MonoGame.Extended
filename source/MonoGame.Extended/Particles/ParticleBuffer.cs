// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using MonoGame.Extended.Particles.Data;

namespace MonoGame.Extended.Particles;

/// <summary>
/// Represents a memory buffer that efficiently stores and manages particles in contiguous unmanaged memory.
/// </summary>
/// <remarks>
/// The <see cref="ParticleBuffer"/> class provides high-performance memory management for particle systems by
/// allocating an unmanaged memory block to store particle data. It handles allocation, deallocation, and compaction of
/// the buffer as particles are added and removed.
/// </remarks>
public sealed class ParticleBuffer : IDisposable
{
    private int _tail;

    /// <summary>
    /// Gets the native pointer to the unmanaged memory block where particles are stored.
    /// </summary>
    /// <remarks>
    /// This pointer should be used carefully and only within unsafe code blocks. The memory it points to is
    /// automatically freed when the <see cref="ParticleBuffer"/> is disposed.
    /// </remarks>
    public readonly IntPtr NativePointer;

    /// <summary>
    /// Gets the maximum number of particles that can be stored in this buffer.
    /// </summary>
    public readonly int Size;

    /// <summary>
    /// Gets the number of additional particles that can be added to the buffer before it is full.
    /// </summary>
    public int Available
    {
        get
        {
            return Size - _tail;
        }
    }

    /// <summary>
    /// Gets the current number of active particles in the buffer.
    /// </summary>
    public int Count
    {
        get
        {
            return _tail;
        }
    }

    /// <summary>
    /// Gets the total size of the buffer in bytes.
    /// </summary>
    public int SizeInBytes
    {
        get
        {
            return Particle.SizeInBytes * Size;
        }
    }

    /// <summary>
    /// Gets the size of the currently active portion of the buffer in bytes.
    /// </summary>
    public int ActiveSizeInBytes
    {
        get
        {
            return Particle.SizeInBytes * _tail;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="ParticleBuffer"/> has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleBuffer"/> class with the specified capacity.
    /// </summary>
    /// <param name="size">The maximum number of particles that can be stored in the buffer.</param>
    public ParticleBuffer(int size)
    {
        Size = size;
        NativePointer = Marshal.AllocCoTaskMem(SizeInBytes);
        GC.AddMemoryPressure(SizeInBytes);
    }

    /// <summary>Finalizer</summary>
    ~ParticleBuffer()
    {
        Dispose(false);
    }

    /// <summary>
    /// Allocates space in the buffer for a specified number of particles to be released.
    /// </summary>
    /// <remarks>
    /// This method supports the particle emission process by reserving memory for new particles.
    /// It's called by <see cref="ParticleEmitter.Release"/> when particles are being emitted.
    /// </remarks>
    /// <param name="releaseQuantity">The number of particles to allocate space for.</param>
    /// <param name="first">When this method returns, contains a pointer to the first allocated particle.</param>
    /// <returns>
    /// The actual number of particles that were allocated, which may be less than requested if the buffer is nearly
    /// full.
    /// </returns>
    public unsafe int Release(int releaseQuantity, out Particle* first)
    {
        int numToRelease = Math.Min(releaseQuantity, Available);
        int oldTail = _tail;
        _tail += numToRelease;
        first = (Particle*)IntPtr.Add(NativePointer, oldTail * Particle.SizeInBytes);
        return numToRelease;
    }

    /// <summary>
    /// Removes a specified number of particles from the beginning of the buffer and compacts the remaining data.
    /// </summary>
    /// <param name="number">The number of particles to remove from the buffer.</param>
    public unsafe void Reclaim(int number)
    {
        _tail -= number;
        MemCpy(NativePointer, IntPtr.Add(NativePointer, number * Particle.SizeInBytes), ActiveSizeInBytes);
    }

    /// <summary>
    /// Copies the active particle data to a specified destination in memory.
    /// </summary>
    /// <param name="destination">The destination memory address to copy the particle data to.</param>
    public unsafe void CopyTo(IntPtr destination)
    {
        MemCpy(destination, NativePointer, ActiveSizeInBytes);
    }

    /// <summary>
    /// Copies the active particle data to a specified destination in reverse order.
    /// </summary>
    /// <param name="destination">The destination memory address to copy the particle data to.</param>
    public unsafe void CopyToReverse(IntPtr destination)
    {
        int offset = 0;

        for (var i = ActiveSizeInBytes - Particle.SizeInBytes; i >= 0; i -= Particle.SizeInBytes)
        {
            MemCpy(IntPtr.Add(destination, offset), IntPtr.Add(NativePointer, i), Particle.SizeInBytes);
            offset += Particle.SizeInBytes;
        }
    }

    private static unsafe void MemCpy(IntPtr dest, IntPtr src, int count)
    {
        Buffer.MemoryCopy(
           source: (void*)src,
           destination: (void*)dest,
           destinationSizeInBytes: count,
           sourceBytesToCopy: count);
    }

    /// <summary>
    /// Releases all resources used by the <see cref="ParticleBuffer"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (IsDisposed) { return; }

        if (disposing)
        {
            //  No managed resources to free.
        }

        Marshal.FreeHGlobal(NativePointer);
        GC.RemoveMemoryPressure(Particle.SizeInBytes * Size);

        IsDisposed = true;
    }
}
