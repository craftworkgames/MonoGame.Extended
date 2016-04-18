using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Extended.Tweening.Animation.Animatable
{
    public interface IAnimatable
    {
        T GetAnimatableValue<T>(string name);
        IEnumerable<IAnimatableProperty> GetAnimatableProperties();
    }
}