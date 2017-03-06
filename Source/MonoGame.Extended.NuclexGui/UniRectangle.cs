using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui
{
    /// <summary>Two-dimensional rectangle of combined fraction and offset coordinates</summary>
    public struct UniRectangle
    {
        /// <summary>An empty unified rectangle</summary>
        public static readonly UniRectangle Empty = new UniRectangle();

        /// <summary>X coordinate of the rectangle's left border</summary>
        public UniScalar Left
        {
            get { return Location.X; }
            set { Location.X = value; }
        }

        /// <summary>Y coordinate of the rectangle's upper border</summary>
        public UniScalar Top
        {
            get { return Location.Y; }
            set { Location.Y = value; }
        }

        /// <summary>X coordinate of the rectangle's right border</summary>
        public UniScalar Right
        {
            get { return Location.X + Size.X; }
            set { Size.X = value - Location.X; }
        }

        /// <summary>Y coordinate of the rectangle's lower border</summary>
        public UniScalar Bottom
        {
            get { return Location.Y + Size.Y; }
            set { Size.Y = value - Location.Y; }
        }

        /// <summary>Point consisting of the lesser coordinates of the rectangle</summary>
        public UniVector Min
        {
            get { return Location; }
            set
            {
                // In short: this.Size += this.Location - value;
                // Done for performance reasons
                Size.X.Fraction += Location.X.Fraction - value.X.Fraction;
                Size.X.Offset += Location.X.Offset - value.X.Offset;
                Size.Y.Fraction += Location.Y.Fraction - value.Y.Fraction;
                Size.Y.Offset += Location.Y.Offset - value.Y.Offset;

                Location = value;
            }
        }

        /// <summary>Point consisting of the greater coordinates of the rectangle</summary>
        public UniVector Max
        {
            get { return Location + Size; }
            set
            {
                // In short: this.Size = value - this.Location;
                // Done for performance reasons
                Size.X.Fraction = value.X.Fraction - Location.X.Fraction;
                Size.X.Offset = value.X.Offset - Location.X.Offset;
                Size.Y.Fraction = value.Y.Fraction - Location.X.Fraction;
                Size.Y.Offset = value.Y.Offset - Location.Y.Offset;
            }
        }

        /// <summary>The location of the rectangle's upper left corner</summary>
        public UniVector Location;

        /// <summary>The size of the rectangle</summary>
        public UniVector Size;

        /// <summary>Initializes a new rectangle from a location and a size</summary>
        /// <param name="location">Location of the rectangle's upper left corner</param>
        /// <param name="size">Size of the area covered by the rectangle</param>
        public UniRectangle(UniVector location, UniVector size)
        {
            Location = location;
            Size = size;
        }

        /// <summary>Initializes a new rectangle from the provided individual coordinates</summary>
        /// <param name="x">X coordinate of the rectangle's left border</param>
        /// <param name="y">Y coordinate of the rectangle's upper border</param>
        /// <param name="width">Width of the area covered by the rectangle</param>
        /// <param name="height">Height of the area covered by the rectangle</param>
        public UniRectangle(UniScalar x, UniScalar y, UniScalar width, UniScalar height)
        {
            Location = new UniVector(x, y);
            Size = new UniVector(width, height);
        }

        /// <summary>Converts the rectangle into pure offset coordinates</summary>
        /// <param name="containerSize">Dimensions of the container the fractional part of the rectangle count for</param>
        /// <returns>A rectangle with the pure offset coordinates of the rectangle</returns>
        public RectangleF ToOffset(Vector2 containerSize)
        {
            return ToOffset(containerSize.X, containerSize.Y);
        }

        /// <summary>Converts the rectangle into pure offset coordinates</summary>
        /// <param name="containerWidth">Width of the container the fractional part of the rectangle counts for</param>
        /// <param name="containerHeight">Height of the container the fractional part of the rectangle counts for</param>
        /// <returns>A rectangle with the pure offset coordinates of the rectangle</returns>
        public RectangleF ToOffset(float containerWidth, float containerHeight)
        {
            var leftOffset = Left.Fraction*containerWidth + Left.Offset;
            var topOffset = Top.Fraction*containerHeight + Top.Offset;

            return new RectangleF(
                leftOffset,
                topOffset,
                Right.Fraction*containerWidth + Right.Offset - leftOffset,
                Bottom.Fraction*containerHeight + Bottom.Offset - topOffset
            );
        }

        /// <summary>Checks two rectangles for inequality</summary>
        /// <param name="first">First rectangle to be compared</param>
        /// <param name="second">Second rectangle to be compared</param>
        /// <returns>True if the instances differ or exactly one reference is set to null</returns>
        public static bool operator !=(UniRectangle first, UniRectangle second)
        {
            return !(first == second);
        }

        /// <summary>Checks two rectangles for equality</summary>
        /// <param name="first">First rectangle to be compared</param>
        /// <param name="second">Second rectangle to be compared</param>
        /// <returns>True if both instances are equal or both references are null</returns>
        public static bool operator ==(UniRectangle first, UniRectangle second)
        {
            // For a struct, neither 'first' nor 'second' can be null
            return first.Equals(second);
        }

        /// <summary>Checks whether another instance is equal to this instance</summary>
        /// <param name="other">Other instance to compare to this instance</param>
        /// <returns>True if the other instance is equal to this instance</returns>
        public override bool Equals(object other)
        {
            if (!(other is UniRectangle))
                return false;

            return Equals((UniRectangle) other);
        }

        /// <summary>Checks whether another instance is equal to this instance</summary>
        /// <param name="other">Other instance to compare to this instance</param>
        /// <returns>True if the other instance is equal to this instance</returns>
        public bool Equals(UniRectangle other)
        {
            // For a struct, 'other' cannot be null
            return (Location == other.Location) && (Size == other.Size);
        }

        /// <summary>Obtains a hash code of this instance</summary>
        /// <returns>The hash code of the instance</returns>
        public override int GetHashCode()
        {
            return Location.GetHashCode() ^ Size.GetHashCode();
        }

        /// <summary>
        ///     Returns a human-readable string representation for the unified rectangle
        /// </summary>
        /// <returns>The human-readable string representation of the unified rectangle</returns>
        public override string ToString()
        {
            return string.Format(
                "{{Location:{0}, Size:{1}}}",
                Location,
                Size
            );
        }

        /// <summary>
        ///     Moves unified rectangle by the absolute values
        /// </summary>
        public void AbsoluteOffset(float x, float y)
        {
            Location.X.Offset += x;
            Location.Y.Offset += y;
        }

        public void FractionalOffset(float x, float y)
        {
            Location.X.Fraction += x;
            Location.Y.Fraction += y;
        }
    }
}