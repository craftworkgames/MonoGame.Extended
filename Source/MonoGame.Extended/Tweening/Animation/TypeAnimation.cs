using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MonoGame.Extended.Tweening.Animation.Tracks;

namespace MonoGame.Extended.Tweening.Animation
{
    public class TypeAnimation<TType> : IAnimation, ITypeAnimation<TType> where TType : class
    {
        public List<ITrack<TType>> Tracks { get; internal set; } = new List<ITrack<TType>>();
        public IAnimation ParentAnimation { get; internal set; }
        public TType Transformable { get; private set; }

        #region IAnimation

        public string Name { get; set; }

        public double Duration
            =>
                Math.Max(Tracks.Count > 0 ? Tracks.Max(t => t.LastEndtime) : 0,
                    _subTypeAnimations.Count > 0 ? _subTypeAnimations.Max(s => s.Item1.Duration) : 0);

        public double BlendFactor { get; set; } = 1;

        public void Update(double time) {
            foreach (var track in Tracks)
                track.Update(time, BlendFactor);
            foreach (var sub in _subTypeAnimations)
                sub.Item1.Update(time);
            if (time > Duration) End();
        }

        public void End() {
            Tracks = null;
            _subTypeAnimations = null;
        }

        public IAnimation Copy() {
            var result = (TypeAnimation<TType>) MemberwiseClone();
            result.SetTransformable(Transformable);
            result.Tracks = Tracks = Tracks.Select(t => t.Copy()).ToList();
            return result;
        }

        #endregion

        public void SetTransformable(TType transformable) {
            Transformable = transformable;
            foreach (var track in Tracks)
                track.Transformable = transformable;
            foreach (var sub in _subTypeAnimations)
                sub.Item3(sub.Item2.GetValue(transformable));
        }

        private List<Tuple<IAnimation, PropertyInfo, Action<object>>> _subTypeAnimations =
            new List<Tuple<IAnimation, PropertyInfo, Action<object>>>();

        public void AddSubTypeAnimation<T>(Expression<Func<TType, T>> propertySelector, TypeAnimation<T> animation)
            where T : class {
            var property = (propertySelector.Body as MemberExpression).Member as PropertyInfo;
            //_subTypeAnimations.RemoveAll(t => Equals(t.Item2, property));
            animation.ParentAnimation = this;
            _subTypeAnimations.Add(new Tuple<IAnimation, PropertyInfo, Action<object>>(animation, property,
                o => animation.SetTransformable((T) o)));
        }

        public TypeAnimation<T> GetSubTypeAnimation<T>(string propertyname = null) where T : class {
            return _subTypeAnimations.
                Select(s => new {a = s.Item1 as TypeAnimation<T>, p = s.Item2})
                .FirstOrDefault(t => t.a != null && (propertyname == null || t.p.Name == propertyname)).a;
        }
    }
}