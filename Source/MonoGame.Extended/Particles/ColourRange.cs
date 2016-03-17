using System;

namespace MonoGame.Extended.Particles
{
    public struct ColourRange
    {
        public ColourRange(Colour min, Colour max)
        {
            Min = min;
            Max = max;
        }

        public readonly Colour Min;
        public readonly Colour Max;

        public static ColourRange Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            try
            {
                var noBrackets = value.Substring(1, value.Length - 2);
                var colors = noBrackets.Split(';');
                var c1 = Colour.Parse(colors[0]);
                var c2 = Colour.Parse(colors[1]);
                return new ColourRange(c1, c2);
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

        public static implicit operator ColourRange(Colour value)
        {
            return new ColourRange(value, value);
        }
    }
}