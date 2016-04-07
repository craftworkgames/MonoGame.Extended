using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Animations.Transformation;

namespace MonoGame.Extended.Animations.Tracks
{
    public class AnimationTweeningTrack<TTransformable> : IAnimationTrack<TTransformable>
       where TTransformable : class
    {
        public string Name { get; set; }
        private readonly List<ITweenTransform<TTransformable>> _transforms = new List<ITweenTransform<TTransformable>>();

        public ITransform[] GetTransforms() {
            return _transforms.Cast<ITransform>().ToArray();
        }

        public double LastTime { get; private set; }

        public AnimationTweeningTrack(params ITweenTransform<TTransformable>[] transforms) {
            Add(transforms);
        }
        public void Add(params ITweenTransform<TTransformable>[] transforms) {
            foreach (var transform in transforms) {
                _transforms.RemoveAll(t => t.Time == transform.Time);
                _transforms.Add(transform);
            }
            _transforms.Sort((t1, t2) => (int)(t1.Time - t2.Time));
            LastTime = _transforms.Last().Time;
        }

        public void Update(double time,TTransformable transformable) {
            for (int i = 0, n = _transforms.Count; i < n; i++) {
                var current = _transforms[i];
                if (current.Time >= time) { //tween between current and next
                    var previous = i == 0 ? current : _transforms[i - 1];
                    current.Update(time, transformable, previous);
                    return;
                }
                if (i == n - 1) current.Update(time, transformable, current);
            }
        }
        
    }
}