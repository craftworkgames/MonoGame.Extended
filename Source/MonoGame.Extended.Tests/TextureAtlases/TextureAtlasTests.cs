using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Tests.TextureAtlases
{
    [TestFixture]
    public class TextureAtlasTests
    {
        private Game _game;

        [SetUp]
        public void RunBeforeAnyTests()
        {
            _game = TestHelper.CreateGame();
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            _game.Dispose();
        }

        [Test]
        public void TextureAtlas_Create_WithDefaultParameters_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 50, 100)
            {
                Name = "testTexture"
            };
            var atlas = TextureAtlas.Create(texture, 25, 50);

            Assert.AreEqual(4, atlas.RegionCount);
            Assert.IsTrue(atlas.Regions.All(i => i.Width == 25));
            Assert.IsTrue(atlas.Regions.All(i => i.Height == 50));
            Assert.IsTrue(atlas.Regions.All(i => ReferenceEquals(i.Texture, texture)));
            Assert.IsTrue(atlas.Regions.All(i => i.Name.StartsWith(texture.Name)));
        }

        [Test]
        public void TextureAtlas_Create_WithMargin_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 24, 24);
            var atlas = TextureAtlas.Create(texture, 10, 10, margin: 2);

            Assert.AreEqual(4, atlas.RegionCount);
            Assert.IsTrue(atlas.Regions.All(i => i.Width == 10 && i.Height == 10));
            Assert.AreEqual(atlas[0].X, 2);
            Assert.AreEqual(atlas[0].Y, 2);
            Assert.AreEqual(atlas[3].X, 12);
            Assert.AreEqual(atlas[3].Y, 12);
        }

        [Test]
        public void TextureAtlas_Create_WithMarginAndSpacing_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 28, 28);
            var atlas = TextureAtlas.Create(texture, 10, 10, margin: 3, spacing: 2);

            Assert.AreEqual(4, atlas.RegionCount);
            Assert.IsTrue(atlas.Regions.All(i => i.Width == 10 && i.Height == 10));
            Assert.AreEqual(atlas[0].X, 3);
            Assert.AreEqual(atlas[0].Y, 3);
            Assert.AreEqual(atlas[3].X, 15);
            Assert.AreEqual(atlas[3].Y, 15);
        }

        [Test]
        public void TextureAtlas_Create_WithMaxRegionCount_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 64, 64);
            var atlas = TextureAtlas.Create(texture, 32, 32, 3);

            Assert.AreEqual(3, atlas.RegionCount);
        }

        [Test]
        public void TextureAtlas_Create_WithSpacing_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 24, 24);
            var atlas = TextureAtlas.Create(texture, 10, 10, spacing: 2);

            Assert.AreEqual(4, atlas.RegionCount);
            Assert.IsTrue(atlas.Regions.All(i => i.Width == 10 && i.Height == 10));
            Assert.AreEqual(atlas[0].X, 0);
            Assert.AreEqual(atlas[0].Y, 0);
            Assert.AreEqual(atlas[3].X, 12);
            Assert.AreEqual(atlas[3].Y, 12);
        }

        [Test]
        public void TextureAtlas_CreateRegion_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 100, 200);
            var atlas = new TextureAtlas(texture);

            var region = atlas.CreateRegion("region0", 10, 20, 30, 40);

            Assert.AreSame(texture, region.Texture);
            Assert.AreEqual(10, region.X);
            Assert.AreEqual(20, region.Y);
            Assert.AreEqual(30, region.Width);
            Assert.AreEqual(40, region.Height);
        }

        [Test]
        public void TextureAtlas_CreateRegionThatAlreadyExistsThrowsException_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 100, 200);
            var atlas = new TextureAtlas(texture);

            atlas.CreateRegion("region0", 10, 20, 30, 40);
            Assert.Throws<InvalidOperationException>(() => atlas.CreateRegion("region0", 50, 60, 35, 45));
        }

        [Test]
        public void TextureAtlas_EnumerateRegions_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 100, 200);
            var atlas = new TextureAtlas(texture);

            var regions = new TextureRegion2D[3];
            regions[0] = atlas.CreateRegion("region0", 10, 20, 30, 40);
            regions[1] = atlas.CreateRegion("region1", 50, 60, 35, 45);
            regions[2] = atlas.CreateRegion("region2", 32, 33, 34, 35);
            var index = 0;

            foreach (var region in atlas)
            {
                Assert.AreSame(region, regions[index]);
                index++;
            }
        }

        [Test]
        public void TextureAtlas_GetRegion_InvalidIndexThrowsException_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 100, 200);
            var atlas = new TextureAtlas(texture);

            atlas.CreateRegion("region0", 10, 20, 30, 40);
            Assert.Throws<IndexOutOfRangeException>(() => atlas.GetRegion(-1));
        }

        [Test]
        public void TextureAtlas_GetRegion_InvalidNameThrowsException_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 100, 200);
            var atlas = new TextureAtlas(texture);

            atlas.CreateRegion("region0", 10, 20, 30, 40);
            Assert.Throws<KeyNotFoundException>(() => atlas.GetRegion("region1"));
        }

        [Test]
        public void TextureAtlas_GetRegionsByIndex_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 100, 200);
            var atlas = new TextureAtlas(texture);

            var region0 = atlas.CreateRegion("region0", 10, 20, 30, 40);
            var region1 = atlas.CreateRegion("region1", 50, 60, 35, 45);

            Assert.AreSame(region0, atlas[0]);
            Assert.AreSame(region1, atlas[1]);
            Assert.AreSame(region0, atlas.GetRegion(0));
            Assert.AreSame(region1, atlas.GetRegion(1));
        }

        [Test]
        public void TextureAtlas_GetRegionsByName_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 100, 200);
            var atlas = new TextureAtlas(texture);

            var region0 = atlas.CreateRegion("region0", 10, 20, 30, 40);
            var region1 = atlas.CreateRegion("region1", 50, 60, 35, 45);

            Assert.AreSame(region0, atlas["region0"]);
            Assert.AreSame(region1, atlas["region1"]);
            Assert.AreSame(region0, atlas.GetRegion("region0"));
            Assert.AreSame(region1, atlas.GetRegion("region1"));
        }

        [Test]
        public void TextureAtlas_RemoveRegions_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 100, 200);
            var atlas = new TextureAtlas(texture);

            var region0 = atlas.CreateRegion("region0", 10, 20, 30, 40);
            var region1 = atlas.CreateRegion("region1", 50, 60, 35, 45);
            var region2 = atlas.CreateRegion("region2", 32, 33, 34, 35);

            Assert.AreSame(texture, atlas.Texture);
            Assert.AreEqual(3, atlas.RegionCount);
            Assert.AreEqual(atlas.RegionCount, atlas.Regions.Count());
            Assert.AreSame(region1, atlas[1]);

            atlas.RemoveRegion(1);

            Assert.AreEqual(2, atlas.Regions.Count());
            Assert.AreSame(region0, atlas[0]);
            Assert.AreSame(region2, atlas[1]);

            atlas.RemoveRegion("region0");

            Assert.AreEqual(1, atlas.Regions.Count());
            Assert.AreSame(region2, atlas[0]);
        }
    }
}