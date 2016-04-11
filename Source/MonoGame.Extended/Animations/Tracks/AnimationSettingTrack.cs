using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Animations.Fluent;
using MonoGame.Extended.Animations.Transformations;

namespace MonoGame.Extended.Animations.Tracks
{
    public class AnimationSettingTrack<TTransformable> : IFluentSetting<TTransformable>, IAnimationTrack<TTransformable> where TTransformable : class
    {
        private readonly List<ISetTransform<TTransformable>> _transforms = new List<ISetTransform<TTransformable>>();
        public AnimationSettingTrack(ISetTransform<TTransformable>[] transforms) {
            Add(transforms);
        }

        public string Name { get; set; }
        public void Update(double time, TTransformable transformable) {
            var transform = _transforms.LastOrDefault(t => t.Time < time);
            transform?.Set(transformable);
        }

        public ITransform[] GetTransforms() {
            return _transforms.Cast<ITransform>().ToArray();
        }

        public double LastTime { get; private set; }
        public void Clear() {
            throw new NotImplementedException();
        }

        public void Add(ISetTransform<TTransformable>[] transforms) {
            foreach (var transform in transforms) {
                _transforms.RemoveAll(t => t.Time == transform.Time);
                _transforms.Add(transform);
            }
            _transforms.Sort((t1, t2) => (int)(t1.Time - t2.Time));
            LastTime = _transforms.Last().Time;
        }

        public bool Remove(ISetTransform<TTransformable> transform) {
            _transforms.Remove(transform);
            if (_transforms.Count < 0) return false;
            LastTime = _transforms.Last().Time;
            return true;
        }

    }
}