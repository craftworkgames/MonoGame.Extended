using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations
{
    public class Animator : GameComponent
    {
        private Dictionary<int, List<Animation>> _layerAnimations = new Dictionary<int, List<Animation>>();
        private readonly List<RunningAnimation> _runningAnimations = new List<RunningAnimation>();
        private double _currentTime;
        private List<Animation> _animations = new List<Animation>();

        public Animator(Game game) : base(game) { }

        public void AddAnimation(Animation animation, int layer = 0) {
            _layerAnimations.GetOrAddList(layer).Add(animation);
        }

        public override void Update(GameTime gameTime) {
            _currentTime = gameTime.TotalGameTime.TotalMilliseconds;
            foreach (var runningAnimation in _runningAnimations) {
                var time = _currentTime - runningAnimation.StartTime;
                var duration = runningAnimation.Animation.Duration;
                if (runningAnimation.Keeprunning) {
                    time %= duration;
                }
                else {
                    if (time > duration) {
                        time = duration;
                        runningAnimation.Done = true;
                    }
                }
                runningAnimation.Animation.Update(time);
            }
            _runningAnimations.RemoveAll(a => !a.Keeprunning && a.Done);
        }

        public void StopAllRunning() {
            _runningAnimations.Clear();
        }

        public Animation RunAnimation(string name, bool keeprunning, bool overridelayer = true) {
            var animation = _animations.Find(a => a.Name == name);
            if (animation == null) return null;
            _runningAnimations.RemoveAll(a => a.Animation == animation || overridelayer && (a.Animation.AnimationLayer == animation.AnimationLayer));
            _runningAnimations.Add(new RunningAnimation { StartTime = _currentTime, Keeprunning = keeprunning, Animation = animation });
            return animation;
        }
        private class RunningAnimation
        {
            public double StartTime;
            public bool Keeprunning;
            public bool Done;
            public Animation Animation;
        }
        private class BlendingAnimation
        {
            public RunningAnimation A;
            public RunningAnimation B;
            public double starttime;
            public double duration;
        }

    }
}