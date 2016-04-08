using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Animations.Tracks;
using MonoGame.Extended.Animations.Transformations;

namespace MonoGame.Extended.Animations
{
    public delegate void AnimationEventHandler(object sender, AnimationEvent e);
    public class Animation
    {
        private readonly Dictionary<object, IAnimationTrackGroup> _trackGroups = new Dictionary<object, IAnimationTrackGroup>();
        private readonly List<AnimationEvent> _animationEvents = new List<AnimationEvent>();
        private double _duration;

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
        public event EventHandler AnimationEndEvent;

        #region running
        internal double CurrentDuration;
        private List<IAnimationTrackGroup> _currentTrackGroups;
        private List<AnimationEvent> _currentEvents;
        internal bool RunDone;
        internal void StartRun() {
            RunDone = false;
            CurrentDuration = Duration;
            _currentTrackGroups = _trackGroups.Values.ToList();
            _currentEvents = _animationEvents.ToList();
        }
        #endregion

        public void Update(double time) {
            if (time >= CurrentDuration) {
                AnimationEndEvent?.Invoke(this, EventArgs.Empty);
                RunDone = true;
            }
            foreach (var animationEvent in _currentEvents.Where(animationEvent => animationEvent.Time <= time)) {
                AnimationEvent?.Invoke(this, animationEvent);
            }
            foreach (var trackGroup in _currentTrackGroups) {
                trackGroup.Update(time);
            }
            _currentTrackGroups.RemoveAll(t => t.MaxEndtime <= time);
            _currentEvents.RemoveAll(e => e.Time <= time);
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
        public void RemoveTransformations(object transformable) {
            IAnimationTrackGroup trackGroup;
            if (!_trackGroups.TryGetValue(transformable, out trackGroup)) return;
            trackGroup.Clear();
            _trackGroups.Remove(transformable);
        }
        public void AddEvents(params AnimationEvent[] events) {
            _animationEvents.AddRange(events);
        }
        public void RemoveEvent(AnimationEvent @event) {
            _animationEvents.Remove(@event);
        }
        public AnimationEvent GetEvent(string name) {
            return _animationEvents.Find(e => e.Name == name);
        }
    }
}