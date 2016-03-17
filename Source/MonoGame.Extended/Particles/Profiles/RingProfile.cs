namespace MonoGame.Extended.Particles.Profiles {
    public class RingProfile : Profile
    {
        public float Radius { get; set; }
        public CircleRadiation Radiate { get; set; }

        public override void GetOffsetAndHeading(out Vector offset, out Axis heading) {
            FastRand.NextUnitVector(out heading);

            if (Radiate == CircleRadiation.In)
                offset = new Vector(-heading.X * Radius, -heading.Y * Radius);
            else
                offset = new Vector(heading.X * Radius, heading.Y * Radius);

            if (Radiate == CircleRadiation.None)
                FastRand.NextUnitVector(out heading);
        }
    }
}