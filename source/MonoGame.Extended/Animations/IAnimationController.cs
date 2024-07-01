// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations;

/// <summary>
/// Defines the interface for an animation controller with features to play, pause, stop, reset, and set the state of
/// animation playback such as looping, reversing, and ping-pong effects.
/// </summary>
public interface IAnimationController : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether this animation controller has been disposed of.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Gets a value indicating whether the animation is paused.
    /// </summary>
    bool IsPaused { get; }

    /// <summary>
    /// Gets a value indicating whether the animation is currently animating.
    /// </summary>
    bool IsAnimating { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the animation should loop.
    /// </summary>
    bool IsLooping { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the animation is reversed.
    /// </summary>
    bool IsReversed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the animation should ping-pong (reverse direction at the ends).
    /// </summary>
    bool IsPingPong { get; set; }

    /// <summary>
    /// Gets or sets the speed of the animation.
    /// </summary>
    /// <value>The speed cannot be less than zero.</value>
    double Speed { get; set; }

    /// <summary>
    /// Gets or sets the action to perform when an animation event is triggered.
    /// </summary>
    event Action<IAnimationController, AnimationEventTrigger> OnAnimationEvent;

    /// <summary>
    /// Gets the time remaining for the current frame.
    /// </summary>
    TimeSpan CurrentFrameTimeRemaining { get; }

    /// <summary>
    /// Gets the index of the current frame of the animation.
    /// </summary>
    int CurrentFrame { get; }

    /// <summary>
    /// Gets the total number of frames in the animation.
    /// </summary>
    int FrameCount { get; }

    /// <summary>
    /// Sets the animation to a specified frame.
    /// </summary>
    /// <param name="index">The index of the frame to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the <paramref name="index"/> parameter is less than zero or greater than or equal to the total
    /// number of frames.
    /// </exception>
    void SetFrame(int index);

    /// <summary>
    /// Plays the animation from the beginning.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the animation was successfully started; otherwise, <see langword="false"/>.
    /// </returns>
    bool Play();

    /// <summary>
    /// Plays the animation from a specified starting frame.
    /// </summary>
    /// <param name="startingFrame">The frame to start the animation from.</param>
    /// <returns>
    /// <see langword="true"/> if the animation was successfully started; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the <paramref name="startingFrame"/> parameter is less than zero or greater than or equal to the
    /// total number of frames.
    /// </exception>
    bool Play(int startingFrame);

    /// <summary>
    /// Pauses the animation.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the animation was successfully paused; otherwise, <see langword="false"/>.
    /// </returns>
    bool Pause();

    /// <summary>
    /// Pauses the animation.
    /// </summary>
    /// <param name="resetFrameDuration">If set to <see langword="true"/>, resets the frame duration.</param>
    /// <returns>
    /// <see langword="true"/> if the animation was successfully paused; otherwise, <see langword="false"/>.
    /// </returns>
    bool Pause(bool resetFrameDuration);

    /// <summary>
    /// Unpauses the animation.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the animation was successfully unpaused; otherwise, <see langword="false"/>.
    /// </returns>
    bool Unpause();

    /// <summary>
    /// Unpauses the animation.
    /// </summary>
    /// <param name="advanceToNextFrame">If set to <see langword="true"/>, advances to the next frame.</param>
    /// <returns>
    /// <see langword="true"/> if the animation was successfully unpaused; otherwise, <see langword="false"/>.
    /// </returns>
    bool Unpause(bool advanceToNextFrame);

    /// <summary>
    /// Updates the animation.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    void Update(GameTime gameTime);

    /// <summary>
    /// Stops the animation.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the animation was successfully stopped; otherwise, <see langword="false"/>.
    /// </returns>
    bool Stop();

    /// <summary>
    /// Resets the animation to its initial state.
    /// </summary>
    void Reset();
}
