using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Particles.Modifiers
{
    public class ContainmentModifier : IModifier
    {
        public ContainmentModifier(IShapeF container, bool inside) {
            Container = container;
            Inside = inside;
        }

        public IShapeF Container { get; set; }
        public bool Inside { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            while (iterator.HasNext) {
                var p = iterator.Next();
                if (Inside) {
                    if(Container.Contains(p->Position)) continue;
                    p->Velocity = -p->Velocity;
                    //TODO collision stuff
                }
                else {
                    if (!Container.Contains(p->Position)) continue;
                    p->Velocity = -p->Velocity;
                }
            }
        }
    }
}