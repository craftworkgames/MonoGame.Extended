using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MonoGame.Extended.Animations.Transformation;

namespace MonoGame.Extended.Animations.Tracks
{
    public interface IAnimationTrackGroup
    {
        void Update(double time);
        double MaxEndtime { get; }
    }

    public class AnimationTrackGroup<TTransformable> : IAnimationTrackGroup where TTransformable : class
    {
        public TTransformable Transformable { get; set; }
        private readonly Dictionary<Type, IAnimationTrack<TTransformable>> _animationTracks = new Dictionary<Type, IAnimationTrack<TTransformable>>();

        public IAnimationTrack<TTransformable>[] GetTracks() {
            return _animationTracks.Values.ToArray();
        }

        public void Update(double time) {
            foreach (var track in _animationTracks.Values) {
                track.Update(time, Transformable);
            }
        }

        public AnimationTrackGroup(TTransformable transformable) {
            Transformable = transformable;
        }

        public double MaxEndtime => _animationTracks.Values.Max(t => t.LastTime);

        public void Add(params ISetTransform<TTransformable>[] transforms) {
            foreach (var setTransforms in transforms.GroupBy(t => t.GetType())) { //make separate tracks for each type
                IAnimationTrack<TTransformable> result;
                if (_animationTracks.TryGetValue(setTransforms.Key, out result)) {
                    (result as AnimationSettingTrack<TTransformable>).Add(setTransforms.ToArray());
                }
                else {
                    _animationTracks.Add(setTransforms.Key, new AnimationSettingTrack<TTransformable>(setTransforms.ToArray()));
                }
            }
        }
        public void Add(params ITweenTransform<TTransformable>[] transforms) {
            foreach (var tweenTransforms in transforms.GroupBy(t => t.GetType())) {
                IAnimationTrack<TTransformable> result;
                if (_animationTracks.TryGetValue(tweenTransforms.Key, out result)) {
                    (result as AnimationTweeningTrack<TTransformable>).Add(tweenTransforms.ToArray());
                }
                else {
                    _animationTracks.Add(tweenTransforms.Key, new AnimationTweeningTrack<TTransformable>(tweenTransforms.ToArray()));
                }
            }
        }
    }
}