// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Represents a single frame within a sprite sheet animation, including its index, display duration, and texture
/// region.
/// </summary>
[DebuggerDisplay("{Index} {Duration}")]
internal class SpriteSheetAnimationFrame : IAnimationFrame
{
    /// <summary>
    /// Gets the index of the frame in the overall sprite sheet.
    /// </summary>
    public int FrameIndex { get; }

    /// <summary>
    /// Gets the total duration this frame should be displayed during an animation.
    /// </summary>
    public TimeSpan Duration { get; }

    internal SpriteSheetAnimationFrame(int index, TimeSpan duration)
    {
        FrameIndex = index;
        Duration = duration;
    }
}
