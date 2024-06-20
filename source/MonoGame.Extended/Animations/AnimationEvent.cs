// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;

namespace MonoGame.Extended.Animations;

/// <summary>
/// Represents an event that occurs during an animation.
/// </summary>
public class AnimationEvent : EventArgs
{
    /// <summary>
    /// Gets the animation associated with the event.
    /// </summary>
    public IAnimation Animation { get; }

    /// <summary>
    /// Gets the trigger that caused the event.
    /// </summary>
    public AnimationEventTrigger Trigger { get; }

    internal AnimationEvent(IAnimation animation, AnimationEventTrigger trigger) => (Animation, Trigger) = (animation, trigger);
}
