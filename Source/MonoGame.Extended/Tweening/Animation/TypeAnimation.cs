using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MonoGame.Extended.Tweening.Animation.Tracks;

namespace MonoGame.Extended.Tweening.Animation
{
    public class TypeAnimation<TType> : IAnimation where TType : class
    {
        public List<ITrack<TType>> Tracks { get; } = new List<ITrack<TType>>();

        public void Update(double time) {
            foreach (var track in Tracks) {
                track.Update(time);
            }
            foreach (var sub in _subTypeAnimations) {
                sub.Item1.Update(time);
            }
        }

        public void SetTransformable(TType transformable) {
            foreach (var track in Tracks) {
                track.Transformable = transformable;
            }
            foreach (var sub in _subTypeAnimations) {
                sub.Item3(sub.Item2.GetValue(transformable));
            }

        }

        private readonly List<Tuple<IAnimation, PropertyInfo, Action<object>>> _subTypeAnimations = new List<Tuple<IAnimation, PropertyInfo, Action<object>>>();
        public void AddSubTypeAnimation<T>(Expression<Func<TType, T>> propertySelector, TypeAnimation<T> animation) where T : class {
            var property = (propertySelector.Body as MemberExpression).Member as PropertyInfo;
            var a = _subTypeAnimations.RemoveAll(t => Equals(t.Item2, property));
            _subTypeAnimations.Add(new Tuple<IAnimation, PropertyInfo, Action<object>>(animation, property, o => animation.SetTransformable((T)o)));
        }
        public TypeAnimation<T> GetSubTypeAnimation<T>(string propertyname = null) where T : class {
            return _subTypeAnimations.
                Select(s => new { a = s.Item1 as TypeAnimation<T>, p = s.Item2 })
                .FirstOrDefault(t => t.a != null && (propertyname == null || t.p.Name == propertyname)).a;
        }
    }
}