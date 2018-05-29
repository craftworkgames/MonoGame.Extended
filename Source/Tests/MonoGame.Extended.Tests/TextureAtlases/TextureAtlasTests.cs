using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Xunit;

namespace MonoGame.Extended.Tests.TextureAtlases
{
    //public class TextureAtlasTests : IDisposable
    //{
    //    private readonly TestGame _game;

    //    public TextureAtlasTests()
    //    {
    //        _game = new TestGame();
    //    }

    //    public void Dispose()
    //    {
    //        _game.Dispose();
    //    }

    //    [Fact]
    //    public void TextureAtlas_CreateRegion_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 100, 200))
    //        {
    //            var atlas = new TextureAtlas(null, texture);

    //            var region = atlas.CreateRegion("region0", 10, 20, 30, 40);

    //            Assert.Same(texture, region.Texture);
    //            Assert.Equal(10, region.X);
    //            Assert.Equal(20, region.Y);
    //            Assert.Equal(30, region.Width);
    //            Assert.Equal(40, region.Height);
    //        }
    //    }

    //    [Fact]
    //    public void TextureAtlas_GetRegionsByIndex_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 100, 200))
    //        {
    //            var atlas = new TextureAtlas(null, texture);

    //            var region0 = atlas.CreateRegion("region0", 10, 20, 30, 40);
    //            var region1 = atlas.CreateRegion("region1", 50, 60, 35, 45);

    //            Assert.Same(region0, atlas[0]);
    //            Assert.Same(region1, atlas[1]);
    //            Assert.Same(region0, atlas.GetRegion(0));
    //            Assert.Same(region1, atlas.GetRegion(1));
    //        }
    //    }


    //    [Fact]
    //    public void TextureAtlas_GetRegionsByName_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 100, 200))
    //        {
    //            var atlas = new TextureAtlas(null, texture);

    //            var region0 = atlas.CreateRegion("region0", 10, 20, 30, 40);
    //            var region1 = atlas.CreateRegion("region1", 50, 60, 35, 45);

    //            Assert.Same(region0, atlas["region0"]);
    //            Assert.Same(region1, atlas["region1"]);
    //            Assert.Same(region0, atlas.GetRegion("region0"));
    //            Assert.Same(region1, atlas.GetRegion("region1"));
    //        }
    //    }

    //    [Fact]
    //    public void TextureAtlas_RemoveRegions_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 100, 200))
    //        {
    //            var atlas = new TextureAtlas(null, texture);

    //            var region0 = atlas.CreateRegion("region0", 10, 20, 30, 40);
    //            var region1 = atlas.CreateRegion("region1", 50, 60, 35, 45);
    //            var region2 = atlas.CreateRegion("region2", 32, 33, 34, 35);

    //            Assert.Same(texture, atlas.Texture);
    //            Assert.Equal(3, atlas.RegionCount);
    //            Assert.Equal(atlas.RegionCount, atlas.Regions.Count());
    //            Assert.Same(region1, atlas[1]);

    //            atlas.RemoveRegion(1);

    //            Assert.Equal(2, atlas.Regions.Count());
    //            Assert.Same(region0, atlas[0]);
    //            Assert.Same(region2, atlas[1]);

    //            atlas.RemoveRegion("region0");

    //            Assert.Single(atlas.Regions);
    //            Assert.Same(region2, atlas[0]);
    //        }
    //    }

    //    [Fact]
    //    public void TextureAtlas_CreateRegionThatAlreadyExistsThrowsException_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 100, 200))
    //        {
    //            var atlas = new TextureAtlas(null, texture);

    //            atlas.CreateRegion("region0", 10, 20, 30, 40);
    //            Assert.Throws<InvalidOperationException>(() => atlas.CreateRegion("region0", 50, 60, 35, 45));
    //        }
    //    }

    //    [Fact]
    //    public void TextureAtlas_GetRegion_InvalidIndexThrowsException_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 100, 200))
    //        {
    //            var atlas = new TextureAtlas(null, texture);

    //            atlas.CreateRegion("region0", 10, 20, 30, 40);
    //            Assert.Throws<IndexOutOfRangeException>(() => atlas.GetRegion(-1));
    //        }
    //    }

    //    [Fact]
    //    public void TextureAtlas_GetRegion_InvalidNameThrowsException_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 100, 200))
    //        {
    //            var atlas = new TextureAtlas(null, texture);

    //            atlas.CreateRegion("region0", 10, 20, 30, 40);
    //            Assert.Throws<KeyNotFoundException>(() => atlas.GetRegion("region1"));
    //        }
    //    }

    //    [Fact]
    //    public void TextureAtlas_EnumerateRegions_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 100, 200))
    //        {
    //            var atlas = new TextureAtlas(null, texture);

    //            var regions = new TextureRegion2D[3];
    //            regions[0] = atlas.CreateRegion("region0", 10, 20, 30, 40);
    //            regions[1] = atlas.CreateRegion("region1", 50, 60, 35, 45);
    //            regions[2] = atlas.CreateRegion("region2", 32, 33, 34, 35);
    //            var index = 0;

    //            foreach (var region in atlas)
    //            {
    //                Assert.Same(region, regions[index]);
    //                index++;
    //            }
    //        }
    //    }
        
    //    [Fact]
    //    public void TextureAtlas_Create_WithDefaultParameters_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 50, 100) {Name = "testTexture"})
    //        {
    //            var atlas = TextureAtlas.Create(null, texture, 25, 50);

    //            Assert.Equal(4, atlas.RegionCount);
    //            Assert.True(atlas.Regions.All(i => i.Width == 25));
    //            Assert.True(atlas.Regions.All(i => i.Height == 50));
    //            Assert.True(atlas.Regions.All(i => ReferenceEquals(i.Texture, texture)));
    //            Assert.True(atlas.Regions.All(i => i.Name.StartsWith(texture.Name)));
    //        }
    //    }

    //    [Fact]
    //    public void TextureAtlas_Create_WithMaxRegionCount_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 64, 64))
    //        {
    //            var atlas = TextureAtlas.Create(null, texture, 32, 32, maxRegionCount: 3);

    //            Assert.Equal(3, atlas.RegionCount);
    //        }
    //    }

    //    [Fact]
    //    public void TextureAtlas_Create_WithMargin_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 24, 24))
    //        {
    //            var atlas = TextureAtlas.Create(null, texture, 10, 10, margin: 2);

    //            Assert.Equal(4, atlas.RegionCount);
    //            Assert.True(atlas.Regions.All(i => i.Width == 10 && i.Height == 10));
    //            Assert.Equal(2, atlas[0].X);
    //            Assert.Equal(2, atlas[0].Y);
    //            Assert.Equal(12, atlas[3].X);
    //            Assert.Equal(12, atlas[3].Y);
    //        }
    //    }

    //    [Fact]
    //    public void TextureAtlas_Create_WithSpacing_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 24, 24))
    //        {
    //            var atlas = TextureAtlas.Create(null, texture, 10, 10, spacing: 2);

    //            Assert.Equal(4, atlas.RegionCount);
    //            Assert.True(atlas.Regions.All(i => i.Width == 10 && i.Height == 10));
    //            Assert.Equal(0, atlas[0].X);
    //            Assert.Equal(0, atlas[0].Y);
    //            Assert.Equal(12, atlas[3].X);
    //            Assert.Equal(12, atlas[3].Y);
    //        }
    //    }

    //    [Fact]
    //    public void TextureAtlas_Create_WithMarginAndSpacing_Test()
    //    {
    //        using (var texture = new Texture2D(_game.GraphicsDevice, 28, 28))
    //        {
    //            var atlas = TextureAtlas.Create(null, texture, 10, 10, margin: 3, spacing: 2);

    //            Assert.Equal(4, atlas.RegionCount);
    //            Assert.True(atlas.Regions.All(i => i.Width == 10 && i.Height == 10));
    //            Assert.Equal(3, atlas[0].X);
    //            Assert.Equal(3, atlas[0].Y);
    //            Assert.Equal(15, atlas[3].X);
    //            Assert.Equal(15, atlas[3].Y);
    //        }
    //    }
    //}
}
