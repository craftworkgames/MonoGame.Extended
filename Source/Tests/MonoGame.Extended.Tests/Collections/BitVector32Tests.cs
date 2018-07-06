using System;
using System.Collections.Specialized;
using MonoGame.Extended.Collections;
using Xunit;

namespace MonoGame.Extended.Tests.Collections
{
    public class BitVector32Tests
    {
        [Fact]
        public void Constructors()
        {
            var bitVector = new BitVector32(31);
            Assert.Equal(31, bitVector.Data);
            Assert.True(bitVector[31]);
            Assert.False(bitVector[32]);
            Assert.Equal("BitVector32{00000000000000000000000000011111}", bitVector.ToString());

            var bitVector2 = new BitVector32(bitVector);
            Assert.True(bitVector.Data == bitVector2.Data);
            Assert.Equal(bitVector.GetHashCode(), bitVector2.GetHashCode());

            bitVector2[32] = true;
            Assert.False(bitVector.Data == bitVector2.Data);
            Assert.False(bitVector.GetHashCode() == bitVector2.GetHashCode());
        }

        [Fact]
        public void Constructors_MaxValue()
        {
            var bitVector = new BitVector32(-1);
            Assert.Equal(-1, bitVector.Data);
            Assert.Equal("BitVector32{11111111111111111111111111111111}", BitVector32.ToString(bitVector));
        }

        [Fact]
        public void Constructors_MinValue()
        {
            var bitVector = new BitVector32(0);
            Assert.Equal(0, bitVector.Data);
            Assert.Equal("BitVector32{00000000000000000000000000000000}", BitVector32.ToString(bitVector));
        }

        [Fact]
        public void Indexers()
        {
            var bitVector = new BitVector32(7);
            Assert.True(bitVector[0]);
            Assert.True(bitVector[1]);
            Assert.True(bitVector[2]);
            Assert.True(bitVector[4]);
            Assert.True(!bitVector[8]);
            Assert.True(!bitVector[16]);
            bitVector[8] = true;
            Assert.True(bitVector[4]);
            Assert.True(bitVector[8]);
            Assert.True(!bitVector[16]);
            bitVector[8] = false;
            Assert.True(bitVector[4]);
            Assert.True(!bitVector[8]);
            Assert.True(!bitVector[16]);

            var section = BitVector32.CreateSection(31);
            section = BitVector32.CreateSection(64, section);

            var bitVector1 = new BitVector32(0xffff77);
            var bitVector2 = new BitVector32(bitVector1[section]);
            Assert.Equal(123, bitVector1[section]);
        }

        [Fact]
        public void CreateMask()
        {
            Assert.Equal(1, BitVector32.CreateMask());
            Assert.Equal(1, BitVector32.CreateMask(0));
            Assert.Equal(2, BitVector32.CreateMask(1));
            Assert.Equal(32, BitVector32.CreateMask(16));
            var overflow = -2;
            Assert.Equal(overflow, BitVector32.CreateMask(int.MaxValue));
            // ReSharper disable once ConvertToConstant.Local
            var overflow2 = -4;
            Assert.Equal(overflow2, BitVector32.CreateMask(overflow));
            // ReSharper disable once ConvertToConstant.Local
            overflow = int.MinValue + 1;
            Assert.Equal(2, (int)BitVector32.CreateMask(overflow));
        }

        [Fact]
        public void CreateMask_MinValue()
        {
            // ReSharper disable once ConvertToConstant.Local
            var overflow = int.MinValue;

            Assert.Throws<InvalidOperationException>(() => BitVector32.CreateMask(overflow));
        }

        [Fact]
        public void CreateSection()
        {
            var section = BitVector32.CreateSection(1);
            Assert.Equal(1, section.Mask);

            section = BitVector32.CreateSection(2);
            Assert.Equal(3, section.Mask);

            section = BitVector32.CreateSection(3);
            Assert.Equal(3, section.Mask);

            section = BitVector32.CreateSection(5);
            Assert.Equal(7, section.Mask);

            section = BitVector32.CreateSection(20);
            Assert.Equal(0x1f, section.Mask);

            section = BitVector32.CreateSection(short.MaxValue);
            Assert.Equal(0x7fff, section.Mask);

            section = BitVector32.CreateSection(short.MaxValue - 100);
            Assert.Equal(0x7fff, section.Mask);

            Assert.Throws<ArgumentException>(() => BitVector32.CreateSection(0));
            Assert.Throws<ArgumentException>(() => BitVector32.CreateSection(-1));
            Assert.Throws<ArgumentException>(() => BitVector32.CreateSection(short.MinValue));

            section = BitVector32.CreateSection(20);
            Assert.Equal(0x1f, section.Mask);
            Assert.Equal(0x00, section.Offset);
            section = BitVector32.CreateSection(120, section);
            Assert.Equal(0x7f, section.Mask);
            Assert.Equal(0x05, section.Offset);
            section = BitVector32.CreateSection(1000, section);
            Assert.Equal(0x3ff, section.Mask);
            Assert.Equal(0x0c, section.Offset);
        }

        [Fact]
        public void Section()
        {
            var section1 = BitVector32.CreateSection(20);
            Assert.Equal(31, section1.Mask);
            Assert.Equal(0, section1.Offset);
            Assert.Equal("Section{0x1f, 0x0}", BitVector32.Section.ToString(section1));

            var section2 = BitVector32.CreateSection(20);
            Assert.True(section1.Equals(section2));
            Assert.True(section2.Equals((object)section1));
            Assert.Equal(section1.GetHashCode(), section2.GetHashCode());
            Assert.Equal("Section{0x1f, 0x0}", section2.ToString());
        }

        [Fact]
        public void SectionCorrectSize()
        {
            var section1 = BitVector32.CreateSection(32767);
            var section2 = BitVector32.CreateSection(32767, section1);
            var section3 = BitVector32.CreateSection(3, section2);
            var bitVector = new BitVector32(0)
            {
                [section3] = 3
            };
            Assert.Equal(3, bitVector[section3]);
        }

        [Fact]
        public void SectionIncorrectSize()
        {
            var section1 = BitVector32.CreateSection(32767);
            var section2 = BitVector32.CreateSection(32767, section1);
            BitVector32.CreateSection(4, section2);
        }

        [Fact]
        public void NegativeIndexer()
        {
            var bitVector = new BitVector32(-1);
            Assert.True(bitVector[0], "Int32.MinValue");
        }

        [Fact]
        public void TestSectionIndexer()
        {
            var bitVector = new BitVector32(-1);
            var section = BitVector32.CreateSection(1);
            section = BitVector32.CreateSection(short.MaxValue, section);
            section = BitVector32.CreateSection(short.MaxValue, section);
            section = BitVector32.CreateSection(1, section);
            Assert.Equal(1, bitVector[section]);
            bitVector[section] = 0;

            Assert.Equal(int.MaxValue, bitVector.Data);
        }

        [Fact]
        public void TestCreateSection1()
        {
            var section = BitVector32.CreateSection(short.MaxValue);
            Assert.Throws<ArgumentException>(() => BitVector32.CreateSection(0, section));
        }

        [Fact]
        public void TestCreateSection2()
        {
            var section = BitVector32.CreateSection(short.MaxValue);
            Assert.Throws<ArgumentException>(() => BitVector32.CreateSection(-1, section));
        }

        [Fact]
        public void TestCreateSection3()
        {
            var section = BitVector32.CreateSection(short.MaxValue);
            Assert.Throws<ArgumentException>(() => BitVector32.CreateSection(short.MinValue, section));
        }
    }
}
