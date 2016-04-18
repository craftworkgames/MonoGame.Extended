using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Tweening.Animation
{
    public class Animation : IAnimation
    {

        public Animation(string name) {
            Name = name;
        }

        public List<AnimationEvent> AnimationEvents { get; internal set; } = new List<AnimationEvent>();
        public List<IAnimation> SubAnimations { get; set; } = new List<IAnimation>();
        public event EventHandler AnimationEndEvent;
        public event EventHandler<AnimationEvent> AnimationEvent;
        public string Name { get; set; }
        public double Duration => SubAnimations.Max(s => s.Duration);

        public void AddChildAnimation(IAnimation sub) {
            SubAnimations.Add(sub);
        }

        public IAnimation Copy() {
            var result = (Animation)MemberwiseClone();
            result.SubAnimations = SubAnimations.Select(s => s.Copy()).ToList();
            result.AnimationEvents = AnimationEvents.Select(s => s.Copy()).ToList();
            return result;
        }

        //running
        public double Speed { get; set; } = 1;
        public double StartTime { get; internal set; }
        public void Update(double time) {
            time = (time - StartTime) * Speed;
            foreach (var subAnimation in SubAnimations) {
                subAnimation.Update(time);
            }
            foreach (var animationEvent in AnimationEvents.Where(animationEvent => animationEvent.Time <= time).ToArray()) {
                AnimationEvent?.Invoke(this, animationEvent);
                AnimationEvents.Remove(animationEvent);
            }
            if (time > Duration) End();
        }
        public bool Done { get; private set; }
        public void End() {
            AnimationEndEvent?.Invoke(this, EventArgs.Empty);
            AnimationEndEvent = null;
            SubAnimations = null;
            AnimationEvents = null;
            Done = true;
        }
    }
}