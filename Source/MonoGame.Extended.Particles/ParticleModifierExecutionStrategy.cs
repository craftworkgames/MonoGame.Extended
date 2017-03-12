using System.Collections.Generic;
using MonoGame.Extended.Particles.Modifiers;

namespace MonoGame.Extended.Particles
{
    using TPL = System.Threading.Tasks;

    public abstract class ParticleModifierExecutionStrategy
    {
        public static ParticleModifierExecutionStrategy Serial = new SerialModifierExecutionStrategy();
        public static ParticleModifierExecutionStrategy Parallel = new ParallelModifierExecutionStrategy();

        internal abstract void ExecuteModifiers(IEnumerable<IModifier> modifiers, float elapsedSeconds,
            ParticleBuffer.ParticleIterator iterator);

        internal class SerialModifierExecutionStrategy : ParticleModifierExecutionStrategy
        {
            internal override void ExecuteModifiers(IEnumerable<IModifier> modifiers, float elapsedSeconds,
                ParticleBuffer.ParticleIterator iterator)
            {
                foreach (var modifier in modifiers)
                    modifier.Update(elapsedSeconds, iterator.Reset());
            }
        }

        internal class ParallelModifierExecutionStrategy : ParticleModifierExecutionStrategy
        {
            internal override void ExecuteModifiers(IEnumerable<IModifier> modifiers, float elapsedSeconds,
                ParticleBuffer.ParticleIterator iterator)
            {
                TPL.Parallel.ForEach(modifiers, modifier => modifier.Update(elapsedSeconds, iterator.Reset()));
            }
        }
    }
}