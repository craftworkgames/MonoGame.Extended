// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;

namespace MonoGame.Extended.Animations;

public interface IAnimation : IUpdate
{
    bool IsPaused { get; }
    bool IsAnimating { get; }
    bool IsLooping { get; set; }
    bool IsReversed { get; set; }
    bool IsPingPong { get; set; }
    double Speed { get; set; }
    Action<Animation> OnFrameBegin { get; set; }
    Action<Animation> OnFrameEnd { get; set; }
    Action<Animation> OnAnimationLoop { get; set; }
    Action<Animation> OnAnimationCompleted { get; set; }
    TimeSpan CurrentFrameTimeRemaining { get; }
    IAnimationFrame CurrentFrame { get; }

    void SetFrame(int index);
    bool Play();
    bool Play(int startingFrame);
    bool Pause();
    bool Pause(bool resetFrameDuration);
    bool UnPause();
    bool UnPause(bool advanceToNextFrame);
    bool Stop();
    void Reset();
}
