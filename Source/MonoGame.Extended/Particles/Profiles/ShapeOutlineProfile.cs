using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Particles.Profiles
{
    public class ShapeRandomProfile : Profile
    {
        public IShapeF Shape { get; set; }
        public ShapeRandomProfile(IShapeF shape) {
            Shape = shape;
        }
        public override void GetOffsetAndHeading(out Vector2 offset, out Axis heading) {
            offset = Shape.RandomPointInside();
            FastRand.NextUnitVector(out heading);
        }
    }
    public class ShapeSpawnTargetProfile : Profile
    {
        public ShapeSpawnTargetProfile(IShapeF spawn, IShapeF target) {
            Target = target;
            Spawn = spawn;
        }
        public IShapeF Spawn { get; set; }
        public IShapeF Target { get; set; }
        public override void GetOffsetAndHeading(out Vector2 offset, out Axis heading) {
            var t = FastRand.NextSingle(0, 1);
            offset = Spawn.PointOnOutline(t);
            var h = Target.PointOnOutline(t) - offset;
            h.Normalize();
            heading = new Axis(h.X, h.Y);
        }
    }

    public class ShapeOutlineProfile : Profile
    {
        public ShapeOutlineProfile(IShapeF shape, CircleRadiation radiation, Vector2 center) {
            Shape = shape;
            Radiation = radiation;
            Center = center;
        }

        public IShapeF Shape { get; set; }
        public Vector2 Center { get; set; }
        public CircleRadiation Radiation { get; set; }
        public override void GetOffsetAndHeading(out Vector2 offset, out Axis heading) {
            offset = Shape.PointOnOutline(FastRand.NextSingle(0, 1));
            switch (Radiation) {
                case CircleRadiation.In:
                    var i = offset - Center;
                    i.Normalize();
                    heading = new Axis(i.X, i.Y);
                    break;
                case CircleRadiation.Out:
                    var o = Center - offset;
                    o.Normalize();
                    heading = new Axis(o.X, o.Y);
                    break;
                default:
                    heading = new Axis(0, 0);
                    break;
            }
        }
    }
}