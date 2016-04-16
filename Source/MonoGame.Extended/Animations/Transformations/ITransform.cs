using System;
using MonoGame.Extended.Tweening.Easing;

namespace MonoGame.Extended.Animations.Transformations
{
    public interface ITransform
    {
        /// <summary>
        /// Type of the property to transform.
        /// </summary>
        Type ValueType { get; }
        /// <summary>
        /// The value of the property to transform.
        /// </summary>
        object ValueObject { get; }
        /// <summary>
        /// The time at which the transform takes place (relative to start of animation).
        /// </summary>
        double Time { get; set; }
    }
    //interpolates the values between time and previous time (or 0)
    public interface ITweenTransform<TTransformable> : ITransform where TTransformable : class
    {
        /// <summary>
        /// Sets the value on the transformable based on the supplied time and previous transform (of the same kind).
        /// </summary>
        void Update(double time, TTransformable transformable, ITweenTransform<TTransformable> previous);
        /// <summary>
        /// The type of interpolation used.
        /// </summary>
        EasingFunction Easing { get; set; }
    }
    //sets value at certain time
    public interface ISetTransform<in TTransformable> : ITransform where TTransformable : class
    {
        void Set(TTransformable transformable);
    }

}
