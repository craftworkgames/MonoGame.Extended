
using System;

namespace MonoGame.Extended.Tweening.Animation
{
    public class RangeTransformation<T> : Transformation<T> where T : IComparable<T>
    {
        public Range<T> Range { get; set; }
        public override Transformation<T> Copy() {
            return new Transformation<T> {
                Easing = Easing,
                Tween = Tween,
                Time = Time,
                Value = Range.Random()
            };
        }
    }
}