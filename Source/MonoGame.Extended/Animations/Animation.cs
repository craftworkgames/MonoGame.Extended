using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monogame.Extended.Animations.Transformations;
using MonoGame.Extended.Animations.Tracks;
using MonoGame.Extended.Animations.Transformations;

namespace MonoGame.Extended.Animations
{
    public delegate void AnimationEventHandler(object sender, AnimationEvent e);

    public class Animation
    {
        private readonly Dictionary<object, IAnimationTrackGroup> _trackGroups =
            new Dictionary<object, IAnimationTrackGroup>();

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
            get
            {
                return _duration > 0 ? _duration : _trackGroups.Count > 0 ? _trackGroups.Values.Max(t => t.MaxEndtime) : 0;
            }
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

        //public ISetTransform<TTransformable> AddSetTransform<TTransformable, TValue>(TTransformable transformable,
        //    Expression<Func<TTransformable, TValue>> propertySelector, double time, TValue value)
        //    where TTransformable : class {
        //    var setPropertyTransform = new SetPropertyTransform<TTransformable, TValue>(propertySelector, time, value);
        //    AddTransformations(transformable, setPropertyTransform);
        //    return setPropertyTransform;
        //}

        public void AddAnimationTrackGroup<T>(AnimationTrackGroup<T> trackGroup) where T : class {
            _trackGroups.Add(trackGroup.Transformable, trackGroup);
        }

        //public ITweenTransform<TTransformable> AddTweenTransform<TTransformable, TValue>(
        //    TTransformable transformable, Expression<Func<TTransformable, TValue>> propertySelector, double time,
        //    TValue value, Easing easing = null)
        //    where TTransformable : class {
        //    var vType = typeof(TValue);
        //    dynamic ps = propertySelector;
        //    ITweenTransform<TTransformable> result;
        //    if (vType == typeof (Vector2))
        //        result = new Vector2Transform<TTransformable>(ps, time, (Vector2) (object) value, easing);
        //    else if (vType == typeof (float))
        //        result = new FloatTransform<TTransformable>(ps, time, (float) (object) value, easing);
        //    else if (vType == typeof(double))
        //        result = new DoubleTransform<TTransformable>(ps, time, (double)(object)value, easing);
        //    else if (vType == typeof(Color))
        //        result = new ColorTransform<TTransformable>(ps, time, (Color)(object)value, easing);
        //    else
        //        result = new DynamicTransform<TTransformable, TValue>(ps, time, value, easing);

        //    AddTransformations(transformable, result);
        //    return result;
        //}

        public void AddTransformations<TTransformable>(TTransformable transformable,
            params ITweenTransform<TTransformable>[] transforms)
            where TTransformable : class {
            IAnimationTrackGroup result;
            //check if we have a trackgroup of this transformable, if not -> add one
            if (!_trackGroups.TryGetValue(transformable, out result)) {
                _trackGroups.Add(transformable, result = new AnimationTrackGroup<TTransformable>(transformable));
            }
            (result as AnimationTrackGroup<TTransformable>).Add(transforms);
        }

        public void AddTransformations<TTransformable>(TTransformable transformable,
            params ISetTransform<TTransformable>[] transforms)
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