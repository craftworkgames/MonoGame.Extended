using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MonoGame.Extended.Animations.Transformations;

namespace MonoGame.Extended.Animations.Tracks
{
    public interface IAnimationTrack<T>
    {
        string Name { get; set; }
        void Update(double time, T transformable);
        IEnumerable<ITransform> GetTransforms();
        double LastTime { get; }
        void Clear();
        bool Equals(IAnimationTrack<T> other);
    }
}