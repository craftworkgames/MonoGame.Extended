// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Graphics;

public class SpriteSheetAnimationCycle
{
    private SpriteSheetAnimationFrame[] _frames;

    /// <summary>
    /// Gets the name of this animation cycle.
    /// </summary>
    public string Name { get; }
    public ReadOnlySpan<SpriteSheetAnimationFrame> Frames => _frames;

    public SpriteSheetAnimationFrame this[int index] => GetFrame(index);

    public int FrameCount => _frames.Length;

    public bool IsLooping { get; set; }
    public bool IsReversed { get; set; }
    public bool IsPingPong { get; set; }

    internal SpriteSheetAnimationCycle(string name, SpriteSheetAnimationFrame[] frames, bool isLooping, bool isReversed, bool isPingPong)
    {
        Name = name;
        IsLooping = isLooping;
        IsReversed = isReversed;
        IsPingPong = isPingPong;
        _frames = frames;
    }

    public SpriteSheetAnimationFrame GetFrame(int index) => _frames[index];

}
