using System;
using MonoGame.Extended.Collections;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Collections
{
    internal class BitVector32Tests
    {
        [TestFixture]
        public class BitVector32Test
        {
            [Test]
            public void Constructors()
            {
                var bitVector = new BitVector32((uint)31);
                Assert.AreEqual(31, bitVector);
                Assert.IsTrue(bitVector[31]);
                Assert.IsFalse(bitVector[32]);
                Assert.AreEqual(bitVector.ToString(), "BitVector32{00000000000000000000000000011111}");

                var bitVector2 = new BitVector32(bitVector);
                Assert.IsTrue(bitVector == bitVector2);
                Assert.AreEqual(bitVector.GetHashCode(), bitVector2.GetHashCode());

                bitVector2[32] = true;
                Assert.IsFalse(bitVector == bitVector2);
                Assert.IsFalse(bitVector.GetHashCode() == bitVector2.GetHashCode());
            }

            [Test]
            public void Constructors_MaxValue()
            {
                var bitVector = new BitVector32(uint.MaxValue);
                Assert.AreEqual(uint.MaxValue, bitVector);
                Assert.AreEqual("BitVector32{11111111111111111111111111111111}", BitVector32.ToString(bitVector));
            }

            [Test]
            public void Constructors_MinValue()
            {
                var bitVector = new BitVector32(uint.MinValue);
                Assert.AreEqual(uint.MinValue, bitVector);
                Assert.AreEqual("BitVector32{00000000000000000000000000000000}", BitVector32.ToString(bitVector));
            }

            [Test]
            public void Indexers()
            {
                var bitVector = new BitVector32((uint)7);
                Assert.IsTrue(bitVector[0]);
                Assert.IsTrue(bitVector[1]);
                Assert.IsTrue(bitVector[2]);
                Assert.IsTrue(bitVector[4]);
                Assert.IsTrue(!bitVector[8]);
                Assert.IsTrue(!bitVector[16]);
                bitVector[8] = true;
                Assert.IsTrue(bitVector[4]);
                Assert.IsTrue(bitVector[8]);
                Assert.IsTrue(!bitVector[16]);
                bitVector[8] = false;
                Assert.IsTrue(bitVector[4]);
                Assert.IsTrue(!bitVector[8]);
                Assert.IsTrue(!bitVector[16]);

                var section = BitVector32.CreateSection(31);
                section = BitVector32.CreateSection(64, section);

                var bitVector1 = new BitVector32((uint)0xffff77);
                var bitVector2 = new BitVector32((uint)bitVector1[section]);
                Assert.AreEqual(123, bitVector1[section]);
            }

            [Test]
            public void CreateMask()
            {
                Assert.AreEqual(1, BitVector32.CreateMask());
                Assert.AreEqual(1, BitVector32.CreateMask(0));
                Assert.AreEqual(2, BitVector32.CreateMask(1));
                Assert.AreEqual(32, BitVector32.CreateMask(16));
                var overflow = -2;
                Assert.AreEqual((uint)overflow, BitVector32.CreateMask(int.MaxValue));
                // ReSharper disable once ConvertToConstant.Local
                var overflow2 = -4;
                Assert.AreEqual((uint)overflow2, BitVector32.CreateMask((uint)overflow));
                // ReSharper disable once ConvertToConstant.Local
                overflow = int.MinValue + 1;
                Assert.AreEqual(2, BitVector32.CreateMask((uint)overflow));
            }

            [Test]
            [ExpectedException(typeof (InvalidOperationException))]
            public void CreateMask_MinValue()
            {
                // ReSharper disable once ConvertToConstant.Local
                var overflow = int.MinValue;
                BitVector32.CreateMask((uint)overflow);
            }

            [Test]
            public void CreateSection()
            {
                var section = BitVector32.CreateSection(1);
                Assert.AreEqual(1, section.Mask);

                section = BitVector32.CreateSection(2);
                Assert.AreEqual(3, section.Mask);

                section = BitVector32.CreateSection(3);
                Assert.AreEqual(3, section.Mask);

                section = BitVector32.CreateSection(5);
                Assert.AreEqual(7, section.Mask);

                section = BitVector32.CreateSection(20);
                Assert.AreEqual(0x1f, section.Mask);

                section = BitVector32.CreateSection(short.MaxValue);
                Assert.AreEqual(0x7fff, section.Mask);

                section = BitVector32.CreateSection(short.MaxValue - 100);
                Assert.AreEqual(0x7fff, section.Mask);

                try
                {
                    BitVector32.CreateSection(0);
                    Assert.Fail();
                }
                catch (ArgumentException)
                {
                }

                try
                {
                    BitVector32.CreateSection(-1);
                    Assert.Fail();
                }
                catch (ArgumentException)
                {
                }

                try
                {
                    BitVector32.CreateSection(short.MinValue);
                    Assert.Fail();
                }
                catch (ArgumentException)
                {
                }

                section = BitVector32.CreateSection(20);
                Assert.AreEqual(0x1f, section.Mask);
                Assert.AreEqual(0x00, section.Offset);
                section = BitVector32.CreateSection(120, section);
                Assert.AreEqual(0x7f, section.Mask);
                Assert.AreEqual(0x05, section.Offset);
                section = BitVector32.CreateSection(1000, section);
                Assert.AreEqual(0x3ff, section.Mask);
                Assert.AreEqual(0x0c, section.Offset);
            }

            [Test]
            public void Section()
            {
                var section1 = BitVector32.CreateSection(20);
                Assert.AreEqual(31, section1.Mask);
                Assert.AreEqual(0, section1.Offset);
                Assert.AreEqual("Section{0x1f, 0x0}", BitVector32.Section.ToString(section1));

                var section2 = BitVector32.CreateSection(20);
                Assert.IsTrue(section1.Equals(section2));
                Assert.IsTrue(section2.Equals((object)section1));
                Assert.AreEqual(section1.GetHashCode(), section2.GetHashCode());
                Assert.AreEqual("Section{0x1f, 0x0}", section2.ToString());
            }

            [Test]
            public void SectionCorrectSize()
            {
                var section1 = BitVector32.CreateSection(32767);
                var section2 = BitVector32.CreateSection(32767, section1);
                var section3 = BitVector32.CreateSection(3, section2);
                var bitVector = new BitVector32((uint)0)
                {
                    [section3] = 3
                };
                Assert.AreEqual(bitVector[section3], 3);
            }

            [Test]
            public void SectionIncorrectSize()
            {
                var section1 = BitVector32.CreateSection(32767);
                var section2 = BitVector32.CreateSection(32767, section1);
                BitVector32.CreateSection(4, section2);
            }

            [Test]
            public void NegativeIndexer()
            {
                var bitVector = new BitVector32(uint.MaxValue);
                Assert.IsTrue(bitVector[uint.MinValue], "UInt32.MinValue");
            }

            [Test]
            public void TestSectionIndexer()
            {
                var bitVector = new BitVector32(uint.MaxValue);
                var section = BitVector32.CreateSection(1);
                section = BitVector32.CreateSection(short.MaxValue, section);
                section = BitVector32.CreateSection(short.MaxValue, section);
                section = BitVector32.CreateSection(1, section);
                Assert.AreEqual(1, bitVector[section]);
                bitVector[section] = 0;

                Assert.AreEqual(int.MaxValue, bitVector);
            }

            [Test, ExpectedException(typeof (ArgumentException))]
            public void TestCreateSection1()
            {
                var section = BitVector32.CreateSection(short.MaxValue);
                BitVector32.CreateSection(0, section);
            }

            [Test, ExpectedException(typeof (ArgumentException))]
            public void TestCreateSection2()
            {
                var section = BitVector32.CreateSection(short.MaxValue);
                BitVector32.CreateSection(-1, section);
            }

            [Test, ExpectedException(typeof (ArgumentException))]
            public void TestCreateSection3()
            {
                var section = BitVector32.CreateSection(short.MaxValue);
                BitVector32.CreateSection(short.MinValue, section);
            }

            private void Print(BitVector32.Section s)
            {
                Console.WriteLine(s + " : " + s.Mask + " : " + s.Offset);
            }
        }
    }
}
