//#region

//using Microsoft.Xna.Framework.Content.Pipeline;
//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.Tests;
//using Xunit;

//#endregion

//namespace MonoGame.Extended.Tiled.Tests
//{
//    
//    public class TiledTilesetTests
//    {
//        [Fact]
//        public void Constructor()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = new Texture2D(graphicsDevice, 64, 64);

//            var tiledTileset = new TiledMapTileset(texture, 10, 32, 32, 4, 0, 0);


//            //Assert.IsNull(tiledTileset.GetTileRegion(0));
//        }

//        [Fact]
//        public void GetTileRegion_BlankTile()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = new Texture2D(graphicsDevice, 64, 64);

//            var tiledTileset = new TiledTileset(texture, 10, 32, 32, 4, 0, 0);


//            //Assert.IsNull(tiledTileset.GetTileRegion(0));
//        }

//        [Fact]
//        [TestCase(9, Result = false, Description = "Too low")]
//        [TestCase(10, Result = true, Description = "Min tile")]
//        [TestCase(11, Result = true, Description = "Middle tile")]
//        [TestCase(13, Result = true, Description = "Last tile")]
//        [TestCase(14, Result = false, Description = "Too high")]
//        public bool ContainsTileId(int id)
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = new Texture2D(graphicsDevice, 64, 64);

//            var tiledTileset = new TiledTileset(texture, 10, 32, 32, 4, 0, 0);

//            return tiledTileset.ContainsTileId(id);
//        }

//        [Fact]
//        public void Constructor_NoMargin([Values(0, 2)] int spacing)
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = new Texture2D(graphicsDevice, 64, 64);

//            var tiledTileset = new TiledTileset(texture, 1, 32, 32, 4, spacing, 0);

//            var region = tiledTileset.GetTileRegion(1);
//            Assert.Equal(texture, region.Texture);
//            Assert.Equal(0, region.X);
//            Assert.Equal(0, region.Y);
//            Assert.Equal(32, region.Width);
//            Assert.Equal(32, region.Height);

//            region = tiledTileset.GetTileRegion(2);
//            Assert.Equal(texture, region.Texture);
//            Assert.Equal(spacing + 32, region.X);
//            Assert.Equal(0, region.Y);
//            Assert.Equal(32, region.Width);
//            Assert.Equal(32, region.Height);

//            region = tiledTileset.GetTileRegion(3);
//            Assert.Equal(texture, region.Texture);
//            Assert.Equal(0, region.X);
//            Assert.Equal(spacing + 32, region.Y);
//            Assert.Equal(32, region.Width);
//            Assert.Equal(32, region.Height);

//            region = tiledTileset.GetTileRegion(4);
//            Assert.Equal(texture, region.Texture);
//            Assert.Equal(spacing + 32, region.X);
//            Assert.Equal(spacing + 32, region.Y);
//            Assert.Equal(32, region.Width);
//            Assert.Equal(32, region.Height);
//        }

//        [Fact]
//        public void Constructor_NoSpacing([Values(0, 2)] int margin)
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = new Texture2D(graphicsDevice, 64, 64);

//            var tileset = new TiledTileset(texture, 1, 32, 32, 4, 0, margin);

//            var region = tileset.GetTileRegion(1);
//            Assert.Equal(texture, region.Texture);
//            Assert.Equal(margin, region.X);
//            Assert.Equal(margin, region.Y);
//            Assert.Equal(32, region.Width);
//            Assert.Equal(32, region.Height);

//            region = tileset.GetTileRegion(2);
//            Assert.Equal(texture, region.Texture);
//            Assert.Equal(margin + 32, region.X);
//            Assert.Equal(margin, region.Y);
//            Assert.Equal(32, region.Width);
//            Assert.Equal(32, region.Height);

//            region = tileset.GetTileRegion(3);
//            Assert.Equal(texture, region.Texture);
//            Assert.Equal(margin, region.X);
//            Assert.Equal(margin + 32, region.Y);
//            Assert.Equal(32, region.Width);
//            Assert.Equal(32, region.Height);

//            region = tileset.GetTileRegion(4);
//            Assert.Equal(texture, region.Texture);
//            Assert.Equal(margin + 32, region.X);
//            Assert.Equal(margin + 32, region.Y);
//            Assert.Equal(32, region.Width);
//            Assert.Equal(32, region.Height);
//        }
//    }
//}
