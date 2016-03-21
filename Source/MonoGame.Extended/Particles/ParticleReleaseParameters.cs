namespace MonoGame.Extended.Particles
{
    public class ParticleReleaseParameters
    {
        public ParticleReleaseParameters()
        {
            Quantity = 1;
            Speed = RangeF.Parse("[-1.0,1.0]");
            Color = new ColorRange(new HslColor(0f, 0.5f, 0.5f), new HslColor(360f, 0.5f, 0.5f));
            Opacity = RangeF.Parse("[0.0,1.0]");
            Scale = RangeF.Parse("[1.0,10.0]");
            Rotation = RangeF.Parse("[-3.14159,3.14159]");
            Mass = 1f;
        }

        public Range Quantity { get; set; }
        public RangeF Speed { get; set; }
        public ColorRange Color { get; set; }
        public RangeF Opacity { get; set; }
        public RangeF Scale { get; set; }
        public RangeF Rotation { get; set; }
        public RangeF Mass { get; set; }
    }
}