using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Particles.Profiles
{
    public class OutlineProfile : Profile
    {
        public OutlineProfile(IShapeBase shape, CircleRadiation radiation, Vector2 center) {
            Shape = shape;
            Radiation = radiation;
            Center = center;
        }
        public OutlineProfile(IShapeBase shape) : this(shape, CircleRadiation.None, Vector2.Zero) { }

        public IShapeBase Shape { get; set; }
        public Vector2 Center { get; set; }
        public CircleRadiation Radiation { get; set; }
        public override void GetOffsetAndHeading(out Vector2 offset, out Vector2 heading) {
            offset = Shape.PointOnOutline(FastRand.NextSingle(0, 1));
            switch (Radiation) {
                case CircleRadiation.In:
                    heading = offset - Center;
                    heading.Normalize();
                    break;
                case CircleRadiation.Out:
                    heading = Center - offset;
                    heading.Normalize();
                    break;
                default:
                    FastRand.NextUnitVector(out heading);
                    break;
            }
        }
    }
}