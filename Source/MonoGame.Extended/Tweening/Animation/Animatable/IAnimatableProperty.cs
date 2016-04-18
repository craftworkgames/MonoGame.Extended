namespace MonoGame.Extended.Tweening.Animation.Animatable
{
    public interface IAnimatableProperty
    {
         
    }
    public interface IAnimatableProperty<T> : IAnimatableProperty
    {
        T Value { get; set; }

    }
    public class AnimatableProperty<T> : IAnimatableProperty<T>
    {
        public AnimatableProperty(T value) {
            Value = value;
        }
        public T Value { get; set; }
        public static implicit operator T (AnimatableProperty<T> property) {
            return property.Value;
        }
        public static implicit operator AnimatableProperty<T>(T value) {
            return new AnimatableProperty<T>(value);
        }

        public AnimatableProperty<string> TestAnimatableProperty { get; set; } = "Hello";
    }
}