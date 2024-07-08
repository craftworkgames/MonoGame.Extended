// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;

namespace MonoGame.Extended.Animations;

/// <summary>
/// Defines the interface for an animation, specifying properties of the animation such as frames, looping, reversing,
/// and ping-pong effects.
/// </summary>
public interface IAnimation
{
    /// <summary>
    /// Gets the name of the animation.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the read-only collection of frames in the animation.
    /// </summary>
    ReadOnlySpan<IAnimationFrame> Frames { get; }

    /// <summary>
    /// Gets the total number of frames in the animation.
    /// </summary>
    int FrameCount { get; }

    /// <summary>
    /// Gets a value indicating whether the animation should loop.
    /// </summary>
    bool IsLooping { get; }

    /// <summary>
    /// Gets a value indicating whether the animation is reversed.
    /// </summary>
    bool IsReversed { get; }

    /// <summary>
    /// Gets a value indicating whether the animation should ping-pong (reverse direction at the ends).
    /// </summary>
    bool IsPingPong { get; }
}
