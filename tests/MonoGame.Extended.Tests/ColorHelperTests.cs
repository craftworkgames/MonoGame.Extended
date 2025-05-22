// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests;

public class ColorHelperTests
{
    public class FromHex
    {
        [Theory]
        [InlineData("#00000000", 0, 0, 0, 0)]
        [InlineData("#FFFFFFFF", 255, 255, 255, 255)]
        [InlineData("#AABBCCFF", 170, 187, 204, 255)]
        public void LengthEight_WithPrefix_ReturnsCorrectColor(string hex, int r, int g, int b, int a)
        {
            Color expected = new Color(r, g, b, a);
            Color actual = ColorHelper.FromHex(hex);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("#000000", 0, 0, 0, 255)]
        [InlineData("#FFFFFF", 255, 255, 255, 255)]
        [InlineData("#AABBCC", 170, 187, 204, 255)]
        public void LengthSix_WithPrefix_ReturnsCorrectColor(string hex, int r, int g, int b, int a)
        {
            Color expected = new Color(r, g, b, a);
            Color actual = ColorHelper.FromHex(hex);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("#000", 0, 0, 0, 255)]
        [InlineData("#FFF", 255, 255, 255, 255)]
        [InlineData("#ABC", 170, 187, 204, 255)]
        public void LengthThree_WithPrefix_ReturnsCorrectColor(string hex, int r, int g, int b, int a)
        {
            Color expected = new Color(r, g, b, a);
            Color actual = ColorHelper.FromHex(hex);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("00000000", 0, 0, 0, 0)]
        [InlineData("FFFFFFFF", 255, 255, 255, 255)]
        [InlineData("AABBCCFF", 170, 187, 204, 255)]
        public void LengthEight_WithoutPrefix_ReturnsCorrectColor(string hex, int r, int g, int b, int a)
        {
            Color expected = new Color(r, g, b, a);
            Color actual = ColorHelper.FromHex(hex);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("000000", 0, 0, 0, 255)]
        [InlineData("FFFFFF", 255, 255, 255, 255)]
        [InlineData("AABBCC", 170, 187, 204, 255)]
        public void LengthSix_WithoutPrefix_ReturnsCorrectColor(string hex, int r, int g, int b, int a)
        {
            Color expected = new Color(r, g, b, a);
            Color actual = ColorHelper.FromHex(hex);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("000", 0, 0, 0, 255)]
        [InlineData("FFF", 255, 255, 255, 255)]
        [InlineData("ABC", 170, 187, 204, 255)]
        public void LengthThree_WithoutPrefix_ReturnsCorrectColor(string hex, int r, int g, int b, int a)
        {
            Color expected = new Color(r, g, b, a);
            Color actual = ColorHelper.FromHex(hex);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Empty_ReturnsTransparent()
        {
            Color expected = Color.Transparent;
            Color actual = ColorHelper.FromHex(string.Empty);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InvalidLength_ThrowsArgumentException()
        {
            string lengthOne = "0";
            string lengthTwo = "00";
            string lengthFive = "00000";
            string lengthSeven = "0000000";
            string lengthNine = "000000000";

            Assert.Throws<ArgumentException>(() => ColorHelper.FromHex(lengthOne));
            Assert.Throws<ArgumentException>(() => ColorHelper.FromHex(lengthTwo));
            Assert.Throws<ArgumentException>(() => ColorHelper.FromHex(lengthFive));
            Assert.Throws<ArgumentException>(() => ColorHelper.FromHex(lengthSeven));
            Assert.Throws<ArgumentException>(() => ColorHelper.FromHex(lengthNine));
        }
    }
}
