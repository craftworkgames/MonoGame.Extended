using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MonoGame.Extended.Animations.Fluent;
using MonoGame.Extended.Animations.Transformations;

namespace MonoGame.Extended.Animations.Tracks
{
    public class AnimationTrackGroup<TTransformable> : IFluentBoth<TTransformable>, IAnimationTrackGroup where TTransformable : class
    {
        public TTransformable Transformable { get; set; }
        private readonly Dictionary<Type, IAnimationTrack<TTransformable>> _animationTracks = new Dictionary<Type, IAnimationTrack<TTransformable>>();
        //buffer for performance reasons when adding transforms.
        private readonly List<ITweenTransform<TTransformable>> _dirtyTweens = new List<ITweenTransform<TTransformable>>();
        private readonly List<ISetTransform<TTransformable>> _dirtySets = new List<ISetTransform<TTransformable>>();
        private bool _dirty;
        public IAnimationTrack<TTransformable>[] GetTracks() {
            return _animationTracks.Values.ToArray();
        }

        internal Dictionary<Type,IAnimationTrack<TTransformable>> AnimationTracks
        {
            get
            {
                if (_dirty) {
                    foreach (var setTransforms in _dirtySets.GroupBy(t => t.GetType())) { //make separate tracks for each type
                        IAnimationTrack<TTransformable> result;
                        if (_animationTracks.TryGetValue(setTransforms.Key, out result)) {
                            (result as AnimationSettingTrack<TTransformable>).Add(setTransforms.ToArray());
                        }
                        else {
                            _animationTracks.Add(setTransforms.Key, new AnimationSettingTrack<TTransformable>(setTransforms.ToArray()));
                        }
                    }
                    _dirtySets.Clear();
                    foreach (var tweenTransforms in _dirtyTweens.GroupBy(t => t.GetType())) {
                        IAnimationTrack<TTransformable> result;
                        if (_animationTracks.TryGetValue(tweenTransforms.Key, out result)) {
                            (result as AnimationTweeningTrack<TTransformable>).Add(tweenTransforms.ToArray());
                        }
                        else {
                            _animationTracks.Add(tweenTransforms.Key, new AnimationTweeningTrack<TTransformable>(tweenTransforms.ToArray()));
                        }
                    }
                    _dirtyTweens.Clear();
                }
                return _animationTracks;
            }
        } 

        public void Update(double time) {
            
            foreach (var track in AnimationTracks.Values) {
                track.Update(time, Transformable);
            }
        }

        public AnimationTrackGroup(TTransformable transformable) {
            Transformable = transformable;
        }

        public double MaxEndtime => AnimationTracks.Values.Max(t => t.LastTime);
        public void Clear() {
            foreach (var animationTrack in AnimationTracks.Values) {
                animationTrack.Clear();
            }
            
        }

        public void Add(params ISetTransform<TTransformable>[] transforms) {
            _dirtySets.AddRange(transforms);
            _dirty = true;
        }
        public void Add(params ITweenTransform<TTransformable>[] transforms) {
            _dirtyTweens.AddRange(transforms);
            _dirty = true;
        }
        public void Remove(ISetTransform<TTransformable> transform) {
            IAnimationTrack<TTransformable> result;
            var key = transform.GetType();
            if (!AnimationTracks.TryGetValue(key, out result))  return;
            if (!(result as AnimationSettingTrack<TTransformable>).Remove(transform)) {
                AnimationTracks.Remove(key);
            }
        }
        public void Remove(ITweenTransform<TTransformable> transform) {
            IAnimationTrack<TTransformable> result;
            var key = transform.GetType();
            if (!AnimationTracks.TryGetValue(key, out result))  return;
            if (!(result as AnimationTweeningTrack<TTransformable>).Remove(transform)) {
                AnimationTracks.Remove(key);
            }
        }
    }
}