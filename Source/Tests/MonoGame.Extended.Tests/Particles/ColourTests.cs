using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Particles
{
    [TestFixture]
    public class ColourTests
    {
        public class Constructor
        {
            [Test]
            public void WhenGivenValues_ReturnsInitializedColour()
            {
                var colour = new HslColor(1f, 1f, 1f);
                Assert.AreEqual(colour.H, 1f);
                Assert.AreEqual(colour.S, 1f);
                Assert.AreEqual(colour.L, 1f);
            }
        }

        public class AreEqualColourMethod
        {
            [Test]
            public void WhenGivenEqualValues_ReturnsTrue()
            {
                var x = new HslColor(360f, 1f, 1f);
                var y = new HslColor(360f, 1f, 1f);
                Assert.AreEqual(x, y);
            }

            [Test]
            public void WhenGivenDifferentValues_ReturnsFalse()
            {
                var x = new HslColor(0f, 1f, 0f);
                var y = new HslColor(360f, 1f, 1f);
                Assert.IsFalse(x.Equals(y));
            }
        }

        public class AreEqualObjectMethod
        {
            [Test]
            public void WhenGivenNull_ReturnsFalse()
            {
                var colour = new HslColor(360f, 1f, 1f);
                Assert.IsNotNull(colour);
            }

            [Test]

            public void WhenGivenEqualColour_ReturnsTrue()
            {
                var x = new HslColor(360f, 1f, 1f);

                Object y = new HslColor(360f, 1f, 1f);
                Assert.AreEqual(x, y);
            }

            [Test]

            public void WhenGivenDifferentColour_ReturnsFalse()
            {
                var x = new HslColor(360f, 1f, 1f);

                Object y = new HslColor(0f, 1f, 0f);
                Assert.IsFalse(x.Equals(y));
            }

            [Test]

            public void WhenGivenObjectOfAntotherType_ReturnsFalse()
            {
                var colour = new HslColor(360f, 1f, 1f);

                // ReSharper disable once SuspiciousTypeConversion.Global
                Assert.IsFalse(colour.Equals(DateTime.Now));
            }
        }

        public class GetHashCodeMethod
        {
            [Test]

            public void WhenObjectsAreDifferent_YieldsDifferentHashCodes()
            {
                var x = new HslColor(0f, 1f, 0f);
                var y = new HslColor(360f, 1f, 1f);
                Assert.AreNotEqual(x.GetHashCode(), y.GetHashCode());
            }

            [Test]

            public void WhenObjectsAreSame_YieldsIdenticalHashCodes()
            {
                var x = new HslColor(180f, 0.5f, 0.5f);
                var y = new HslColor(180f, 0.5f, 0.5f);
                Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
            }
        }

        public class ToStringMethod
        {
            [Theory]
            [TestCase(360f, 1f, 1f, "H:0.0° S:100.0 L:100.0")]
            [TestCase(180f, 0.5f, 0.5f, "H:180.0° S:50.0 L:50.0")]
            [TestCase(0f, 0f, 0f, "H:0.0° S:0.0 L:0.0")]
            public void ReturnsCorrectValue(float h, float s, float l, string expected)
            {
                var colour = new HslColor(h, s, l);
                Assert.AreEqual(expected, colour.ToString());
            }
        }

        public class ToRgbMethod
        {
            [TestCase(0f, 1f, 0.5f, "{R:255 G:0 B:0 A:255}")] // Color.Red
            [TestCase(360f, 1f, 0.5f, "{R:255 G:0 B:0 A:255}")] // Color.Red
            [TestCase(120f, 1f, 0.5f, "{R:0 G:255 B:0 A:255}")] // Color.Lime
            public void ReturnsCorrectValue(float h, float s, float l, string expected)
            {
                var hslColour = new HslColor(h, s, l);
                Color rgbColor = hslColour.ToRgb();

                Assert.AreEqual(expected, rgbColor.ToString());
            }

            [Test]
            public void FromRgbAndToRgbWorksCorrectly()
            {
                HslColor blueHsl = HslColor.FromRgb(Color.Blue);
                Color blueRgb = blueHsl.ToRgb();

                Assert.AreEqual(Color.Blue, blueRgb);
            }
        }
    }
}