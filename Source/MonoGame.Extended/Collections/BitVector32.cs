using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace MonoGame.Extended.Collections
{
    /// <summary>
    ///     Defines a bit vector with easy integer or boolean access to a 32 bit storage.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="BitVector32" /> is more efficient than <see cref="BitArray" /> for boolean values and small integers
    ///         that are used internally. A <see cref="BitArray" /> can grow indefinitely as needed, but it has the memory and
    ///         performance overhead that a class instance requires. In contrast, a <see cref="BitVector32" /> uses only 32
    ///         bits.
    ///     </para>
    ///     <para>
    ///         A <see cref="BitVector32" /> structure can be set up to contain either sections for small integers or bit
    ///         flags for booleans, but not both. A <see cref="Section" /> is a window into the <see cref="BitVector32" /> and
    ///         is composed of the smallest number of consecutive bits that can contain the maximum value specified in
    ///         <see cref="CreateSection(short,Section)" />. For example, a section with a maximum value of 1 is composed of
    ///         only one bit, whereas a section with a maximum value of 5 is composed of three bits. You can create a
    ///         <see cref="Section" /> with a maximum value of 1 to serve as a <see cref="bool" />, thereby allowing you to
    ///         store integers and booleans in the same <see cref="BitVector32" />.
    ///     </para>
    ///     <para>
    ///         Some members can be used for a <see cref="BitVector32" /> that is set up as sections, while other members can
    ///         be used for one that is set up as bit flags. For example, the <see cref="Item(Section)" /> property is the
    ///         indexer for a <see cref="BitVector32" /> that is set up as sections, and the <see cref="Item(uint)" /> property
    ///         is the indexer for a <see cref="BitVector32" /> that is set up as bit flags. <see cref="CreateMask(int)" />
    ///         creates a series of masks that can be used to access individual bits in a <see cref="BitVector32" /> that is
    ///         set up as bit flags.
    ///     </para>
    /// </remarks>
    public struct BitVector32 : IEquatable<BitVector32>
    {
        private uint _data;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BitVector32" /> structure with the specified internal data.
        /// </summary>
        /// <param name="data">An integer representing the data of the new <see cref="BitVector32" />.</param>
        /// <remarks>
        ///     <para>This constructor is an O(1) operation.</para>
        /// </remarks>
        public BitVector32(uint data)
        {
            _data = data;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BitVector32" /> structure with the specified internal data.
        /// </summary>
        /// <param name="data">An integer representing the data of the new <see cref="BitVector32" />.</param>
        /// <remarks>
        ///     <para>This constructor is an O(1) operation.</para>
        /// </remarks>
        public BitVector32(int data)
        {
            _data = (uint) data;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BitVector32" /> structure containing the data represented in an
        ///     existing <see cref="BitVector32" /> structure.
        /// </summary>
        /// <param name="value">A <see cref="BitVector32" /> structure that contains the data to copy.</param>
        /// <remarks>
        ///     <para>This constructor is an O(1) operation.</para>
        /// </remarks>
        public BitVector32(BitVector32 value)
        {
            _data = value._data;
        }


        /// <summary>
        ///     Gets or sets the state of the bit flag indicated by the specified mask.
        /// </summary>
        /// <param name="bitMask">A bit mask that indicates the bit flag to get or set.</param>
        /// <returns><c>true</c> if the specified bit flag is on (1); otherwise, <c>false</c>.</returns>
        /// <remarks>
        ///     <para>
        ///         The <see cref="Item(Section)" /> property is the indexer for a <see cref="BitVector32" /> that is set up as
        ///         sections, and the <see cref="Item(uint)" /> property is the indexer for a <see cref="BitVector32" /> that is
        ///         set up as bit flags.
        ///     </para>
        ///     <para>
        ///         Using this property on a <see cref="BitVector32" /> that is set up as sections might cause unexpected
        ///         results.
        ///     </para>
        ///     <para>Retrieving the value of this property is an O(1) operation; setting the property is also an O(1) operation.</para>
        /// </remarks>
        public bool this[uint bitMask]
        {
            get { return (_data & bitMask) == bitMask; }
            set
            {
                if (value)
                    _data |= bitMask;
                else
                    _data &= ~bitMask;
            }
        }

        /// <summary>
        ///     Gets or sets the value stored in the specified <see cref="Section" />.
        /// </summary>
        /// <param name="section">A <see cref="Section" /> that contains the value to get or set.</param>
        /// <returns>The value stored in the specified <see cref="Section" />.</returns>
        /// <remarks>
        ///     <para>
        ///         The <see cref="Item(Section)" /> property is the indexer for a <see cref="BitVector32" /> that is set up as
        ///         sections, and the <see cref="Item(uint)" /> property is the indexer for a <see cref="BitVector32" /> that is
        ///         set up as bit flags.
        ///     </para>
        ///     <para>
        ///         A <see cref="Section" /> is a window into the <see cref="BitVector32" /> and is composed of the smallest
        ///         number of consecutive bits that can contain the maximum value specified in <see cref="CreateSection(short)" />.
        ///         For example, a section with a maximum value of 1 is composed of only one bit, whereas a section with a maximum
        ///         value of 5 is composed of three bits. You can create a <see cref="Section" /> with a maximum value of 1 to
        ///         serve as a <see cref="bool" />, thereby allowing you to store integers and booleans in the same
        ///         <see cref="BitVector32" />.
        ///     </para>
        ///     <para>Retrieving the value of this property is an O(1) operation; setting the property is also an O(1) operation.</para>
        /// </remarks>
        public int this[Section section]
        {
            get { return (int) ((_data & (uint) (section.Mask << section.Offset)) >> section.Offset); }
            set
            {
                Debug.Assert((value & section.Mask) == value, "Value out of bounds on BitVector32 Section set.");
                value <<= section.Offset;
                var offsetMask = (0xFFFF & section.Mask) << section.Offset;
                _data = (_data & ~(uint) offsetMask) | ((uint) value & (uint) offsetMask);
            }
        }

        /// <summary>
        ///     Converts a <see cref="BitVector32" /> to an <see cref="uint" />.
        /// </summary>
        /// <param name="bitVector">The <see cref="BitVector32" /> to convert into a <see cref="uint" />.</param>
        public static implicit operator uint(BitVector32 bitVector)
        {
            return bitVector._data;
        }

        /// <summary>
        ///     Converts a <see cref="uint" /> to a <see cref="BitVector32" />.
        /// </summary>
        /// <param name="data">The <see cref="uint" /> to convert into a <see cref="BitVector32" />.</param>
        public static implicit operator BitVector32(uint data)
        {
            return new BitVector32(data);
        }

        /// <summary>
        ///     Converts a <see cref="BitVector32" /> to an <see cref="uint" />.
        /// </summary>
        /// <param name="bitVector">The <see cref="BitVector32" /> to convert into a <see cref="uint" />.</param>
        public static implicit operator int(BitVector32 bitVector)
        {
            return (int) bitVector._data;
        }

        /// <summary>
        ///     Converts a <see cref="int" /> to a <see cref="BitVector32" />.
        /// </summary>
        /// <param name="data">The <see cref="int" /> to convert into a <see cref="BitVector32" />.</param>
        public static implicit operator BitVector32(int data)
        {
            return new BitVector32(data);
        }

        private static short CountBitsSet(short mask)
        {
            // bits are always right aligned with no holes, e.g., always 00000111 never 000100011
            short value = 0;
            while ((mask & 0x1) != 0)
            {
                value++;
                mask >>= 1;
            }
            return value;
        }

        /// <summary>
        ///     Creates the first mask in a series of masks that can be used to retrieve individual bits in a
        ///     <see cref="BitVector32" /> that is set up as bit flags.
        /// </summary>
        /// <returns>A mask that isolates the first bit flag in the <see cref="BitVector32" />.</returns>
        /// <remarks>
        ///     <para>
        ///         Use <see cref="CreateMask()" /> to create the first mask in a series and <see cref="CreateMask(int)" /> for
        ///         all subsequent masks.
        ///     </para>
        ///     <para>Multiple masks can be created to refer to the same bit flag.</para>
        ///     <para>
        ///         The resulting mask isolates only one bit flag in the <see cref="BitVector32" />. You can combine masks using
        ///         the bitwise OR operation to create a mask that isolates multiple bit flags in the <see cref="BitVector32" />.
        ///     </para>
        ///     <para>Using a mask on a <see cref="BitVector32" /> that is set up as sections might cause unexpected results.</para>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public static uint CreateMask()
        {
            return CreateMask(0);
        }

        /// <summary>
        ///     Creates an additional mask following the specified mask in a series of masks that can be used to retrieve
        ///     individual bits in a <see cref="BitVector32" /> that is set up as bit flags.
        /// </summary>
        /// <param name="previous">The mask that indicates the previous bit flag.</param>
        /// <returns>
        ///     A mask that isolates the bit flag following the one that <paramref name="previous" /> points to in
        ///     <see cref="BitVector32" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="previous" /> indicates the last bit flag in the
        ///     <see cref="BitVector32" />.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         Use <see cref="CreateMask()" /> to create the first mask in a series and <see cref="CreateMask(int)" /> for
        ///         all subsequent masks.
        ///     </para>
        ///     <para>Multiple masks can be created to refer to the same bit flag.</para>
        ///     <para>
        ///         The resulting mask isolates only one bit flag in the <see cref="BitVector32" />. You can combine masks using
        ///         the bitwise OR operation to create a mask that isolates multiple bit flags in the <see cref="BitVector32" />.
        ///     </para>
        ///     <para>Using a mask on a <see cref="BitVector32" /> that is set up as sections might cause unexpected results.</para>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public static uint CreateMask(uint previous)
        {
            if (previous == 0)
                return 1;

            if (previous == 0x80000000)
                throw new InvalidOperationException("Can't create a new mask; the bit vector is full.");

            return previous << 1;
        }

        private static short CreateMaskFromHighValue(short highValue)
        {
            short required = 16;
            while ((highValue & 0x8000) == 0)
            {
                required--;
                highValue <<= 1;
            }

            ushort value = 0;
            while (required > 0)
            {
                required--;
                value <<= 1;
                value |= 0x1;
            }

            return unchecked((short) value);
        }

        /// <summary>
        ///     Creates the first <see cref="Section" /> in a series of sections that contain small integers.
        /// </summary>
        /// <param name="maxValue">A 16-bit signed integer that specifies the maximum value for the new <see cref="Section" />.</param>
        /// <returns>A <see cref="Section" /> that can hold a number from zero to <paramref name="maxValue" />.</returns>
        /// <exception cref="ArgumentException"><paramref name="maxValue" /> is less than 1.</exception>
        /// <remarks>
        ///     <para>
        ///         A <see cref="Section" /> is a window into the <see cref="BitVector32" /> and is composed of the smallest
        ///         number of consecutive bits that can contain the maximum value specified in
        ///         <see cref="CreateSection(short, Section)" />.
        ///         For example, a section with a maximum value of 1 is composed of only one bit, whereas a section with a maximum
        ///         value of 5 is composed of three bits. You can create a <see cref="Section" /> with a maximum value of 1 to
        ///         serve as a <see cref="bool" />, thereby allowing you to store integers and booleans in the same
        ///         <see cref="BitVector32" />.
        ///     </para>
        ///     <para>
        ///         If sections already exist in the <see cref="BitVector32" />, those sections are still accessible; however,
        ///         overlapping sections might cause unexpected results.
        ///     </para>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public static Section CreateSection(short maxValue)
        {
            return CreateSectionHelper(maxValue, 0, 0);
        }

        /// <summary>
        ///     Creates a new <see cref="Section" /> following the specified <see cref="Section" /> in a series of sections that
        ///     contain small integers.
        /// </summary>
        /// <param name="maxValue">A 16-bit signed integer that specifies the maximum value for the new <see cref="Section" />.</param>
        /// <param name="previous">The previous <see cref="Section" /> in the <see cref="BitVector32" />.</param>
        /// <returns>A <see cref="Section" /> that can hold a number from zero to <paramref name="maxValue" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <param name="maxValue"> is less than 1.</param>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="previous" /> includes the final bit in the
        ///     <see cref="BitVector32" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <param name="maxValue"></param>
        ///     is greater than the highest value that can be represented by the number of bits after <paramref name="previous" />.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         A <see cref="Section" /> is a window into the <see cref="BitVector32" /> and is composed of the smallest
        ///         number of consecutive bits that can contain the maximum value specified in
        ///         <see cref="CreateSection(short, Section)" />. For example, a section with a maximum value of 1 is composed of
        ///         only one bit, whereas a section with a maximum value of 5 is composed of three bits. You can create a
        ///         BitVector32.Section with a maximum value of 1 to serve as a Boolean, thereby allowing you to store integers and
        ///         Booleans in the same BitVector32.
        ///     </para>
        ///     <para>
        ///         If sections already exist after previous in the <see cref="BitVector32" />, those sections are still
        ///         accessible; however, overlapping sections might cause unexpected results.
        ///     </para>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public static Section CreateSection(short maxValue, Section previous)
        {
            return CreateSectionHelper(maxValue, previous.Mask, previous.Offset);
        }

        private static Section CreateSectionHelper(short maxValue, short priorMask, short priorOffset)
        {
            if (maxValue < 1)
                throw new ArgumentException("The max value was less than 0.");
#if DEBUG
            int maskCheck = CreateMaskFromHighValue(maxValue);
            var offsetCheck = priorOffset + CountBitsSet(priorMask);
            Debug.Assert((maskCheck <= short.MaxValue) && (offsetCheck < 32), "Overflow on BitVector32");
#endif
            var offset = (short) (priorOffset + CountBitsSet(priorMask));
            if (offset >= 32)
                throw new InvalidOperationException("Can't create a new mask; the bit vector is full.");
            return new Section(CreateMaskFromHighValue(maxValue), offset);
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the <see cref="BitVector32" />.
        /// </summary>
        /// <param name="obj">The object to compare with the <see cref="BitVector32" />.</param>
        /// <returns><c>true</c> if the specified object is equal to the <see cref="BitVector32" />; otherwise, <c>false</c>.</returns>
        /// <remarks>
        ///     <para>
        ///         The object <paramref name="obj" /> is considered equal to the <see cref="BitVector32" /> if the type of
        ///         <paramref name="obj" /> is compatible with the <see cref="BitVector32" /> type and if the value of
        ///         <paramref name="obj" /> is equal to the value of <see cref="BitVector32" />.
        ///     </para>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is BitVector32 && Equals((BitVector32) obj);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="BitVector32" /> is equal to the <see cref="BitVector32" />.
        /// </summary>
        /// <param name="other">The <see cref="BitVector32" /> to compare with the <see cref="BitVector32" />.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="other" /> is the same as the <see cref="BitVector32" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public bool Equals(BitVector32 other)
        {
            return _data == other._data;
        }

        /// <summary>
        ///     Determines whether two specified <see cref="BitVector32" /> values are equal.
        /// </summary>
        /// <param name="a">The first <see cref="BitVector32" />.</param>
        /// <param name="b">The second <see cref="BitVector32" /></param>
        /// <returns>
        ///     <c>true</c> if <paramref name="a" /> and <paramref name="b" /> represent the same <see cref="BitVector32" />,
        ///     otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public static bool operator ==(BitVector32 a, BitVector32 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        ///     Determines whether two <see cref="BitVector32" /> values have different values.
        /// </summary>
        /// <param name="a">The first <see cref="BitVector32" />.</param>
        /// <param name="b">The second <see cref="BitVector32" /></param>
        /// <returns>
        ///     <c>true</c> if <paramref name="a" /> and <paramref name="b" /> do not represent the same <see cref="BitVector32" />
        ///     ,
        ///     otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public static bool operator !=(BitVector32 a, BitVector32 b)
        {
            return !(a == b);
        }

        /// <summary>
        ///     Serves as a hash function for the <see cref="BitVector32" />.
        /// </summary>
        /// <returns>A hash code for the <see cref="BitVector32" />.</returns>
        /// <remarks>
        ///     <para>This method overrides <see cref="object.GetHashCode()" />.</para>
        ///     <para>
        ///         The hash code of a <see cref="BitVector32" /> is based on the value of <see cref="BitVector32" />. Two
        ///         instances of <see cref="BitVector32" /> with the same values also generate the same hash code.
        ///     </para>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///     Returns a string that represents the specified <see cref="BitVector32" />.
        /// </summary>
        /// <param name="value">The <see cref="BitVector32" /> to represent.</param>
        /// <returns>A string that represents the specified <see cref="BitVector32" />.</returns>
        /// <remarks>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public static string ToString(BitVector32 value)
        {
            var stringBuilder = new StringBuilder(12 + 32 + 1);
            stringBuilder.Append("BitVector32{");
            var data = (int) value._data;
            for (var i = 0; i < 32; i++)
            {
                stringBuilder.Append((data & 0x80000000) != 0 ? "1" : "0");
                data <<= 1;
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Returns a string that represents the current <see cref="BitVector32" />.
        /// </summary>
        /// <returns>A string that represents the current <see cref="BitVector32" />.</returns>
        /// <remarks>
        ///     <para>This method overrides <see cref="object.ToString()" />.</para>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public override string ToString()
        {
            return ToString(this);
        }

        /// <summary>
        ///     Represents a section of a <see cref="BitVector32" /> that can contain an integer number.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Use <see cref="CreateSection(short)" /> to define a new section. A <see cref="Section" /> is a window into the
        ///         <see cref="BitVector32" /> and is composed of the smallest number of consecutive bits that can contain the
        ///         maximum value specified in <see cref="CreateSection(short, Section)" />. For example, a section with a maximum
        ///         value of 1 is composed of only one bit, whereas a section with a maximum value of 5 is composed of three bits.
        ///         You can create a <see cref="Section" /> with a maximum value of 1 to serve as a <see cref="bool" />, thereby
        ///         allowing you to store integers and booleans in the same <see cref="BitVector32" />.
        ///     </para>
        /// </remarks>
        public struct Section : IEquatable<Section>
        {
            /// <summary>
            ///     The mask that isolates the <see cref="Section" /> within the <see cref="BitVector32" />.
            /// </summary>
            public readonly short Mask;

            /// <summary>
            ///     Gets the offset of the <see cref="Section" /> from the start of the <see cref="BitVector32" />.
            /// </summary>
            public readonly short Offset;

            internal Section(short mask, short offset)
            {
                Mask = mask;
                Offset = offset;
            }

            /// <summary>
            ///     Determines whether the specified object is equal to the <see cref="Section" />.
            /// </summary>
            /// <param name="obj">The object to compare with the <see cref="Section" />.</param>
            /// <returns><c>true</c> if <paramref name="obj" /> is the same as the <see cref="Section" />; otherwise, <c>false</c>.</returns>
            /// <remarks>
            ///     <para>This method overrides <see cref="object.ToString()" />.</para>
            ///     <para>
            ///         Two <see cref="Section" /> values are considered equal if both sections are of the same length and are in the
            ///         same location within a <see cref="BitVector32" />.
            ///     </para>
            ///     <para>This method is an O(1) operation.</para>
            /// </remarks>
            public override bool Equals(object obj)
            {
                if (obj is Section)
                    return Equals((Section) obj);
                return false;
            }

            /// <summary>
            ///     Determines whether the specified <see cref="Section" /> is equal to the <see cref="Section" />.
            /// </summary>
            /// <param name="other">The <see cref="Section" /> to compare with the <see cref="Section" />.</param>
            /// <returns><c>true</c> if <paramref name="other" /> is the same as the <see cref="Section" />; otherwise, <c>false</c>.</returns>
            /// <remarks>
            ///     <para>
            ///         Two <see cref="Section" /> values are considered equal if both sections are of the same length and are in the
            ///         same location within a <see cref="BitVector32" />.
            ///     </para>
            ///     <para>This method is an O(1) operation.</para>
            /// </remarks>
            public bool Equals(Section other)
            {
                return (other.Mask == Mask) && (other.Offset == Offset);
            }

            /// <summary>
            ///     Determines whether two specified <see cref="Section" /> values are equal.
            /// </summary>
            /// <param name="a">The first <see cref="Section" />.</param>
            /// <param name="b">The second <see cref="Section" /></param>
            /// <returns>
            ///     <c>true</c> if <paramref name="a" /> and <paramref name="b" /> represent the same <see cref="Section" />,
            ///     otherwise, <c>false</c>.
            /// </returns>
            /// <remarks>
            ///     <para>
            ///         Two <see cref="Section" /> values are considered equal if both sections are of the same length and are in the
            ///         same location within a <see cref="BitVector32" />.
            ///     </para>
            ///     <para>This method is an O(1) operation.</para>
            /// </remarks>
            public static bool operator ==(Section a, Section b)
            {
                return (a.Mask == b.Mask) && (a.Offset == b.Offset);
            }

            /// <summary>
            ///     Determines whether two <see cref="Section" /> values have different values.
            /// </summary>
            /// <param name="a">The first <see cref="Section" />.</param>
            /// <param name="b">The second <see cref="Section" /></param>
            /// <returns>
            ///     <c>true</c> if <paramref name="a" /> and <paramref name="b" /> do not represent the same <see cref="Section" />,
            ///     otherwise, <c>false</c>.
            /// </returns>
            /// <remarks>
            ///     <para>
            ///         Two <see cref="Section" /> values are considered equal if both sections are of the same length and are in the
            ///         same location within a <see cref="BitVector32" />.
            ///     </para>
            ///     <para>This method is an O(1) operation.</para>
            /// </remarks>
            public static bool operator !=(Section a, Section b)
            {
                return !(a == b);
            }

            /// <summary>
            ///     Serves as a hash function for the <see cref="Section" />, suitable for hashing algorithms and data structures, such
            ///     as a hash table.
            /// </summary>
            /// <returns>A hash code for the current <see cref="Section" />.</returns>
            /// <remarks>
            ///     <para>This method overrides <see cref="object.GetHashCode()" />.</para>
            ///     <para>
            ///         This method generates the same hash code for two objects that are equal according to the
            ///         <see cref="Equals(Section)" /> method.
            ///     </para>
            ///     <para>This method is an O(1) operation.</para>
            /// </remarks>
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            /// <summary>
            ///     Returns a string that represents the specified <see cref="Section" />.
            /// </summary>
            /// <param name="value">The <see cref="Section" /> to represent.</param>
            /// <returns>A string that represents the specified <see cref="Section" />.</returns>
            /// <remarks>
            ///     <para>This method is an O(1) operation.</para>
            /// </remarks>
            public static string ToString(Section value)
            {
                return "Section{0x" + Convert.ToString(value.Mask, 16) + ", 0x" + Convert.ToString(value.Offset, 16) +
                       "}";
            }

            /// <summary>
            ///     Returns a string that represents the <see cref="Section" />.
            /// </summary>
            /// <returns>A string that represents the <see cref="Section" />.</returns>
            /// <remarks>
            ///     <para>This method overrides <see cref="object.ToString()" />.</para>
            ///     <para>This method is an O(1) operation.</para>
            /// </remarks>
            public override string ToString()
            {
                return ToString(this);
            }
        }
    }
}