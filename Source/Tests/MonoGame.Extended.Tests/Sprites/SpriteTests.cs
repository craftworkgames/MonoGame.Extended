//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.Sprites;
//using MonoGame.Extended.TextureAtlases;
//using NSubstitute;
//using Xunit;

//namespace MonoGame.Extended.Tests.Sprites
//{
//    
//    public class SpriteTests
//    {
//        [Fact]
//        public void Sprite_BoundingRectangleAfterPosition_Test()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = Substitute.For<Texture2D>(graphicsDevice, 50, 200);
//            var sprite = new Sprite(texture);

//            Assert.Equal(new RectangleF(375, 140, 50, 200), sprite.GetBoundingRectangle(new Vector2(400, 240), 0, Vector2.One));
//        }

//        [Fact]
//        public void Sprite_BoundingRectangleAfterOrigin_Test()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = Substitute.For<Texture2D>(graphicsDevice, 50, 200);
//            var sprite = new Sprite(texture) { OriginNormalized = new Vector2(1.0f, 1.0f) };

//            Assert.Equal(new RectangleF(-50, -200, 50, 200), sprite.GetBoundingRectangle(Vector2.Zero, 0, Vector2.One));
//        }

//        [Fact]
//        public void Sprite_BoundingRectangleAfterScale_Test()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = Substitute.For<Texture2D>(graphicsDevice, 50, 200);
//            var sprite = new Sprite(texture);

//            Assert.Equal(new RectangleF(-50, -200, 100, 400), sprite.GetBoundingRectangle(Vector2.Zero, 0, Vector2.One * 2.0f));
//        }

//        [Fact]
//        public void Sprite_BoundingRectangleAfterRotation_Test()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = Substitute.For<Texture2D>(graphicsDevice, 50, 200);
//            var sprite = new Sprite(texture);

//            AssertExtensions.AreApproximatelyEqual(new RectangleF(-100, -25, 200, 50), sprite.GetBoundingRectangle(Vector2.Zero, MathHelper.ToRadians(90), Vector2.One * 2.0f));
//        }

//        [Fact]
//        public void Sprite_TextureRegionIsFullTextureWhenTextureConstructorIsUsed_Test()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = Substitute.For<Texture2D>(graphicsDevice, 100, 200);
//            var sprite = new Sprite(texture);

//            Assert.Equal(new Rectangle(0, 0, 100, 200), sprite.TextureRegion.Bounds);
//        }

//        [Fact]
//        public void Sprite_DefaultOriginIsCentre_Test()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = Substitute.For<Texture2D>(graphicsDevice, 100, 200);
//            var sprite = new Sprite(texture);
            
//            Assert.Equal(new Vector2(0.5f, 0.5f), sprite.OriginNormalized);
//            Assert.Equal(new Vector2(50, 100), sprite.Origin);
//        }

//        [Fact]
//        public void Sprite_PreserveNormalizedOriginWhenTextureRegionChanges_Test()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = Substitute.For<Texture2D>(graphicsDevice, 100, 100);
//            var textureRegion = new TextureRegion2D(texture, 10, 20, 30, 40);
//            var sprite = new Sprite(textureRegion);

//            Assert.Equal(new Vector2(0.5f, 0.5f), sprite.OriginNormalized);
//            Assert.Equal(new Vector2(15, 20), sprite.Origin);

//            sprite.TextureRegion = new TextureRegion2D(texture, 30, 40, 50, 60);

//            Assert.Equal(new Vector2(0.5f, 0.5f), sprite.OriginNormalized);
//            Assert.Equal(new Vector2(25, 30), sprite.Origin);
//        }
//    }
//}
