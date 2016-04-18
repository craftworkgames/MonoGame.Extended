using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Animation
{
    public class TweeningAnimatorComponent : GameComponent
    {
        private readonly List<Animation> _runningAnimations = new List<Animation>();
        private double _currentTime;
        private readonly List<Animation> _animations = new List<Animation>();

        public TweeningAnimatorComponent(Game game) : base(game) { }

        public void AddAnimation(Animation animation) {
            _animations.Add(animation);
        }

        public override void Update(GameTime gameTime) {
            _currentTime = gameTime.TotalGameTime.TotalSeconds;
            foreach (var runningAnimation in _runningAnimations.Where(r => r.StartTime <= _currentTime)) {
                runningAnimation.Update(_currentTime);
            }
            _runningAnimations.RemoveAll(a => a.Done);
        }

        public void StopAllRunning() {
            _runningAnimations.Clear();
        }

        public void RunAnimation(Animation animation, int repeat = 1, bool overridelayer = true) {
            //_runningAnimations.RemoveAll(a => a == animation || overridelayer && (a.Animation.AnimationLayer == animation.AnimationLayer));
            var duration = animation.Duration;
            for (int i = 0; i < repeat; i++) {
                var a = (Animation)animation.Copy();
                a.StartTime = _currentTime + duration * i;
                _runningAnimations.Add(a);
            }
        }

        public Animation RunAnimation(string name, int repeat = 1, bool overridelayer = true) {
            var animation = _animations.Find(a => a.Name == name);
            if (animation == null) return null;
            RunAnimation(animation, repeat, overridelayer);
            return animation;
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