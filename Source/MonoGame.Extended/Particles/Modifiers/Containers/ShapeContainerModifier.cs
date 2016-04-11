using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Particles.Modifiers.Containers
{
    public class ShapeContainerModifier : IModifier
    {
        public IShapeF Shape { get; set; }
        public bool Inside { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            while (iterator.HasNext) {
                var p = iterator.Next();
                if (Inside) {
                    if(Shape.Contains(p->Position)) continue;
                    //TODO add flip
                }
                else {
                    if (!Shape.Contains(p->Position)) continue;
                    //TODO add flip
                }
            }
        }
    }
}