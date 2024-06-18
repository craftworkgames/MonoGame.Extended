// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Graphics;

public sealed class SpriteSheetAnimationDefinitionBuilder
{
    private readonly string _name;
    private readonly SpriteSheet _spriteSheet;
    private readonly List<SpriteSheetAnimationFrame> _frames = new List<SpriteSheetAnimationFrame>();
    private bool _isLooping;
    private bool _isReversed;
    private bool _isPingPong;

    internal SpriteSheetAnimationDefinitionBuilder(string name, SpriteSheet spriteSheet)
    {
        _name = name;
        _spriteSheet = spriteSheet;
    }

    public SpriteSheetAnimationDefinitionBuilder AddFrame(int regionIndex, TimeSpan duration)
    {
        SpriteSheetAnimationFrame frame = new SpriteSheetAnimationFrame(regionIndex, duration);
        _frames.Add(frame);
        return this;
    }

    public SpriteSheetAnimationDefinitionBuilder AddFrame(string regionName, TimeSpan duration)
    {
        int index = _spriteSheet.TextureAtlas.GetIndexOfRegion(regionName);
        return AddFrame(index, duration);
    }

    public SpriteSheetAnimationDefinitionBuilder IsLooping(bool isLooping)
    {
        _isLooping = isLooping;
        return this;
    }

    public SpriteSheetAnimationDefinitionBuilder IsReversed(bool isReversed)
    {
        _isReversed = isReversed;
        return this;
    }

    public SpriteSheetAnimationDefinitionBuilder IsPingPong(bool isPingPong)
    {
        _isPingPong = isPingPong;
        return this;
    }

    internal SpriteSheetAnimationDefinition Build() =>
        new SpriteSheetAnimationDefinition(_name, _frames.ToArray(), _isLooping, _isReversed, _isPingPong);
}
