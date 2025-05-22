// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests;

public class ColorExtensionsTests
{
    public class ToHex
    {
        [Theory]
        [InlineData(0, 0, 0, 0, "#00000000")]
        [InlineData(255, 255, 255, 255, "#ffffffff")]
        [InlineData(0, 0, 0, 255, "#000000ff")]
        [InlineData(0, 0, 255, 0, "#0000ff00")]
        [InlineData(0, 255, 0, 0, "#00ff0000")]
        [InlineData(255, 0, 0, 0, "#ff000000")]
        [InlineData(171, 205, 239, 128, "#abcdef80")]
        public void ReturnsCorrectHex(int r, int g, int b, int a, string expected)
        {
            Color color = new Color(r, g, b, a);
            string result = color.ToHex();
            Assert.Equal(expected, result);
        }
    }
}
