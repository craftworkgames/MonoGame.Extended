using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MonoGame.Extended.Tweening.Animation.Tracks;

namespace MonoGame.Extended.Tweening.Animation
{
    public interface ITypeAnimation<TType> where TType : class
    {
        List<ITrack<TType>> Tracks { get; }
    }
}