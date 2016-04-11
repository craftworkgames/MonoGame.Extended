using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Particles.Profiles
{
    public class SpawnTargetProfile : Profile
    {
        public SpawnTargetProfile(IShapeBase spawn, IShapeBase target) {
            Target = target;
            Spawn = spawn;
        }
        public IShapeBase Spawn { get; set; }
        public IShapeBase Target { get; set; }
        public override void GetOffsetAndHeading(out Vector2 offset, out Vector2 heading) {
            var t = FastRand.NextSingle(0, 1);
            offset = Spawn.PointOnOutline(t);
            heading = Target.PointOnOutline(t) - offset;
            heading.Normalize();
        }
    }
}