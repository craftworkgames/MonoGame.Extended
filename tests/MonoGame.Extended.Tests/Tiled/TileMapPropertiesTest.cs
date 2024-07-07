// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace MonoGame.Extended.Tiled.Tests;

public class TileMapPropertiesTest
{
    // NullReferenceException in TiledMapProperties
    // https://github.com/craftworkgames/MonoGame.Extended/issues/901
    [Fact]
    public void TryGetValue_InvalidKey_ReturnsFalse()
    {
        TiledMapProperties properties = new TiledMapProperties();
        var result = properties.TryGetValue("invalid", out string value);
        Assert.False(result);
        Assert.True(string.IsNullOrEmpty(value));
    }

    [Fact]
    public void TryGetValue_ValidKey_ReturnsFalse()
    {
        var expected = "value";
        TiledMapProperties properties = new TiledMapProperties();
        properties.Add("test", new TiledMapPropertyValue(expected));

        var result = properties.TryGetValue("test", out string actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }
}
