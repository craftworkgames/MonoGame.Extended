// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.Animations;

public class AnimationEvent : EventArgs
{
    public IAnimation Animation { get; }
    public AnimationEventTrigger Trigger { get; }
    internal AnimationEvent(IAnimation animation, AnimationEventTrigger trigger) => (Animation, Trigger) = (animation, trigger);
}
