using System;

namespace MonoGame.Extended.Particles
{
    public struct ColorRange
    {
        public ColorRange(HslColor min, HslColor max)
        {
            Min = min;
            Max = max;
        }

        public HslColor Min { get; }
        public HslColor Max { get; }

        public static ColorRange Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            try
            {
                var noBrackets = value.Substring(1, value.Length - 2);
                var colors = noBrackets.Split(';');
                var c1 = HslColor.Parse(colors[0]);
                var c2 = HslColor.Parse(colors[1]);
                return new ColorRange(c1, c2);
            }
            catch
            {
                throw new FormatException(
                    "ColorRange should be formatted like: [HUE°, SAT, LUM;HUE°, SAT, LUM], but was " +
                    value);
            }
        }

        public override string ToString()
        {
            return "[" + Min + ';' + Max + ']';
        }

        public static implicit operator ColorRange(HslColor value)
        {
            return new ColorRange(value, value);
        }
    }
}