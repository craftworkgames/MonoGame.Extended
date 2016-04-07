using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Animations.Tracks;
using MonoGame.Extended.Animations.Transformation;

namespace MonoGame.Extended.Animations
{
    public delegate void AnimationEventHandler(object sender, AnimationEvent e);
    public class Animation
    {
        private readonly Dictionary<object, IAnimationTrackGroup> _trackGroups = new Dictionary<object, IAnimationTrackGroup>();
        private double _duration;
        public List<AnimationEvent> AnimationEvents { get; } = new List<AnimationEvent>();

        public Animation(string name, double duration = -1) {
            Name = name;
            Duration = duration;
        }

        public string Name { get; set; }
        public int AnimationLayer { get; set; }
        public double Duration
        {
            get { return _duration > 0 ? _duration : _trackGroups.Values.Max(t => t.MaxEndtime); }
            set { _duration = value; }
        }

        public event EventHandler<AnimationEvent> AnimationEvent;

        public void Update(double time) {
            foreach (var animationEvent in AnimationEvents) {
                //TODO if correct time
                AnimationEvent?.Invoke(this, animationEvent);
            }
            foreach (var animationTrackGroup in _trackGroups.Values) {
                animationTrackGroup.Update(time);
            }
        }


        public void AddTransfromations<TTransformable>(TTransformable transformable, params ITweenTransform<TTransformable>[] transforms)
            where TTransformable : class {
            IAnimationTrackGroup result;
            //check if we have a trackgroup of this transformable, if not -> add one
            if (!_trackGroups.TryGetValue(transformable, out result)) {
                _trackGroups.Add(transformable, result = new AnimationTrackGroup<TTransformable>(transformable));
            }
            (result as AnimationTrackGroup<TTransformable>).Add(transforms);
        }

        public void AddTransfromations<TTransformable>(TTransformable transformable, params ISetTransform<TTransformable>[] transforms)
            where TTransformable : class {
            IAnimationTrackGroup result;
            //check if we have a trackgroup of this transformable, if not -> add one
            if (!_trackGroups.TryGetValue(transformable, out result)) {
                _trackGroups.Add(transformable, result = new AnimationTrackGroup<TTransformable>(transformable));
            }
            (result as AnimationTrackGroup<TTransformable>).Add(transforms);
        }
    }
}