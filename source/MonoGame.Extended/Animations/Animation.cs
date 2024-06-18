// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations;

public class Animation : IAnimation
{
    private readonly IAnimationDefinition _definition;
    private readonly IAnimationFrame[] _frames;
    private int _currentIndex;
    private int _direction;

    public bool IsPaused { get; private set; }

    public bool IsAnimating { get; private set; }

    public bool IsLooping { get; set; }
    public bool IsReversed
    {
        get => _direction == -1;
        set => _direction = value ? -1 : 1;
    }

    public bool IsPingPong { get; set; }
    public double Speed { get; set; }
    public Action<Animation> OnFrameBegin { get; set; } = default;
    public Action<Animation> OnFrameEnd { get; set; } = default;
    public Action<Animation> OnAnimationLoop { get; set; } = default;
    public Action<Animation> OnAnimationCompleted { get; set; } = default;

    public TimeSpan CurrentFrameTimeRemaining { get; private set; }

    public IAnimationFrame CurrentFrame => _frames[_currentIndex];

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
            CurrentFrameTimeRemaining = CurrentFrame.Duration;
        }

        return true;
    }

    /// <inheritdoc />
    public bool Play() => Play(0);

    /// <inheritdoc />
    public bool Play(int startingFrame)
    {
        if(startingFrame < 0 || startingFrame >= _frames.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(startingFrame), $"{nameof(startingFrame)} cannot be less than zero or greater than or equal to the total number of frames in this {nameof(Animation)}");
        }

        //  Cannot play something that is already playing
        if(IsAnimating)
        {
            return false;
        }

        IsAnimating = true;
        _currentIndex = startingFrame;
        CurrentFrameTimeRemaining = CurrentFrame.Duration;
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
        _currentIndex = IsReversed ? _frames.Length - 1 : 0;
        CurrentFrameTimeRemaining = CurrentFrame.Duration;
    }

    /// <inheritdoc />
    public void SetFrame(int index)
    {
        if(index < 0 || index >= _frames.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} cannot be less than zero or greater than or equal to the total number of frames in this {nameof(Animation)}");
        }

        _currentIndex = index;
        CurrentFrameTimeRemaining = CurrentFrame.Duration;
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
    public void Update(double deltaTimeInSeonds) => Update(TimeSpan.FromSeconds(deltaTimeInSeonds));

    /// <inheritdoc />
    public void Update(GameTime gameTime) => Update(gameTime.ElapsedGameTime);

    /// <inheritdoc />
    public void Update(in TimeSpan elapsedTime)
    {
        if(!IsAnimating || IsPaused)
        {
            return;
        }

        if(CurrentFrameTimeRemaining == CurrentFrame.Duration)
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

        _currentIndex += _direction;

        if(_currentIndex >= _frames.Length || _currentIndex < 0)
        {
            if(IsLooping)
            {
                if(IsPingPong)
                {
                    _direction = -_direction;

                    //  Adjust the current index again after ping ponging so we don't repeat the same frame twice
                    //  in a row
                    _currentIndex += _direction * 2;
                }
                else
                {
                    _currentIndex = IsReversed ? _frames.Length - 1 : 0;
                }

                OnAnimationLoop?.Invoke(this);
            }
            else
            {
                _currentIndex -= _direction;
                Stop();
            }
        }

        CurrentFrameTimeRemaining = CurrentFrame.Duration;
    }
}
