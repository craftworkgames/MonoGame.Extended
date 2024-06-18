using System;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Graphics;

public class AnimatedSprite : Sprite, Animation
{
    private readonly SpriteSheetAnimationFrame[] _frames;
    private readonly Texture2DRegion[] _regions;
    private int _currentIndex;
    private bool _hasBegin;
    private double _speed;

    /// <inheritdoc/>
    public bool IsPaused { get; private set; }

    /// <inheritdoc/>
    public bool IsAnimating { get; private set; }

    /// <inheritdoc/>
    public bool IsLooping { get; set; }

    /// <inheritdoc/>
    public bool IsReversed { get; set; }

    /// <inheritdoc/>
    public bool IsPingPong { get; set; }

    /// <inheritdoc/>
    public double Speed
    {
        get => _speed;
        set => _speed = value < 0 ? 0 : value;
    }

    /// <inheritdoc/>
    public Action<Animation> OnFrameBegin { get; set; } = default;

    /// <inheritdoc/>
    public Action<Animation> OnFrameEnd { get; set; } = default;

    /// <inheritdoc/>
    public Action<Animation> OnAnimationBegin { get; set; } = default;

    /// <inheritdoc/>
    public Action<Animation> OnAnimationLoop { get; set; } = default;

    /// <inheritdoc/>
    public Action<Animation> OnAnimationCompleted { get; set; } = default;

    /// <inheritdoc/>
    public TimeSpan CurrentTimeRemaining { get; private set; }

    internal AnimatedSprite(SpriteSheetAnimationDefinition definition, Texture2DRegion[] regions)
        : base(regions[0])
    {
        _regions = regions;
        _frames = definition.Frames.ToArray();
        IsLooping = definition.IsLooping;
        IsReversed = definition.IsReversed;
        IsPingPong = definition.IsPingPong;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Pause()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Pause(bool resetFrameDuration)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Play()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Play(int startingFrame)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Reset()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetFrame(int index)
    {
        if(index < 0 || index >= _frames.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} cannot be less than zero or greater than or equal to the total number of frames in this animation.");
        }
    }

    /// <inheritdoc/>
    public bool Stop()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool UnPause()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool UnPause(bool advanceToNextFrame)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Update(double deltaTime) => Update(TimeSpan.FromSeconds(deltaTime));

    /// <inheritdoc/>
    public void Update(GameTime gameTime) => Update(gameTime.ElapsedGameTime);

    /// <inheritdoc/>
    public void Update(in TimeSpan elapsedTime)
    {
        if(!IsAnimating || IsPaused)
        {
            return;
        }

        if(!_hasBegin)
        {
            _hasBegin = true;
            OnAnimationBegin?.Invoke(this);
        }

        if(CurrentTimeRemaining == _frames[_currentIndex].Duration)
        {
            OnFrameBegin?.Invoke(this);
        }

        CurrentTimeRemaining -= elapsedTime * _speed;

        if(CurrentTimeRemaining <= TimeSpan.Zero)
        {
            AdvanceFrame();
        }
    }


}
