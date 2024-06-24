// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace MonoGame.Extended.Animations;

/// <summary>
/// Specifies the trigger for an animation event.
/// </summary>
public enum AnimationEventTrigger
{
    /// <summary>
    /// Triggered at the beginning of a frame.
    /// </summary>
    FrameBegin,

    /// <summary>
    /// Triggered at the end of a frame.
    /// </summary>
    FrameEnd,

    /// <summary>
    /// Triggered when a frame is skipped.
    /// </summary>
    FrameSkipped,

    /// <summary>
    /// Triggered when the animation loops.
    /// </summary>
    AnimationLoop,

    /// <summary>
    /// Triggered when the animation completes.
    /// </summary>
    AnimationCompleted,

    /// <summary>
    /// Triggered when the animation stops.
    /// </summary>
    AnimationStopped,
}

