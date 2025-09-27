using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    /// Represents a color in the HSL (Hue, Saturation, Lightness) color space.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>Hue (H) represents the color, ranging from 0 to 360 degrees on the color wheel.</item>
    ///   <item>Saturation (S) represents the intensity of the color, ranging from 0.0 (gray) to 1.0 (full color).</item>
    ///   <item>Lightness (L) represents the brightness, ranging from 0.0 (black) to 1.0 (white).</item>
    /// </list>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct HslColor : IEquatable<HslColor>, IComparable<HslColor>
    {

        private float _h;
        private float _s;
        private float _l;

        /// <summary>
        /// The hue component value (in degrees) of the color ranging from 0.0 to 360.0.
        /// </summary>
        public readonly float H => _h;

        /// <summary>
        /// The saturation component value of the color ranging from 0.0 to 1.0.
        /// </summary>
        public readonly float S => _s;

        /// <summary>
        /// The lightness component value of the color ranging from 0.0 to 1.0.
        /// </summary>
        public readonly float L => _l;

        /// <summary>
        /// Normalizes a hue value to be within the range [0, 360).
        /// Handles negative values by wrapping them around.
        /// </summary>
        /// <param name="h">The hue value to normalize.</param>
        /// <returns>The normalized hue value.</returns>
        private static float NormalizeHue(float h)
        {
            if (h < 0) return h + 360 * ((int)(h / 360) + 1);
            return h % 360;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HslColor"/> struct with the specified hue, saturation,
        /// and lightness component values.
        /// </summary>
        /// <param name="h">The hue component value (in degrees) from 0.0 to 360.0.</param>
        /// <param name="s">The saturation component value from 0.0 to 1.0.</param>
        /// <param name="l">The lightness component value from 0.0 to 1.0.</param>
        public HslColor(float h, float s, float l)
        {
            _h = Math.Clamp(h, 0.0f, 360.0f);
            _s = Math.Clamp(s, 0.0f, 1.0f);
            _l = Math.Clamp(l, 0.0f, 1.0f);
        }

        /// <summary>
        /// Copies the values of this <see cref="HslColor"/> struct to a new instance.
        /// </summary>
        /// <param name="destination">When this method returns, contains a copy of this <see cref="HslColor"/>.</param>
        [Obsolete("Use CopyToRef instead.  This will be removed in the next major SemVer release.")]
        public readonly void CopyTo(out HslColor destination)
        {
            destination = new HslColor(H, S, L);
        }

        /// <summary>
        /// Copies the value of this <see cref="HslColor"/> struct to an existing destination.
        /// </summary>
        /// <param name="destination">A reference to the destination <see cref="HslColor"/> struct where values will be copied to.</param>
        /// <remarks>
        /// This method directly modifies the internal components of the destination struct for improved performance.
        /// Unlike typical operations on immutable structs, this method does not create a new instance but alters
        /// the existing one in-place. It should be used only in scenarios where performance is critical.
        /// </remarks>
        public readonly void CopyToRef(ref HslColor destination)
        {
            destination._h = _h;
            destination._s = _s;
            destination._l = _l;
        }

        /// <summary>
        /// Deconstructs this <see cref="HslColor"/>  into its hue, saturation, and lightness component values.
        /// </summary>
        /// <param name="h">When this method returns, contains the hue component value of this <see cref="HslColor"/>.</param>
        /// <param name="s">When this method returns, contains the saturation component value of this <see cref="HslColor"/>.</param>
        /// <param name="l">When this method returns, contains the lightness component value of this <see cref="HslColor"/>.</param>
        [Obsolete("Will be removed in next major SemVer release.  Use Deconstruct instead.")]
        public readonly void Destructure(out float h, out float s, out float l)
        {
            h = H;
            s = S;
            l = L;
        }

        /// <summary>
        /// Deconstructs this <see cref="HslColor"/>  into its hue, saturation, and lightness component values.
        /// </summary>
        /// <param name="h">When this method returns, contains the hue component value of this <see cref="HslColor"/>.</param>
        /// <param name="s">When this method returns, contains the saturation component value of this <see cref="HslColor"/>.</param>
        /// <param name="l">When this method returns, contains the lightness component value of this <see cref="HslColor"/>.</param>
        public readonly void Deconstruct(out float h, out float s, out float l)
        {
            h = H;
            s = S;
            l = L;
        }

        /// <summary>
        /// Executes a callback with the components of this <see cref="HslColor"/>.
        /// </summary>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>
        public readonly void Match(Action<float, float, float> callback)
        {
            ArgumentNullException.ThrowIfNull(callback);
            callback(H, S, L);
        }

        /// <summary>
        /// Maps the components of this <see cref="HslColor"/> to a new value using the specified mapping function.
        /// </summary>
        /// <typeparam name="T">The type of the result of the mapping function.</typeparam>
        /// <param name="map">The mapping function to apply to the components of this <see cref="HslColor"/>.</param>
        /// <returns>
        /// The result of applying the mapping function to the components of this <see cref="HslColor"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="map"/> is <see langword="null"/>.
        /// </exception>
        public readonly T Map<T>(Func<float, float, float, T> map)
        {
            ArgumentNullException.ThrowIfNull(map);
            return map(H, S, L);
        }

        /// <summary>
        /// Implicitly converts a string to an <see cref="HslColor"/>.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>The <see cref="HslColor"/> represented by the string.</returns>
        [Obsolete("Use HslColor.Parse instead to make string parsing explicit and improve code readability.  This method will be removed in the next major SemVer release.")]
        public static implicit operator HslColor(string value)
        {
            return Parse(value);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This comparison uses a lexicographic approach that establishes a hierarchy among the HSL components:
        ///
        /// <list type="bullet">
        ///   <item>Hue is the primary sorting factor</item>
        ///   <item>Lightness is the secondary sorting factor</item>
        ///   <item>Saturation is the tertiary sorting factor</item>
        /// </list>
        ///
        /// The comparison returns the result of the first differing component, ensuring that hue differences
        /// take precedence over lightness differences, which in turn take precedence over saturation differences.
        /// This creates a consistent and predictable ordering for color intervals while prioritizing the most
        /// visually significant color properties.
        /// </remarks>
        public readonly int CompareTo(HslColor other)
        {
            int result = _h.CompareTo(other._h);
            if (result != 0)
            {
                return result;
            }

            result = _l.CompareTo(other._l);
            if (result != 0)
            {
                return result;
            }

            return _s.CompareTo(other._s);
        }

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is HslColor other && Equals(other);
        }

        /// <inheritdoc/>
        public readonly bool Equals(HslColor value)
        {
            return H.Equals(value.H) &&
                   L.Equals(value.L) &&
                   S.Equals(value.S);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return H.GetHashCode() ^
                   S.GetHashCode() ^
                   L.GetHashCode();
        }

        /// <inheritdoc/>
        public override readonly string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "H:{0:N1}° S:{1:N1} L:{2:N1}",
                H, 100 * S, 100 * L);
        }

        /// <summary>
        /// Parses a string into an <see cref="HslColor"/>.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The <see cref="HslColor"/> represented by the string.</returns>
        /// <remarks>
        /// The input string should be in the format "hue,saturation,lightness", where hue is in degrees
        /// (optionally followed by the '°' symbol), and saturation and lightness are decimal values.
        /// </remarks>
        public static HslColor Parse(string s)
        {
            var hsl = s.Split(',');
            var hue = float.Parse(hsl[0].TrimEnd('°'), CultureInfo.InvariantCulture.NumberFormat);
            var sat = float.Parse(hsl[1], CultureInfo.InvariantCulture.NumberFormat);
            var lig = float.Parse(hsl[2], CultureInfo.InvariantCulture.NumberFormat);

            return new HslColor(hue, sat, lig);
        }


        /// <summary>
        /// Determines whether two <see cref="HslColor"/> values are equal.
        /// </summary>
        /// <param name="x">The first <see cref="HslColor"/> to compare.</param>
        /// <param name="y">The second <see cref="HslColor"/> to compare.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="HslColor"/> values are equal; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(HslColor x, HslColor y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Determines whether two <see cref="HslColor"/> values are not equal.
        /// </summary>
        /// <param name="x">The first <see cref="HslColor"/> to compare.</param>
        /// <param name="y">The second <see cref="HslColor"/> to compare.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="HslColor"/> values are not equal; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(HslColor x, HslColor y)
        {
            return !x.Equals(y);
        }

        /// <summary>
        /// Adds two <see cref="HslColor"/> values together.
        /// </summary>
        /// <param name="a">The first <see cref="HslColor"/> to add.</param>
        /// <param name="b">The second <see cref="HslColor"/> to add.</param>
        /// <returns>
        /// A new <see cref="HslColor"/> value where the hue, saturation, and light component values are the sum of
        /// the components of the two input colors.
        /// </returns>
        public static HslColor operator +(HslColor a, HslColor b)
        {
            return new HslColor(
                a._h + b._h,
                a._s + b._s,
                a._l + b._l
            );
        }

        /// <summary>
        /// Subtracts one <see cref="HslColor"/> value from another.
        /// </summary>
        /// <param name="a">The <see cref="HslColor"/> to subtract from.</param>
        /// <param name="b">The <see cref="HslColor"/> to subtract.</param>
        /// <returns>
        /// A new <see cref="HslColor"/> value where the hue, saturation, and light component values are the difference
        /// of the components of the two input colors.
        /// </returns>
        public static HslColor operator -(HslColor a, HslColor b)
        {
            return new HslColor(
                a._h - b._h,
                a._s - b._s,
                a._l - b._l
            );
        }

        /// <summary>
        /// Linearly interpolates between two <see cref="HslColor"/> values.
        /// </summary>
        /// <param name="c1">The first <see cref="HslColor"/>.</param>
        /// <param name="c2">The second <see cref="HslColor"/>.</param>
        /// <param name="t">The interpolation factor. A value of 0 returns <paramref name="c1"/>, a value of 1 returns <paramref name="c2"/>.</param>
        /// <returns>The interpolated <see cref="HslColor"/>.</returns>
        public static HslColor Lerp(HslColor c1, HslColor c2, float t)
        {
            // loop around if c2.H < c1.HF
            var h2 = c2.H >= c1.H ? c2.H : c2.H + 360;
            return new HslColor(
                c1.H + t * (h2 - c1.H),
                c1.S + t * (c2.S - c1.S),
                c1.L + t * (c2.L - c1.L));
        }

        /// <summary>
        /// Convers a <see cref="HslColor"/> value to a <see cref="Microsoft.Xna.Framework.Color"/> value.
        /// </summary>
        /// <param name="hsl">The <see cref="HslColor"/> value to convert.</param>
        /// <returns>
        /// A <see cref="Microsoft.Xna.Framework.Color"/> value representing the RGB equivalent of the specified
        /// <see cref="HslColor"/> value.
        /// </returns>
        public static Color ToRgb(HslColor hsl)
        {
            float h = hsl._h;
            float s = hsl._s;
            float l = hsl._l;

            if (s < MathExtended.MachineEpsilon)
            {
                return new Color(l, l, l);
            }

            if (l <= MathExtended.MachineEpsilon)
            {
                return Color.Black;
            }

            h /= 360.0f;

            float max = l < 0.5f ?
                        l * (1 + s) :
                        l + s - l * s;

            float min = 2.0f * l - max;

            float r = RgbFromHue(min, max, h + 0.3333333f);
            float g = RgbFromHue(min, max, h);
            float b = RgbFromHue(min, max, h - 0.3333333f);

            return new Color(r, g, b);
        }

        private static float RgbFromHue(float min, float max, float hue)
        {
            hue = (hue + 1.0f) % 1.0f;

            if (hue * 6.0f < 1.0f)
            {
                return min + (max - min) * 6.0f * hue;
            }

            if (hue * 2.0f < 1.0f)
            {
                return max;
            }

            if (hue * 3.0f < 2.0f)
            {
                return min + (max - min) * (2.0f / 3.0f - hue) * 6.0f;
            }

            return min;
        }

        /// <summary>
        /// Converts an RGB color to an HSL color.
        /// </summary>
        /// <param name="color">The RGB color to convert.</param>
        /// <returns>The equivalent HSL color.</returns>
        public static HslColor FromRgb(Color color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = MathF.Max(r, MathF.Max(g, b));
            float min = MathF.Min(r, MathF.Min(g, b));
            float delta = max - min;

            float h = 0.0f;
            float s = 0.0f;
            float l = (max + min) * 0.5f;

            if (MathF.Abs(delta) < float.Epsilon)
            {
                return new HslColor(h, s, l);
            }

            if (MathF.Abs(r - max) < float.Epsilon)
            {
                h = (g - b) / delta;
            }
            else if (MathF.Abs(g - max) < float.Epsilon)
            {
                h = (b - r) / delta + 2.0f;
            }
            else if (MathF.Abs(b - max) < float.Epsilon)
            {
                h = (r - g) / delta + 4.0f;
            }

            h *= 60.0f;

            h = NormalizeHue(h);

            if (l <= 0.5f)
            {
                s = delta / (max + min);
            }
            else
            {
                s = delta / (2.0f - max - min);
            }

            return new HslColor(h, s, l);
        }
    }
}
