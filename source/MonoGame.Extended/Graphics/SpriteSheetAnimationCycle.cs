// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace MonoGame.Extended.Graphics;

public class SpriteSheetAnimationCycle
{
    public SpriteSheetAnimationCycle()
    {
        Frames = new List<SpriteSheetAnimationFrame>();
    }

    public float FrameDuration { get; set; } = 0.2f;
    public List<SpriteSheetAnimationFrame> Frames { get; set; }
    public bool IsLooping { get; set; }
    public bool IsReversed { get; set; }
    public bool IsPingPong { get; set; }
}
