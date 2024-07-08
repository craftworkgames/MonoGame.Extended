// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;
using Xunit;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Tests.Animations;


public class AnimationTests
{
    private class TestAnimationFrame : IAnimationFrame
    {
        public int FrameIndex { get; set; }
        public TimeSpan Duration { get; set; }
    }

    private class TestAnimation : IAnimation
    {
        private readonly IAnimationFrame[] _frames;
        public string Name { get; set; }
        public ReadOnlySpan<IAnimationFrame> Frames => _frames;
        public int FrameCount { get; set; }
        public bool IsLooping { get; set; }
        public bool IsReversed { get; set; }
        public bool IsPingPong { get; set; }

        public TestAnimation(IAnimationFrame[] frames) => _frames = frames;
    }

    private readonly IAnimation _animation;
    private readonly IAnimationController _animationController;

    public AnimationTests()
    {
        var frames = new IAnimationFrame[]
        {
            new TestAnimationFrame { FrameIndex = 0, Duration = TimeSpan.FromSeconds(1) },
            new TestAnimationFrame { FrameIndex = 1, Duration = TimeSpan.FromSeconds(1) }
        };
        _animation = new TestAnimation(frames)
        {
            FrameCount = frames.Length,
            IsLooping = false,
            IsReversed = false,
            IsPingPong = false
        };

        _animationController = new AnimationController(_animation);
    }

    [Fact]
    public void Play_ShouldThrowException_ForInvalidFrame()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _animationController.Play(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => _animationController.Play(10));
    }

    [Fact]
    public void Pause_ShouldPauseAnimation()
    {
        _animationController.Play();
        var result = _animationController.Pause();

        Assert.True(result);
        Assert.True(_animationController.IsPaused);
    }

    [Fact]
    public void Pause_ShouldResetFrameDuration_WhenSpecified()
    {
        _animationController.Play();
        _animationController.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.5)));

        var result = _animationController.Pause(true);

        Assert.True(result);
        Assert.Equal(_animation.Frames[0].Duration, _animationController.CurrentFrameTimeRemaining);
    }

    [Fact]
    public void UnPause_ShouldResumeAnimation()
    {
        _animationController.Play();
        _animationController.Pause();
        var result = _animationController.Unpause();

        Assert.True(result);
        Assert.False(_animationController.IsPaused);
    }

    [Fact]
    public void UnPause_ShouldAdvanceFrame_WhenSpecified()
    {
        _animationController.Play();
        _animationController.Pause();
        var result = _animationController.Unpause(true);

        Assert.True(result);
        Assert.Equal(1, _animationController.CurrentFrame);
    }

    [Fact]
    public void Stop_ShouldStopAnimation()
    {
        _animationController.Play();
        var result = _animationController.Stop();

        Assert.True(result);
        Assert.False(_animationController.IsAnimating);
    }

    [Fact]
    public void Update_ShouldAdvanceFrame()
    {
        _animationController.Play();
        _animationController.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.1)));

        Assert.Equal(1, _animationController.CurrentFrame);
    }

    [Fact]
    public void Update_ShouldTriggerAnimationEvents()
    {
        bool frameBeginTriggered = false;
        bool frameEndTriggered = false;
        bool animationLoopTriggered = false;
        bool animationCompletedTriggered = false;

        _animationController.OnAnimationEvent += (anim, trigger) =>
        {
            switch (trigger)
            {
                case AnimationEventTrigger.FrameBegin:
                    frameBeginTriggered = true;
                    break;
                case AnimationEventTrigger.FrameEnd:
                    frameEndTriggered = true;
                    break;
                case AnimationEventTrigger.AnimationLoop:
                    animationLoopTriggered = true;
                    break;
                case AnimationEventTrigger.AnimationCompleted:
                    animationCompletedTriggered = true;
                    break;
            }
        };

        _animationController.Play();
        _animationController.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.1)));

        Assert.True(frameBeginTriggered);
        Assert.True(frameEndTriggered);

        _animationController.IsLooping = true;
        _animationController.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.1)));
        Assert.True(animationLoopTriggered);

        _animationController.IsLooping = false;
        _animationController.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(2)));
        Assert.True(animationCompletedTriggered);
    }

    [Fact]
    public void Update_ShouldLoopAnimation()
    {
        _animationController.IsLooping = true;
        _animationController.Play();
        _animationController.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(2.1)));

        Assert.Equal(0, _animationController.CurrentFrame);
    }

    [Fact]
    public void Reset_ShouldResetAnimation()
    {
        _animationController.Play();
        _animationController.Reset();

        Assert.False(_animationController.IsAnimating);
        Assert.True(_animationController.IsPaused);
        Assert.Equal(0, _animationController.CurrentFrame);
    }

    [Fact]
    public void SetFrame_ShouldChangeCurrentFrame()
    {
        _animationController.SetFrame(1);

        Assert.Equal(1, _animationController.CurrentFrame);
        Assert.Equal(_animation.Frames[1].Duration, _animationController.CurrentFrameTimeRemaining);
    }

    [Fact]
    public void SetFrame_ShouldThrowException_ForInvalidFrame()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _animationController.SetFrame(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => _animationController.SetFrame(10));
    }

    [Fact]
    public void Dispose_ShouldDisposeAnimation()
    {
        _animationController.Dispose();

        Assert.True(_animationController.IsDisposed);
    }
}
