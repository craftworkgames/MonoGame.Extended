using System;
using System.Diagnostics.Contracts;
using MonoGame.Extended.Animations.Transformations;

namespace MonoGame.Extended.Animations.Tracks
{
    public interface IAnimationTrack<in T>
    {
        string Name { get; set; }
        void Update(double time, T transformable);
        ITransform[] GetTransforms();
        double LastTime { get; }
        void Clear();
        
    }
}