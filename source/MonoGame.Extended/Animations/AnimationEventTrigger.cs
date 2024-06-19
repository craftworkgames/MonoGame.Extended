// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.Animations;

public enum AnimationEventTrigger
{
    FrameBegin,
    FrameEnd,
    FrameSkipped,
    AnimationLoop,
    AnimationCompleted,
    AnimationStopped,

}
