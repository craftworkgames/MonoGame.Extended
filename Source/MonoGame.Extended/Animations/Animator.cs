using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations
{
    public class Animator : GameComponent
    {
        private readonly List<RunningAnimation> _runningAnimations = new List<RunningAnimation>();
        private double _currentTime;
        private readonly List<Animation> _animations = new List<Animation>();

        public Animator(Game game) : base(game) { }

        public void AddAnimation(Animation animation) {
            _animations.Add(animation);
        }

        public override void Update(GameTime gameTime) {
            _currentTime = gameTime.TotalGameTime.TotalMilliseconds;
            foreach (var runningAnimation in _runningAnimations) {
                var time = _currentTime - runningAnimation.StartTime;
                var duration = runningAnimation.Animation.Duration;
                if (time > duration) {
                    if (runningAnimation.Keeprunning) {
                        time %= duration;
                    }
                    else {
                        time = duration;
                    }
                }
                runningAnimation.Animation.Update(time);
            }
            _runningAnimations.RemoveAll(a => !a.Keeprunning && a.Animation.RunDone);
        }

        public void StopAllRunning() {
            _runningAnimations.Clear();
        }

        public void RunAnimation(Animation animation, bool keeprunning, bool overridelayer = true) {
            _runningAnimations.RemoveAll(a => a.Animation == animation || overridelayer && (a.Animation.AnimationLayer == animation.AnimationLayer));
            _runningAnimations.Add(new RunningAnimation { StartTime = _currentTime, Keeprunning = keeprunning, Animation = animation });
            animation.StartRun();
        }

        public Animation RunAnimation(string name, bool keeprunning, bool overridelayer = true) {
            var animation = _animations.Find(a => a.Name == name);
            if (animation == null) return null;
            RunAnimation(animation, keeprunning, overridelayer);
            return animation;
        }
        private class RunningAnimation
        {
            public double StartTime;
            public bool Keeprunning;
            public Animation Animation;
        }
        //private class BlendingAnimation
        //{
        //    public RunningAnimation A;
        //    public RunningAnimation B;
        //    public double starttime;
        //    public double duration;
        //}

    }
}