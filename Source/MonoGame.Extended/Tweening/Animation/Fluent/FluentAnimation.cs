using System.Collections.Generic;
using MonoGame.Extended.Tweening.Animation.Tracks;
using Newtonsoft.Json;

namespace MonoGame.Extended.Tweening.Animation.Fluent
{
    public class FluentTrack<T, TV> : ITypeAnimation<T>, ITrackValue<TV> where T : class
    {
        public Track<T, TV> Track { get; set; }
        public List<Transformation<TV>> Transforms => Track.Transforms;
        public void AddTransform(Transformation<TV> transform) => Track.AddTransform(transform);
        public ITypeAnimation<T> TypeAnimation { get; set; }
        public List<ITrack<T>> Tracks => TypeAnimation.Tracks;
    }
}