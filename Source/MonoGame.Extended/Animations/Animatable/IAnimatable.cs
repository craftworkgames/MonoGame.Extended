using System.Collections.Generic;

namespace MonoGame.Extended.Animations.Animatable
{
    public interface IAnimatable
    {
        T GetAnimatableValue<T>(string name);
        IEnumerable<IAnimatableProperty> GetAnimatableProperties();
    }
}