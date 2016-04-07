using System;
using System.Diagnostics.Contracts;

namespace MonoGame.Extended.Animations.Transformation
{
    public interface ITransform
    {
        Type ValueType { get; }
        object ValueObject { get; }
        double Time { get; set; }
    }

    public interface ITweenTransform<TTransformable> : ITransform where TTransformable : class
    {
        /// <summary>
        /// Sets the value on the transformable based on the supplied time and previous transform (of the same kind).
        /// </summary>
        bool Update(double time, TTransformable transformable, ITweenTransform<TTransformable> previous);
        Easing Easing { get; set; }
    }
    public interface ISetTransform<in TTransformable> : ITransform where TTransformable : class
    {
        void Set(TTransformable transformable);
    }

}
