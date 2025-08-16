using System;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Tests.Graphics;

public sealed class Texture2DAtlasTests
{
    // Reference: https://github.com/MonoGame-Extended/Monogame-Extended/issues/1013
    // Region names being generated during TextureAtlas.Create were not unique
    // which was leading to an exception being thrown after the first region was
    // added
    [Fact]
    public void CalculateRegions_ShouldGenerateUniqueRegionNames()
    {
        ReadOnlySpan<Texture2DAtlas.CalculatedRegion> regions = Texture2DAtlas.CalculateRegions("spritesheet", 64, 64, 32, 32, int.MaxValue, 0, 0);

        Assert.Equal(4, regions.Length);
        Assert.Equal("spritesheet_0", regions[0].Name);
        Assert.Equal("spritesheet_1", regions[1].Name);
        Assert.Equal("spritesheet_2", regions[2].Name);
        Assert.Equal("spritesheet_3", regions[3].Name);
    }
}
