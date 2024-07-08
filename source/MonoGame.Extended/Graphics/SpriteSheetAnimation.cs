// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Graphics;

public class SpriteSheetAnimation : IAnimation
{
    private readonly SpriteSheetAnimationFrame[] _frames;

    public string Name { get; }
    public ReadOnlySpan<IAnimationFrame> Frames => _frames;
    public int FrameCount => _frames.Length;
    public bool IsLooping { get; }
    public bool IsReversed { get; }
    public bool IsPingPong { get; }

    internal SpriteSheetAnimation(string name, SpriteSheetAnimationFrame[] frames, bool isLooping, bool isReversed, bool isPingPong)
    {
        Name = name;
        IsLooping = isLooping;
        IsReversed = isReversed;
        IsPingPong = isPingPong;
        _frames = frames;
    }
}
