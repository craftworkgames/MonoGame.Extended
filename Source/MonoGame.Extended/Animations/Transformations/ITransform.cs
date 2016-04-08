using System;

namespace MonoGame.Extended.Animations.Transformations
{
    public interface ITransform
    {
        Type ValueType { get; }
        object ValueObject { get; }
        double Time { get; set; }
    }
    //interpolates the values between time and previous time (or 0)
    public interface ITweenTransform<TTransformable> : ITransform where TTransformable : class
    {
        /// <summary>
        /// Sets the value on the transformable based on the supplied time and previous transform (of the same kind).
        /// </summary>
        bool Update(double time, TTransformable transformable, ITweenTransform<TTransformable> previous);
        Easing Easing { get; set; }
    }
    //sets value at certain time
    public interface ISetTransform<in TTransformable> : ITransform where TTransformable : class
    {
        void Set(TTransformable transformable);
    }

}
