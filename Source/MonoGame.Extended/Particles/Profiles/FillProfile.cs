using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Particles.Profiles
{
    public class FillProfile : Profile
    {
        public IShapeF Shape { get; set; }

        public FillProfile(IShapeF shape) {
            Shape = shape;
        }

        public override void GetOffsetAndHeading(out Vector2 offset, out Vector2 heading) {
            offset = Shape.RandomPointInside();
            FastRand.NextUnitVector(out heading);
        }
    }
}