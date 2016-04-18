using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Particles.Profiles
{
    public class FillTargetProfile : Profile
    {
        public IShapeF Shape { get; set; }
        public IShapeF Target { get; set; }

        public FillTargetProfile(IShapeF shape, IShapeF target) {
            Shape = shape;
            Target = target;
        }

        public override void GetOffsetAndHeading(out Vector2 offset, out Vector2 heading) {
            offset = Shape.RandomPointInside();
            heading = Shape.RandomPointInside() - offset;
            heading.Normalize();
        }
    }
}