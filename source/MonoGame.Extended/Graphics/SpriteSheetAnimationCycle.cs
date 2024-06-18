// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Represents an animation cycle using frames from a spritesheet.
/// </summary>
public class SpriteSheetAnimationCycle
{
    private SpriteSheetAnimationFrame[] _frames;

    /// <summary>
    /// Gets the name of this animation cycle.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a read-only span of <see cref="SpriteSheetAnimationFrame"/> instances that represent the frames of this
    /// animation cycle.
    /// </summary>
    public ReadOnlySpan<SpriteSheetAnimationFrame> Frames => _frames;

    /// <summary>
    /// Gets the <see cref="SpriteSheetAnimationFrame"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the frame to get.</param>
    /// <returns>The <see cref="SpriteSheetAnimationFrame"/> at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if the <paramref name="index"/> parameter is less than 0 or greater than or equal to
    /// <see cref="FrameCount"/>.
    /// </exception>
    public SpriteSheetAnimationFrame this[int index] => GetFrame(index);

    /// <summary>
    /// Gets the total number of frames in this animation cycle.
    /// </summary>
    public int FrameCount => _frames.Length;

    /// <summary>
    /// Gets or sets a value indicating whether this animation cycle should loop after reaching the last frame.
    /// </summary>
    public bool IsLooping { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the frames in this animation cycle should be played in reverse order.
    /// </summary>
    public bool IsReversed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this animation cycle should alternate between playing forwards and
    /// backwards.
    /// </summary>
    /// <remarks>
    /// If <see cref="IsPingPong"/> is set to <see langword="true"/>, the animation will play forwards until the last
    /// frame, then play backwards until the first frame, and repeat this pattern.
    /// </remarks>
    public bool IsPingPong { get; set; }

    internal SpriteSheetAnimationCycle(string name, SpriteSheetAnimationFrame[] frames, bool isLooping, bool isReversed, bool isPingPong)
    {
        Name = name;
        IsLooping = isLooping;
        IsReversed = isReversed;
        IsPingPong = isPingPong;
        _frames = frames;
    }

    /// <summary>
    /// Gets the <see cref="SpriteSheetAnimationFrame"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the frame to get.</param>
    /// <returns>The <see cref="SpriteSheetAnimationFrame"/> at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if the <paramref name="index"/> parameter is less than 0 or greater than or equal to
    /// <see cref="FrameCount"/>.
    /// </exception>
    public SpriteSheetAnimationFrame GetFrame(int index) => _frames[index];

}
