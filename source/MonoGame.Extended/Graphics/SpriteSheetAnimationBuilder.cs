// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// A builder class for creating <see cref="SpriteSheetAnimation"/> instances.
/// </summary>
public sealed class SpriteSheetAnimationBuilder
{
    private readonly string _name;
    private readonly SpriteSheet _spriteSheet;
    private readonly List<SpriteSheetAnimationFrame> _frames = new List<SpriteSheetAnimationFrame>();
    private bool _isLooping;
    private bool _isReversed;
    private bool _isPingPong;

    internal SpriteSheetAnimationBuilder(string name, SpriteSheet spriteSheet)
    {
        _name = name;
        _spriteSheet = spriteSheet;
    }

    /// <summary>
    /// Adds a frame to the animation using the region index and duration.
    /// </summary>
    /// <param name="regionIndex">The index of the region in the sprite sheet.</param>
    /// <param name="duration">The duration of the frame.</param>
    /// <returns>The <see cref="SpriteSheetAnimationBuilder"/> instance for chaining.</returns>
    public SpriteSheetAnimationBuilder AddFrame(int regionIndex, TimeSpan duration)
    {
        SpriteSheetAnimationFrame frame = new SpriteSheetAnimationFrame(regionIndex, duration);
        _frames.Add(frame);
        return this;
    }

    /// <summary>
    /// Adds a frame to the animation using the region name and duration.
    /// </summary>
    /// <param name="regionName">The name of the region in the sprite sheet.</param>
    /// <param name="duration">The duration of the frame.</param>
    /// <returns>The <see cref="SpriteSheetAnimationBuilder"/> instance for chaining.</returns>
    public SpriteSheetAnimationBuilder AddFrame(string regionName, TimeSpan duration)
    {
        int index = _spriteSheet.TextureAtlas.GetIndexOfRegion(regionName);
        return AddFrame(index, duration);
    }

    /// <summary>
    /// Sets whether the animation should loop.
    /// </summary>
    /// <param name="isLooping">If set to <c>true</c>, the animation will loop.</param>
    /// <returns>The <see cref="SpriteSheetAnimationBuilder"/> instance for chaining.</returns>
    public SpriteSheetAnimationBuilder IsLooping(bool isLooping)
    {
        _isLooping = isLooping;
        return this;
    }

    /// <summary>
    /// Sets whether the animation should be reversed.
    /// </summary>
    /// <param name="isReversed">If set to <c>true</c>, the animation will play in reverse.</param>
    /// <returns>The <see cref="SpriteSheetAnimationBuilder"/> instance for chaining.</returns>
    public SpriteSheetAnimationBuilder IsReversed(bool isReversed)
    {
        _isReversed = isReversed;
        return this;
    }

    /// <summary>
    /// Sets whether the animation should ping-pong (reverse direction at the ends).
    /// </summary>
    /// <param name="isPingPong">If set to <c>true</c>, the animation will ping-pong.</param>
    /// <returns>The <see cref="SpriteSheetAnimationBuilder"/> instance for chaining.</returns>
    public SpriteSheetAnimationBuilder IsPingPong(bool isPingPong)
    {
        _isPingPong = isPingPong;
        return this;
    }

    internal SpriteSheetAnimation Build() =>
        new SpriteSheetAnimation(_name, _frames.ToArray(), _isLooping, _isReversed, _isPingPong);
}
