﻿using System;
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
            var r = int.Parse(value.Substring(1, 2), NumberStyles.HexNumber);
            var g = int.Parse(value.Substring(3, 2), NumberStyles.HexNumber);
            var b = int.Parse(value.Substring(5, 2), NumberStyles.HexNumber);
            var a = value.Length > 7 ? int.Parse(value.Substring(7, 2), NumberStyles.HexNumber) : 255;
            return new Color(r, g, b, a);
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
	}
}
