using MonoGame.Extended;

namespace MonoGame.Extended.Particles
{
    public static class FastRandomExtensions
    {
        public static void NextColor(this FastRandom random, out HslColor color, Range<HslColor> range)
        {
            var maxH = range.Max.H >= range.Min.H
                ? range.Max.H
                : range.Max.H + 360;
            color = new HslColor(random.NextSingle(range.Min.H, maxH),
                random.NextSingle(range.Min.S, range.Max.S),
                random.NextSingle(range.Min.L, range.Max.L));
        }
    }
}