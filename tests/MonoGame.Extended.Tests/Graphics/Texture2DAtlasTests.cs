// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Tests.Graphics;

public class Texture2DAtlasTests : IClassFixture<TestGraphicsDeviceFixture>
{
    private readonly TestGraphicsDeviceFixture _fixture;

    public Texture2DAtlasTests(TestGraphicsDeviceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Initialize_WithValidTexture_ShouldSetProperties()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);

        Assert.Equal($"{texture.Name}Atlas", atlas.Name);
        Assert.Equal(texture, atlas.Texture);
    }

    [Fact]
    public void Initialize_WithNullTexture_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Texture2DAtlas(null));
    }

    [Fact]
    public void Initialize_WithDisposedTexture_ShouldThrowObjectDisposedException()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        texture.Dispose();

        Assert.Throws<ObjectDisposedException>(() => new Texture2DAtlas(texture));
    }

    [Fact]
    public void CreateRegion_ShouldAddRegionToAtlas()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);
        var region = atlas.CreateRegion(0, 0, 10, 10);

        Assert.Equal(1, atlas.RegionCount);
        Assert.Equal(region, atlas[0]);
    }

    [Fact]
    public void CreateRegion_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);

        atlas.CreateRegion("Region1", 0, 0, 10, 10);
        Assert.Throws<InvalidOperationException>(() => atlas.CreateRegion("Region1", 10, 10, 10, 10));
    }

    [Fact]
    public void GetRegion_ByIndex_ShouldReturnCorrectRegion()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);
        var region = atlas.CreateRegion(0, 0, 10, 10);

        Assert.Equal(region, atlas[0]);
    }

    [Fact]
    public void GetRegion_ByInvalidIndex_ShouldThrowArgumentOutOfRangeException()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);

        Assert.Throws<ArgumentOutOfRangeException>(() => atlas[0]);
    }

    [Fact]
    public void GetRegion_ByName_ShouldReturnCorrectRegion()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);
        var region = atlas.CreateRegion("Region1", 0, 0, 10, 10);

        Assert.Equal(region, atlas["Region1"]);
    }

    [Fact]
    public void GetRegion_ByInvalidName_ShouldThrowKeyNotFoundException()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);

        Assert.Throws<KeyNotFoundException>(() => atlas["InvalidName"]);
    }

    [Fact]
    public void ContainsRegion_ShouldReturnTrueIfRegionExists()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);
        atlas.CreateRegion("Region1", 0, 0, 10, 10);

        Assert.True(atlas.ContainsRegion("Region1"));
    }

    [Fact]
    public void ContainsRegion_ShouldReturnFalseIfRegionDoesNotExist()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);

        Assert.False(atlas.ContainsRegion("InvalidName"));
    }

    [Fact]
    public void GetIndexOfRegion_ShouldReturnCorrectIndex()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);
        atlas.CreateRegion("Region1", 0, 0, 10, 10);

        Assert.Equal(0, atlas.GetIndexOfRegion("Region1"));
    }

    [Fact]
    public void GetIndexOfRegion_ShouldReturnMinusOneIfRegionDoesNotExist()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);

        Assert.Equal(-1, atlas.GetIndexOfRegion("InvalidName"));
    }

    [Fact]
    public void RemoveRegion_ByIndex_ShouldRemoveRegion()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);
        atlas.CreateRegion(0, 0, 10, 10);

        var removed = atlas.RemoveRegion(0);

        Assert.True(removed);
        Assert.Equal(0, atlas.RegionCount);
    }

    [Fact]
    public void RemoveRegion_ByName_ShouldRemoveRegion()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);
        atlas.CreateRegion("Region1", 0, 0, 10, 10);

        var removed = atlas.RemoveRegion("Region1");

        Assert.True(removed);
        Assert.Equal(0, atlas.RegionCount);
    }

    [Fact]
    public void ClearRegions_ShouldRemoveAllRegions()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);
        atlas.CreateRegion(0, 0, 10, 10);
        atlas.CreateRegion(10, 10, 10, 10);

        atlas.ClearRegions();

        Assert.Equal(0, atlas.RegionCount);
    }

    [Fact]
    public void CreateSprite_ByIndex_ShouldReturnSprite()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);
        atlas.CreateRegion(0, 0, 10, 10);

        var sprite = atlas.CreateSprite(0);

        Assert.NotNull(sprite);
        Assert.Equal(atlas[0], sprite.TextureRegion);
    }

    [Fact]
    public void CreateSprite_ByInvalidIndex_ShouldThrowArgumentOutOfRangeException()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);

        Assert.Throws<ArgumentOutOfRangeException>(() => atlas.CreateSprite(0));
    }

    [Fact]
    public void CreateSprite_ByName_ShouldReturnSprite()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);
        atlas.CreateRegion("Region1", 0, 0, 10, 10);

        var sprite = atlas.CreateSprite("Region1");

        Assert.NotNull(sprite);
        Assert.Equal(atlas["Region1"], sprite.TextureRegion);
    }

    [Fact]
    public void CreateSprite_ByInvalidName_ShouldThrowKeyNotFoundException()
    {
        var texture = new Texture2D(_fixture.GraphicsDevice, 100, 100);
        var atlas = new Texture2DAtlas(texture);

        Assert.Throws<KeyNotFoundException>(() => atlas.CreateSprite("InvalidName"));
    }
}
