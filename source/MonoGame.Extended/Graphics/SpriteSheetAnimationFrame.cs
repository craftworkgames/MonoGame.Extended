// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Represents a single frame within a sprite sheet animation, including its index, display duration, and texture
/// region.
/// </summary>
[DebuggerDisplay("{Index} {Duration}")]
public class SpriteSheetAnimationFrame
{
    /// <summary>
    /// Gets the index of the frame in the overall sprite sheet.
    /// </summary>
    public int FrameIndex { get; }

    /// <summary>
    /// Gets the total duration this frame should be displayed during an animation.
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    /// Gets the source texture region that represents the texture to render during this frame of animation.
    /// </summary>
    public Texture2DRegion TextureRegion { get; }

    public SpriteSheetAnimationFrame(int index, Texture2DRegion region, TimeSpan duration)
    {
        ArgumentNullException.ThrowIfNull(region);
        if (region.Texture.IsDisposed)
        {
            throw new ObjectDisposedException(nameof(region), $"The source {nameof(Texture2D)} of {nameof(region)} was disposed prior.");
        }

        FrameIndex = index;
        Duration = duration;
    }
}