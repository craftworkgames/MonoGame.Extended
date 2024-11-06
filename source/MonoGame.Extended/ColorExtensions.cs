using System;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    /// Provides additional methods for working with color
    /// </summary>
    public static class ColorExtensions
    {
        public static Color FromHex(string value)
        {
            return ColorHelper.FromHex(value);
        }

        public static Color ToRgb(this HslColor c)
        {
            var h = c.H;
            var s = c.S;
            var l = c.L;

            if (s == 0f)
                return new Color(l, l, l);

            h = h/360f;
            var max = l < 0.5f ? l*(1 + s) : l + s - l*s;
            var min = 2f*l - max;

            return new Color(
                ComponentFromHue(min, max, h + 1f/3f),
                ComponentFromHue(min, max, h),
                ComponentFromHue(min, max, h - 1f/3f));
        }

        private static float ComponentFromHue(float m1, float m2, float h)
        {
            h = (h + 1f)%1f;
            if (h*6f < 1)
                return m1 + (m2 - m1)*6f*h;
            if (h*2 < 1)
                return m2;
            if (h*3 < 2)
                return m1 + (m2 - m1)*(2f/3f - h)*6f;
            return m1;
        }

        public static HslColor ToHsl(this Color c)
        {
            var r = c.R/255f;
            var b = c.B/255f;
            var g = c.G/255f;

            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var chroma = max - min;
            var sum = max + min;

            var l = sum*0.5f;

            if (chroma == 0)
                return new HslColor(0f, 0f, l);

            float h;

            if (r == max)
                h = (60*(g - b)/chroma + 360)%360;
            else
            {
                if (g == max)
                    h = 60*(b - r)/chroma + 120f;
                else
                    h = 60*(r - g)/chroma + 240f;
            }

            var s = l <= 0.5f ? chroma/sum : chroma/(2f - sum);

            return new HslColor(h, s, l);
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> value based on a packed value in the ABGR format.
        /// </summary>
        /// <remarks>
        /// This is useful for when you have HTML hex style values such as #123456 and want to use it in hex format for
        /// the parameter.  Since Color's standard format is RGBA, you would have to do new Color(0xFF563212) since R
        /// is the LSB.  With this method, you can write it the same way it is written in HTML hex by doing
        /// <c>>ColorExtensions.FromAbgr(0x123456FF);</c>
        /// </remarks>
        /// <param name="abgr">The packed color value in ABGR format</param>
        /// <returns>The <see cref="Color"/> value created</returns>
        public static Color FromAbgr(uint abgr)
        {
            uint rgba = (abgr & 0x000000FF) << 24 | // Alpha
                        (abgr & 0x0000FF00) << 8  | // Blue
                        (abgr & 0x00FF0000) >> 8  | // Green
                        (abgr & 0xFF000000) >> 24;  // Red

            Color result;

#if FNA
            result = default;
            result.PackedValue = rgba;
#else
            result = new Color(rgba);
#endif

            return result;
        }
	}
}
