using MonoGame.Extended.Animations.Transformations;

namespace MonoGame.Extended.Animations.Fluent
{
    public interface IFluentTweening<T> where T : class
    {
        void Add(params ITweenTransform<T>[] transforms);
    }
    public interface IFluentSetting<T> where T : class
    {
        void Add(params ISetTransform<T>[] transforms);
    }
    public interface IFluentBoth<T> : IFluentSetting<T>, IFluentTweening<T> where T : class { }

    public class FluentAnimation<T> : IFluentBoth<T> where T : class
    {
        public Animation Animation { get; set; }
        public T Transformable { get; set; }
        public void Add(params ITweenTransform<T>[] transforms) {
            Animation.AddTransformations(Transformable, transforms);
        }
        public void Add(params ISetTransform<T>[] transforms) {
            Animation.AddTransformations(Transformable, transforms);
        }
    }
}