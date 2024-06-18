// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;

namespace MonoGame.Extended.Animations;

public interface IAnimationDefinition
{
    string Name { get; }
    ReadOnlySpan<IAnimationFrame> Frames { get; }
    int FrameCount { get; }
    bool IsLooping { get; }
    bool IsReversed { get; }
    bool IsPingPong { get; }

}
