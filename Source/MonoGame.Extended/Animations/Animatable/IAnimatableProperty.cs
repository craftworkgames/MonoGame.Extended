using System.Diagnostics.Contracts;

namespace MonoGame.Extended.Animations.Animatable
{
    public interface IAnimatableProperty
    {
         
    }
    public interface IAnimatableProperty<T> : IAnimatableProperty
    {
        T Property { get; set; }

    }
}