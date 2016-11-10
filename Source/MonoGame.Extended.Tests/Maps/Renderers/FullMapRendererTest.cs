using MonoGame.Extended.Maps.Renderers;
using NUnit.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Maps.Tiled;

namespace MonoGame.Extended.Tests.Maps.Renderers
{
    [TestFixture]
    public class FullMapRendererTest
    {
        [Test]
        public void Draw_MapObjectLayer_MissingGID_NoGroups()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            MockRenderer r = new MockRenderer(gd);
            Texture2D texture = Substitute.For<Texture2D>(gd, 64, 64);

            TiledMap m = new TiledMap("test", 2, 2, 32, 32);
            m.CreateTileset(texture, 0, 32, 32, 4);

            TiledObject[] objs =
            {
                new TiledObject(TiledObjectType.Tile, 1, null, 1, 1, 1, 1) { IsVisible = true },
            };

            TiledObjectGroup layer = new TiledObjectGroup("object", objs);
            m.AddLayer(layer);

            r.SwapMap(m);

            r.Draw(new Matrix());

            Assert.IsNull(gd.Indices);
        }

        [Test]
        public void Draw_MapObjectLayer_ShapeObject_NoGroups()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            MockRenderer r = new MockRenderer(gd);
            Texture2D texture = Substitute.For<Texture2D>(gd, 64, 64);

            TiledMap m = new TiledMap("test", 2, 2, 32, 32);
            m.CreateTileset(texture, 0, 32, 32, 4);

            TiledObject[] objs =
            {
                new TiledObject(TiledObjectType.Rectangle, 1, 1, 1, 1, 1, 1) { IsVisible = true },
            };

            TiledObjectGroup layer = new TiledObjectGroup("object", objs);
            m.AddLayer(layer);

            r.SwapMap(m);

            r.Draw(new Matrix());

            Assert.IsNull(gd.Indices);
        }

        [Test]
        public void Draw_MapObjectLayer_TileObject_OneGroup()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            MockRenderer r = new MockRenderer(gd);
            Texture2D texture = Substitute.For<Texture2D>(gd, 64, 64);

            TiledMap m = new TiledMap("test", 2, 2, 32, 32);
            m.CreateTileset(texture, 0, 32, 32, 4);

            TiledObject[] objs =
            {
                new TiledObject(TiledObjectType.Tile, 1, 1, 1, 1, 1, 1) { IsVisible = true },
            };

            TiledObjectGroup layer = new TiledObjectGroup("object", objs);
            m.AddLayer(layer);

            r.SwapMap(m);

            r.Draw(new Matrix());

            Assert.IsNotNull(gd.Indices);
            Assert.AreEqual(6, gd.Indices.IndexCount);
        }

        [Test]
        public void Draw_MapObjectLayer_NotVisible_NoGroups()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            MockRenderer r = new MockRenderer(gd);
            Texture2D texture = Substitute.For<Texture2D>(gd, 64, 64);

            TiledMap m = new TiledMap("test", 2, 2, 32, 32);
            m.CreateTileset(texture, 0, 32, 32, 4);

            TiledObject[] objs =
            {
                new TiledObject(TiledObjectType.Tile, 1, 1, 1, 1, 1, 1) { IsVisible = false },
            };

            TiledObjectGroup layer = new TiledObjectGroup("object", objs);
            m.AddLayer(layer);

            r.SwapMap(m);

            r.Draw(new Matrix());

            Assert.IsNull(gd.Indices);
        }

        [Test]
        public void Draw_MapObjectLayer_NoObjects_NoGroups()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            MockRenderer r = new MockRenderer(gd);
            Texture2D texture = Substitute.For<Texture2D>(gd, 64, 64);

            TiledMap m = new TiledMap("test", 2, 2, 32, 32);
            m.CreateTileset(texture, 0, 32, 32, 4);

            TiledObject[] objs = {};

            TiledObjectGroup layer = new TiledObjectGroup("object", objs);
            m.AddLayer(layer);

            r.SwapMap(m);

            r.Draw(new Matrix());

            Assert.IsNull(gd.Indices);
        }

        [Test]
        public void Draw_MapTileLayer_TwoVisible_OneGroup()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            MockRenderer r = new MockRenderer(gd);
            Texture2D texture = Substitute.For<Texture2D>(gd, 64, 64);

            TiledMap m = new TiledMap("test", 2, 2, 32, 32);
            m.CreateTileset(texture, 0, 32, 32, 4);
            m.CreateTileLayer("tile", 2, 2, new int[] { 1, 0, 1, 0 });

            r.SwapMap(m);

            r.Draw(new Matrix());

            Assert.IsNotNull(gd.Indices);
            Assert.AreEqual(12, gd.Indices.IndexCount);
        }

        [Test]
        public void Draw_MapTileLayer_AllBlank_NoGroups()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            MockRenderer r = new MockRenderer(gd);
            Texture2D texture = Substitute.For<Texture2D>(gd, 64, 64);

            TiledMap m = new TiledMap("test", 2, 2, 32, 32);
            m.CreateTileset(texture, 0, 32, 32, 4);
            m.CreateTileLayer("tile", 2, 2, new int[] { 0, 0, 0, 0 });

            r.SwapMap(m);

            r.Draw(new Matrix());

            Assert.IsNull(gd.Indices);
        }

        [Test]
        public void Draw_MapImageLayer_OneGroup()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            MockRenderer r = new MockRenderer(gd);
            Texture2D texture = Substitute.For<Texture2D>(gd, 64, 64);

            TiledMap m = new TiledMap("test", 10, 10, 32, 32);
            m.CreateImageLayer("img", texture, new Vector2(100, 100));

            r.SwapMap(m);

            r.Draw(new Matrix());

            Assert.IsNotNull(gd.Indices);
            Assert.AreEqual(6, gd.Indices.IndexCount);
        }

        [Test]
        public void Draw_MapNoGroups()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            MockRenderer r = new MockRenderer(gd);
            r.SwapMap(new TiledMap("test", 10, 10, 32, 32));

            r.Draw(new Matrix());

            Assert.IsNull(gd.Indices);
        }

        [Test]
        public void Draw_NoMap()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            MockRenderer r = new MockRenderer(gd);

            r.Draw(new Matrix());

            Assert.IsNull(gd.Indices);
        }

        [Test]
        public void CreatePrimatives()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            Texture2D texture = Substitute.For<Texture2D>(gd, 64, 64);
            TextureRegion2D region = Substitute.For<TextureRegion2D>(texture, 1, 1, 32, 32);

            VertexPositionTexture[] vertices;
            ushort[] indexes;

            MockRenderer r = new MockRenderer(gd);
            r.CreatePrimitives(new Point(0, 0), region, 0, 0.5f, out vertices, out indexes);

            Assert.AreEqual(4, vertices.Length);
            Assert.AreEqual(new Vector3(0, 0, .5f), vertices[0].Position);
            Assert.AreEqual(new Vector2(0.0234375f, 0.0234375f), vertices[0].TextureCoordinate);
            Assert.AreEqual(new Vector3(32, 0, .5f), vertices[1].Position);
            Assert.AreEqual(new Vector2(0.515625f, 0.0234375f), vertices[1].TextureCoordinate);
            Assert.AreEqual(new Vector3(0, 32, .5f), vertices[2].Position);
            Assert.AreEqual(new Vector2(0.0234375f, 0.515625f), vertices[2].TextureCoordinate);
            Assert.AreEqual(new Vector3(32, 32, .5f), vertices[3].Position);
            Assert.AreEqual(new Vector2(0.515625f, 0.515625f), vertices[3].TextureCoordinate);

            CollectionAssert.AreEqual(new[] { 0, 1, 2, 1, 3, 2 }, indexes);
        }

        [Test]
        public void CreatePrimatives_Offset10()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            Texture2D texture = Substitute.For<Texture2D>(gd, 64, 64);
            TextureRegion2D region = Substitute.For<TextureRegion2D>(texture, 1, 1, 32, 32);

            VertexPositionTexture[] vertices;
            ushort[] indexes;

            MockRenderer r = new MockRenderer(gd);
            r.CreatePrimitives(new Point(0, 0), region, 10, 0.5f, out vertices, out indexes);

            Assert.AreEqual(4, vertices.Length);
            Assert.AreEqual(new Vector3(0, 0, .5f), vertices[0].Position);
            Assert.AreEqual(new Vector2(0.0234375f, 0.0234375f), vertices[0].TextureCoordinate);
            Assert.AreEqual(new Vector3(32, 0, .5f), vertices[1].Position);
            Assert.AreEqual(new Vector2(0.515625f, 0.0234375f), vertices[1].TextureCoordinate);
            Assert.AreEqual(new Vector3(0, 32, .5f), vertices[2].Position);
            Assert.AreEqual(new Vector2(0.0234375f, 0.515625f), vertices[2].TextureCoordinate);
            Assert.AreEqual(new Vector3(32, 32, .5f), vertices[3].Position);
            Assert.AreEqual(new Vector2(0.515625f, 0.515625f), vertices[3].TextureCoordinate);

            CollectionAssert.AreEqual(new[] { 40, 41, 42, 41, 43, 42 }, indexes);
        }
    }

    class MockRenderer : FullMapRenderer
    {
        public MockRenderer(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }

        public void CreatePrimitives(Point point, TextureRegion2D region, int offset, float depth,
            out VertexPositionTexture[] vertices, out ushort[] indexes)
        {
            base.CreatePrimitives(point, region, offset, depth, out vertices, out indexes);
        }

        public new void Draw(Matrix viewMatrix)
        {
            base.Draw(viewMatrix);
        }
    }
}
