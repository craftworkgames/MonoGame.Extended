// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests;

public class ColorExtensionsTests
{
    public class ToHex
    {
        [Fact]
        public void Color_Transparent_ReturnsCorrectHex()
        {
            Color transparent = new Color(0, 0, 0, 0);
            string hex = transparent.ToHex();
            Assert.Equal("#00000000", hex);
        }

        [Fact]
        public void Color_White_ReturnsCorrectHex()
        {
            Color white = new Color(255, 255, 255);
            string hex = white.ToHex();
            Assert.Equal("#ffffffff", hex);
        }

        [Fact]
        public void Color_Black_ReturnsCorrectHex()
        {
            Color black = new Color(0, 0, 0);
            string hex = black.ToHex();
            Assert.Equal("#000000ff", hex);
        }

        [Fact]
        public void Color_Red_ReturnsCorrectHex()
        {
            Color red = new Color(255, 0, 0);
            string hex = red.ToHex();
            Assert.Equal("#ff0000ff", hex);
        }

        [Fact]
        public void Color_Green_ReturnsCorrectHex()
        {
            Color green = new Color(0, 255, 0);
            string hex = green.ToHex();
            Assert.Equal("#00ff00ff", hex);
        }

        [Fact]
        public void Color_Blue_ReturnsCorrectHex()
        {
            Color blue = new Color(0, 0, 255);
            string hex = blue.ToHex();
            Assert.Equal("#0000ffff", hex);
        }

        [Fact]
        public void Color_ReturnsCorrectHex()
        {
            Color color = new Color(170, 187, 204, 128);
            string hex = color.ToHex();
            Assert.Equal("#aabbcc80", hex);
        }
    }
}
