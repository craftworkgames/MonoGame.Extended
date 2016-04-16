using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Graphics
{
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct SpriteColor : IEquatable<SpriteColor>, IEquatable<Color>
    {
        [DataMember]
        public Color TopLeftColor;

        [DataMember]
        public Color TopRightColor;

        [DataMember]
        public Color BottomLeftColor;

        [DataMember]
        public Color BottomRightColor;

        public SpriteColor(Color topLeftColor, Color topRightColor, Color bottomLeftColor, Color bottomRightColor)
        {
            TopLeftColor = topLeftColor;
            TopRightColor = topRightColor;
            BottomLeftColor = bottomLeftColor;
            BottomRightColor = bottomRightColor;
        }

        public SpriteColor(Color topColor, Color bottomColor)
        {
            TopLeftColor = topColor;
            TopRightColor = topColor;
            BottomLeftColor = bottomColor;
            BottomRightColor = bottomColor;
        }

        public SpriteColor(Color color)
        {
            TopLeftColor = color;
            TopRightColor = color;
            BottomLeftColor = color;
            BottomRightColor = color;
        }

        public static implicit operator SpriteColor(Color color)
        {
            return new SpriteColor(color);
        }

        public static bool operator ==(SpriteColor a, SpriteColor b)
        {
            return a.TopLeftColor == b.TopLeftColor && a.TopRightColor == b.TopRightColor && a.BottomLeftColor == b.BottomLeftColor && a.BottomRightColor == b.BottomRightColor;
        }

        public static bool operator !=(SpriteColor a, SpriteColor b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return (obj is SpriteColor && this == (SpriteColor)obj) || (obj is Color && this == (Color)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                var hashCode = TopLeftColor.GetHashCode();
                hashCode = (hashCode * 397) ^ TopRightColor.GetHashCode();
                hashCode = (hashCode * 397) ^ BottomLeftColor.GetHashCode();
                hashCode = (hashCode * 397) ^ BottomRightColor.GetHashCode();
                // ReSharper restore NonReadonlyMemberInGetHashCode
                return hashCode;
            }
        }

        public bool Equals(SpriteColor other)
        {
            return this == other;
        }

        public bool Equals(Color other)
        {
            return this == other;
        }

        internal string DebugDisplayString()
        {
            return string.Concat(TopLeftColor, " ", TopRightColor, " ", BottomLeftColor, " ", BottomRightColor);
        }
    }
}
