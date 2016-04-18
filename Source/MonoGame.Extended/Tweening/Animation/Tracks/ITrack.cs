using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace MonoGame.Extended.Tweening.Animation.Tracks
{
    public interface ITrack<TTransformable> where TTransformable : class
    {
        bool Interpolate { get; set; }
        TTransformable Transformable { get; set; }
        double LastEndtime { get; }
        void Update(double time);
        ITrack<TTransformable> Copy();
    }
    public interface ITrackValue<T>
    {
        List<Transformation<T>> Transforms { get; }
        void AddTransform(Transformation<T> transform);

    }
}