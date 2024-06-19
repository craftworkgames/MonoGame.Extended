// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;
using static System.Net.WebRequestMethods;

namespace MonoGame.Extended.Animations;

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
    public Action<IAnimation> OnFrameBegin { get; set; } = default;

    /// <inheritdoc />
    public Action<IAnimation> OnFrameEnd { get; set; } = default;

    /// <inheritdoc />
    public Action<IAnimation> OnAnimationLoop { get; set; } = default;

    /// <inheritdoc />
    public Action<IAnimation> OnAnimationCompleted { get; set; } = default;

    /// <inheritdoc />
    public TimeSpan CurrentFrameTimeRemaining { get; private set; }

    /// <inheritdoc />
    public int CurrentFrame { get; private set; }

    public Animation(IAnimationDefinition definition)
    {
        _definition = definition;

        //  Set initial properties but keep original values in the definition cached for Reset()
        IsLooping = definition.IsLooping;
        IsReversed = definition.IsReversed;
        IsPingPong = definition.IsPingPong;
    }

    /// <inheritdoc />
    public  bool Pause() => Pause(false);

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

        if(resetFrameDuration)
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
        if(startingFrame < 0 || startingFrame >= _definition.FrameCount)
        {
            throw new ArgumentOutOfRangeException(nameof(startingFrame), $"{nameof(startingFrame)} cannot be less than zero or greater than or equal to the total number of frames in this {nameof(Animation)}");
        }

        //  Cannot play something that is already playing
        if(IsAnimating)
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
        if(index < 0 || index >= _definition.FrameCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} cannot be less than zero or greater than or equal to the total number of frames in this {nameof(Animation)}");
        }

        CurrentFrame = index;
        CurrentFrameTimeRemaining = _definition.Frames[CurrentFrame].Duration;
    }

    /// <inheritdoc />
    public bool Stop()
    {
        //  We can't stop something that's not animating.  This is to prevent accidentally invoking OnAnimationEnd
        if(!IsAnimating)
        {
            return false;
        }

        IsAnimating = false;
        IsPaused = true;
        OnAnimationCompleted?.Invoke(this);
        return true;
    }

    /// <inheritdoc />
    public bool UnPause() => UnPause(false);

    /// <inheritdoc />
    public bool UnPause(bool advanceToNextFrame)
    {
        //  We can't unpause something that's not animating and also isn't paused. This is to prevent improper usage
        //  that could accidentally advance to the next frame if it was set to true.
        if(!IsAnimating || !IsPaused)
        {
            return false;
        }

        IsPaused = false;

        if(advanceToNextFrame)
        {
            AdvanceFrame();
        }

        return true;
    }

    /// <inheritdoc />
    public void Update(GameTime gameTime)
    {
        TimeSpan elapsedTime = gameTime.ElapsedGameTime;

        if(!IsAnimating || IsPaused)
        {
            return;
        }

        if(CurrentFrameTimeRemaining == _definition.Frames[CurrentFrame].Duration)
        {
            OnFrameBegin?.Invoke(this);
        }

        CurrentFrameTimeRemaining -= elapsedTime * Speed;

        if(CurrentFrameTimeRemaining <= TimeSpan.Zero)
        {
            AdvanceFrame();
        }
    }

    private void AdvanceFrame()
    {
        OnFrameEnd?.Invoke(this);

        CurrentFrame += _direction;

        if(CurrentFrame >= _definition.FrameCount || CurrentFrame < 0)
        {
            if(IsLooping)
            {
                if(IsPingPong)
                {
                    _direction = -_direction;

                    //  Adjust the current index again after ping ponging so we don't repeat the same frame twice
                    //  in a row
                    CurrentFrame += _direction * 2;
                }
                else
                {
                    CurrentFrame = IsReversed ? _definition.FrameCount - 1 : 0;
                }

                OnAnimationLoop?.Invoke(this);
            }
            else
            {
                CurrentFrame -= _direction;
                Stop();
            }
        }

        CurrentFrameTimeRemaining = _definition.Frames[CurrentFrame].Duration;
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
        if(IsDisposed)
        {
            return;
        }

        IsDisposed = true;
    }
}
