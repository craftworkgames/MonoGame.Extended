namespace MonoGame.Extended.Particles
{
    public class ReleaseParameters
    {
        public ReleaseParameters()
        {
            Quantity = 1;
            Speed = RangeF.Parse("[-1.0,1.0]");
            Colour = new ColourRange(new Colour(0f, 0.5f, 0.5f), new Colour(360f, 0.5f, 0.5f));
            Opacity = RangeF.Parse("[0.0,1.0]");
            Scale = RangeF.Parse("[1.0,10.0]");
            Rotation = RangeF.Parse("[-3.14159,3.14159]");
            Mass = 1f;
        }

        public Range Quantity;
        public RangeF Speed;
        public ColourRange Colour;
        public RangeF Opacity;
        public RangeF Scale;
        public RangeF Rotation;
        public RangeF Mass;

    }
}