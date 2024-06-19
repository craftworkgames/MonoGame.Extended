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

    private class TestAnimationDefinition : IAnimationDefinition
    {
        private readonly IAnimationFrame[] _frames;
        public string Name { get; set; }
        public ReadOnlySpan<IAnimationFrame> Frames => _frames;
        public int FrameCount { get; set; }
        public bool IsLooping { get; set; }
        public bool IsReversed { get; set; }
        public bool IsPingPong { get; set; }

        public TestAnimationDefinition(IAnimationFrame[] frames) => _frames = frames;
    }

    private readonly IAnimationDefinition _definition;
    private readonly Animation _animation;

    public AnimationTests()
    {
        var frames = new IAnimationFrame[]
        {
            new TestAnimationFrame { FrameIndex = 0, Duration = TimeSpan.FromSeconds(1) },
            new TestAnimationFrame { FrameIndex = 1, Duration = TimeSpan.FromSeconds(1) }
        };
        _definition = new TestAnimationDefinition(frames)
        {
            FrameCount = frames.Length,
            IsLooping = false,
            IsReversed = false,
            IsPingPong = false
        };
        _animation = new Animation(_definition);
    }

    [Fact]
    public void Play_ShouldStartAnimation()
    {
        var result = _animation.Play();

        Assert.True(result);
        Assert.True(_animation.IsAnimating);
        Assert.Equal(0, _animation.CurrentFrame);
    }

    [Fact]
    public void Play_ShouldThrowException_ForInvalidFrame()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _animation.Play(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => _animation.Play(10));
    }

    [Fact]
    public void Pause_ShouldPauseAnimation()
    {
        _animation.Play();
        var result = _animation.Pause();

        Assert.True(result);
        Assert.True(_animation.IsPaused);
    }

    [Fact]
    public void Pause_ShouldResetFrameDuration_WhenSpecified()
    {
        _animation.Play();
        _animation.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.5)));

        var result = _animation.Pause(true);

        Assert.True(result);
        Assert.Equal(_definition.Frames[0].Duration, _animation.CurrentFrameTimeRemaining);
    }

    [Fact]
    public void UnPause_ShouldResumeAnimation()
    {
        _animation.Play();
        _animation.Pause();
        var result = _animation.UnPause();

        Assert.True(result);
        Assert.False(_animation.IsPaused);
    }

    [Fact]
    public void UnPause_ShouldAdvanceFrame_WhenSpecified()
    {
        _animation.Play();
        _animation.Pause();
        var result = _animation.UnPause(true);

        Assert.True(result);
        Assert.Equal(1, _animation.CurrentFrame);
    }

    [Fact]
    public void Stop_ShouldStopAnimation()
    {
        _animation.Play();
        var result = _animation.Stop();

        Assert.True(result);
        Assert.False(_animation.IsAnimating);
    }

    [Fact]
    public void Update_ShouldAdvanceFrame()
    {
        _animation.Play();
        _animation.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.1)));

        Assert.Equal(1, _animation.CurrentFrame);
    }

    [Fact]
    public void Update_ShouldTriggerAnimationEvents()
    {
        bool frameBeginTriggered = false;
        bool frameEndTriggered = false;
        bool animationLoopTriggered = false;
        bool animationCompletedTriggered = false;

        _animation.OnAnimationEvent += (anim, trigger) =>
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

        _animation.Play();
        _animation.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.1)));

        Assert.True(frameBeginTriggered);
        Assert.True(frameEndTriggered);

        _animation.IsLooping = true;
        _animation.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.1)));
        Assert.True(animationLoopTriggered);

        _animation.IsLooping = false;
        _animation.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.1)));
        Assert.True(animationCompletedTriggered);
    }

    [Fact]
    public void Update_ShouldLoopAnimation()
    {
        _animation.IsLooping = true;
        _animation.Play();
        _animation.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(2.1)));

        Assert.Equal(0, _animation.CurrentFrame);
    }

    [Fact]
    public void Reset_ShouldResetAnimation()
    {
        _animation.Play();
        _animation.Reset();

        Assert.False(_animation.IsAnimating);
        Assert.True(_animation.IsPaused);
        Assert.Equal(0, _animation.CurrentFrame);
    }

    [Fact]
    public void SetFrame_ShouldChangeCurrentFrame()
    {
        _animation.SetFrame(1);

        Assert.Equal(1, _animation.CurrentFrame);
        Assert.Equal(_definition.Frames[1].Duration, _animation.CurrentFrameTimeRemaining);
    }

    [Fact]
    public void SetFrame_ShouldThrowException_ForInvalidFrame()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _animation.SetFrame(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => _animation.SetFrame(10));
    }

    [Fact]
    public void Dispose_ShouldDisposeAnimation()
    {
        _animation.Dispose();

        Assert.True(_animation.IsDisposed);
    }
}
