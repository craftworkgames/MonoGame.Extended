// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations;

/// <summary>
/// Represents an animation with various control features such as play, pause, stop, looping, reversing, and
/// ping-pong effects.
/// </summary>
public class Animation : IAnimation
{
    private readonly IAnimationDefinition _definition;
    private int _direction;

    /// <summary>
    /// Gets a value that indicates whether this animation has been disposed of.
    /// </summary>
    public bool IsDisposed { get; protected set; }

    /// <inheritdoc />
    public bool IsPaused { get; private set; }

    /// <inheritdoc />
    public bool IsAnimating { get; private set; }

    /// <inheritdoc />
    public bool IsLooping { get; set; }

    /// <inheritdoc />
    public bool IsReversed
    {
        get => _direction == -1;
        set => _direction = value ? -1 : 1;
    }

    /// <inheritdoc />
    public bool IsPingPong { get; set; }

    /// <inheritdoc />
    public double Speed { get; set; }

    /// <inheritdoc />
    public event Action<IAnimation, AnimationEventTrigger> OnAnimationEvent;

    /// <inheritdoc />
    public TimeSpan CurrentFrameTimeRemaining { get; private set; }

    /// <inheritdoc />
    public int CurrentFrame { get; private set; }

    /// <inheritdoc />
    public int FrameCount => _definition.FrameCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="Animation"/> class with the specified definition.
    /// </summary>
    /// <param name="definition">The definition of the animation.</param>
    public Animation(IAnimationDefinition definition)
    {
        _definition = definition;

        //  Set initial properties but keep original values in the definition cached for Reset()
        IsLooping = definition.IsLooping;
        IsReversed = definition.IsReversed;
        IsPingPong = definition.IsPingPong;
        Speed = 1.0f;
    }

    /// <inheritdoc />
    public bool Pause() => Pause(false);

    /// <inheritdoc />
    public bool Pause(bool resetFrameDuration)
    {
        //  We can only pause something that is animating and not already paused.  This is to prevent situations
        //  that could accidentally reset frame duration if it was set to true.
        if (!IsAnimating || IsPaused)
        {
            return false;
        }

        IsPaused = true;

        if (resetFrameDuration)
        {
            CurrentFrameTimeRemaining = _definition.Frames[CurrentFrame].Duration;
        }

        return true;
    }

    /// <inheritdoc />
    public bool Play() => Play(0);

    /// <inheritdoc />
    public bool Play(int startingFrame)
    {
        if (startingFrame < 0 || startingFrame >= _definition.FrameCount)
        {
            throw new ArgumentOutOfRangeException(nameof(startingFrame), $"{nameof(startingFrame)} cannot be less than zero or greater than or equal to the total number of frames in this {nameof(Animation)}");
        }

        //  Cannot play something that is already playing
        if (IsAnimating)
        {
            return false;
        }

        IsAnimating = true;
        CurrentFrame = startingFrame;
        CurrentFrameTimeRemaining = _definition.Frames[CurrentFrame].Duration;
        return true;
    }

    /// <inheritdoc />
    public void Reset()
    {
        IsReversed = _definition.IsReversed;
        IsPingPong = _definition.IsPingPong;
        IsLooping = _definition.IsLooping;
        IsAnimating = false;
        IsPaused = true;
        Speed = 1.0d;
        CurrentFrame = IsReversed ? _definition.FrameCount - 1 : 0;
        CurrentFrameTimeRemaining = _definition.Frames[CurrentFrame].Duration;
    }

    /// <inheritdoc />
    public void SetFrame(int index)
    {
        if (index < 0 || index >= _definition.FrameCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} cannot be less than zero or greater than or equal to the total number of frames in this {nameof(Animation)}");
        }

        CurrentFrame = index;
        CurrentFrameTimeRemaining = _definition.Frames[CurrentFrame].Duration;
        OnAnimationEvent?.Invoke(this, AnimationEventTrigger.FrameBegin);
    }

    /// <inheritdoc />
    public bool Stop() => Stop(AnimationEventTrigger.AnimationStopped);

    private bool Stop(AnimationEventTrigger trigger)
    {
        //  We can't stop something that's not animating.  This is to prevent accidentally invoking OnAnimationEnd
        if (!IsAnimating)
        {
            return false;
        }

        IsAnimating = false;
        IsPaused = true;
        OnAnimationEvent?.Invoke(this, trigger);
        return true;
    }

    /// <inheritdoc />
    public bool Unpause() => Unpause(false);

    /// <inheritdoc />
    public bool Unpause(bool advanceToNextFrame)
    {
        //  We can't unpause something that's not animating and also isn't paused. This is to prevent improper usage
        //  that could accidentally advance to the next frame if it was set to true.
        if (!IsAnimating || !IsPaused)
        {
            return false;
        }

        IsPaused = false;

        if (advanceToNextFrame)
        {
            _ = AdvanceFrame();
        }

        return true;
    }

    /// <inheritdoc />
    public void Update(GameTime gameTime)
    {
        TimeSpan elapsedTime = gameTime.ElapsedGameTime;
        TimeSpan remainingTime = TimeSpan.Zero;

        if (!IsAnimating || IsPaused)
        {
            return;
        }

        CurrentFrameTimeRemaining -= elapsedTime * Speed;

        while (CurrentFrameTimeRemaining <= TimeSpan.Zero)
        {
            remainingTime += -CurrentFrameTimeRemaining;

            //  End the current frame
            OnAnimationEvent?.Invoke(this, AnimationEventTrigger.FrameEnd);

            if (!AdvanceFrame())
            {
                break;
            }

            CurrentFrameTimeRemaining -= remainingTime;
            remainingTime = TimeSpan.Zero;
        }
    }

    private bool AdvanceFrame()
    {
        //  Increment the current frame
        CurrentFrame += _direction;

        //  Ensure frame is in bounds
        if (CurrentFrame < 0 || CurrentFrame >= _definition.FrameCount)
        {
            //  Is this a looping animation?
            if (IsLooping)
            {
                //  Is this a standard loop or is it a ping pong?
                if (IsPingPong)
                {
                    _direction = -_direction;
                    CurrentFrame += _direction * 2;
                }
                else
                {
                    CurrentFrame = IsReversed ? _definition.FrameCount - 1 : 0;
                }

                //   We looped
                OnAnimationEvent?.Invoke(this, AnimationEventTrigger.AnimationLoop);
            }
            else
            {
                //  No looping and we've reached the end, stop the animation
                CurrentFrame -= _direction;
                Stop(AnimationEventTrigger.AnimationCompleted);
                return false;
            }
        }

        CurrentFrameTimeRemaining = _definition.Frames[CurrentFrame].Duration;
        OnAnimationEvent?.Invoke(this, AnimationEventTrigger.FrameBegin);
        return true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose()"/>
    /// <remarks>
    ///     <para>
    ///         When overriding this method, check if <paramref name="disposing"/> is <see langword="true"/> or
    ///         <see langword="false"/>.  Only dispose of other managed resources when it is <see langword="true"/>.
    ///     </para>
    ///     <see href="https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/dispose-pattern#basic-dispose-pattern"/>
    /// </remarks>
    /// <param name="disposing">Indicates whether this was called from <see cref="Dispose()"/> or the finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
    }
}
