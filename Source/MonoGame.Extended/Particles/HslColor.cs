using System;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles
{
    /// <summary>
    /// An immutable data structure representing a 24bit color composed of separate hue, saturation and lightness channels.
    /// </summary>
    //[Serializable]
    public struct HslColor : IEquatable<HslColor>
    {
        /// <summary>
        /// Gets the value of the hue channel in degrees.
        /// </summary>
        public float H { get; }
        
        /// <summary>
        /// Gets the value of the saturation channel.
        /// </summary>
        public float S { get; }

        /// <summary>
        /// Gets the value of the lightness channel.
        /// </summary>
        public float L { get; }

        private static float NormalizeHue(float h)
        {
            if (h < 0) return h + 360 * ((int)(h / 360) + 1);
            return h % 360;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HslColor"/> structure.
        /// </summary>
        /// <param name="h">The value of the hue channel.</param>
        /// <param name="s">The value of the saturation channel.</param>
        /// <param name="l">The value of the lightness channel.</param>
        public HslColor(float h, float s, float l) : this()
        {
            // normalize the hue
            H = NormalizeHue(h);
            S = MathHelper.Clamp(s, 0f, 1f);
            L = MathHelper.Clamp(l, 0f, 1f);
        }

        /// <summary>
        /// Copies the individual channels of the color to the specified memory location.
        /// </summary>
        /// <param name="destination">The memory location to copy the axis to.</param>
        public void CopyTo(out HslColor destination)
        {
            destination = new HslColor(H, S, L);
        }

        /// <summary>
        /// Destructures the color, exposing the individual channels.
        /// </summary>
        public void Destructure(out float h, out float s, out float l)
        {
            h = H;
            s = S;
            l = L;
        }

        /// <summary>
        /// Exposes the individual channels of the color to the specified matching function.
        /// </summary>
        /// <param name="callback">The function which matches the individual channels of the color.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="callback"/> parameter is <c>null</c>.
        /// </exception>
        public void Match(Action<float, float, float> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(H, S, L);
        }

        /// <summary>
        /// Exposes the individual channels of the color to the specified mapping function and returns the
        /// result;
        /// </summary>
        /// <typeparam name="T">The type being mapped to.</typeparam>
        /// <param name="map">
        /// A function which maps the color channels to an instance of <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// The result of the <paramref name="map"/> function when passed the individual X and Y components.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="map"/> parameter is <c>null</c>.
        /// </exception>
        public T Map<T>(Func<float, float, float, T> map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return map(H, S, L);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is HslColor)
                return Equals((HslColor)obj);

            return base.Equals(obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="HslColor"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="HslColor"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="HslColor"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(HslColor value)
        {
            return H.Equals(value.H) &&
                   S.Equals(value.S) &&
                   L.Equals(value.L);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return H.GetHashCode() ^
                   S.GetHashCode() ^
                   L.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}°,{1:P0},{2:P0}",
                H.ToString("F1"), (100 * S).ToString("F1"), (100 * L).ToString("F1"));
        }

        public static HslColor Parse(string s)
        {
            var hsl = s.Split(',');
            var hue = float.Parse(hsl[0].TrimEnd('°'));
            var sat = float.Parse(hsl[1]);
            var lig = float.Parse(hsl[2]);

            return new HslColor(hue, sat, lig);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="x">The lvalue.</param>
        /// <param name="y">The rvalue.</param>
        /// <returns>
        ///     <c>true</c> if the lvalue <see cref="HslColor"/> is equal to the rvalue; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(HslColor x, HslColor y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The lvalue.</param>
        /// <param name="y">The rvalue.</param>
        /// <returns>
        ///     <c>true</c> if the lvalue <see cref="HslColor"/> is not equal to the rvalue; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(HslColor x, HslColor y)
        {
            return !x.Equals(y);
        }

        public static HslColor operator -(HslColor a, HslColor b)
        {
            return new HslColor(a.H - b.H, a.S - b.S, a.L - b.L);
        }

        public static HslColor Lerp(HslColor c1, HslColor c2, float t)
        {
            // loop around if c2.H < c1.H
            var h2 = c2.H >= c1.H ? c2.H : c2.H + 360;
            return new HslColor(
                c1.H + t * (h2 - c1.H),
                c1.S + t * (c2.S - c1.S),
                c1.L + t * (c2.L - c2.L));
        }
    }
}