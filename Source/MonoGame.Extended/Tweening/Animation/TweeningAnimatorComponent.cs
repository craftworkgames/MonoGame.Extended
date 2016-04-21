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

        public void RunAnimation(Animation animation, AnimationRunParameters parameters, AnimationBlendParameters blend) {
            if (parameters.OverrideLayer)
                _runningAnimations.RemoveAll(a => a.Layer == animation.Layer);
            if (parameters.RepeatCount <= 0) {
                var a = (Animation)animation.Copy();
                a.KeepRunning = true;
                a.StartTime = _currentTime;
                _runningAnimations.Add(a);
                return;
            }
            var duration = animation.Duration;
            for (int i = 0; i < parameters.RepeatCount; i++) {
                var a = (Animation)animation.Copy();
                a.StartTime = _currentTime + duration * i;
                _runningAnimations.Add(a);
            }
        }

        public Animation RunAnimation(string name, AnimationRunParameters parameters, AnimationBlendParameters blend) {
            var animation = _animations.Find(a => a.Name == name);
            if (animation == null) return null;
            RunAnimation(animation, parameters, blend);
            return animation;
        }
    }
    public struct AnimationRunParameters
    {
        public int RepeatCount { get; set; }
        public bool KeepRunning
        {
            get { return RepeatCount < 1; }
            set { RepeatCount = value ? -1 : 1; }
        }
        public bool OverrideLayer { get; set; }
    }
    public struct AnimationBlendParameters
    {
        public double InDuration { get; set; }
        public double OutDuration { get; set; }
        public double StartValue { get; set; }
        public double Endvalue { get; set; }
        public static AnimationBlendParameters None = new AnimationBlendParameters();
    }

}