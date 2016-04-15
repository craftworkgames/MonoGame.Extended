using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Maps.Tiled;

namespace MonoGame.Extended.Animations.Animatable
{
    public interface IAnimatable
    {
        T GetAnimatableValue<T>(string name);
        IEnumerable<IAnimatableProperty> GetAnimatableProperties();

    }
    public class test : IAnimatable
    {
        public T GetAnimatableValue<T>(string name) => props.OfType<AnimatableProperty<T>>().FirstOrDefault();

        private AnimatableProperty<double> value = 1.0;
        private IAnimatableProperty[] props;
        public IEnumerable<IAnimatableProperty> GetAnimatableProperties() {
            return props ?? (props = new IAnimatableProperty[] { value });
        }
    }
}