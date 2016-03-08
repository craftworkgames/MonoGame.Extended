﻿using NSubstitute;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Tests.Sprites
{
    [TestFixture]
    public class SpriteTests
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
        public void Sprite_BoundingRectangleAfterOrigin_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = Substitute.For<Texture2D>(graphicsDevice, 50, 200);
            var sprite = new Sprite(texture)
            {
                OriginNormalized = new Vector2(1.0f, 1.0f)
            };

            Assert.AreEqual(new RectangleF(-50, -200, 50, 200), sprite.GetBoundingRectangle());
        }

        [Test]
        public void Sprite_BoundingRectangleAfterPosition_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = Substitute.For<Texture2D>(graphicsDevice, 50, 200);
            var sprite = new Sprite(texture)
            {
                Position = new Vector2(400, 240)
            };

            Assert.AreEqual(new RectangleF(375, 140, 50, 200), sprite.GetBoundingRectangle());
        }

        [Test]
        public void Sprite_BoundingRectangleAfterRotation_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = Substitute.For<Texture2D>(graphicsDevice, 50, 200);
            var sprite = new Sprite(texture)
            {
                Rotation = MathHelper.ToRadians(90)
            };

            Assert.AreEqual(new RectangleF(-100, -25, 200, 50), sprite.GetBoundingRectangle());
        }

        [Test]
        public void Sprite_BoundingRectangleAfterScale_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = Substitute.For<Texture2D>(graphicsDevice, 50, 200);
            var sprite = new Sprite(texture)
            {
                Scale = Vector2.One * 2.0f
            };

            Assert.AreEqual(new RectangleF(-50, -200, 100, 400), sprite.GetBoundingRectangle());
        }

        [Test]
        public void Sprite_DefaultOriginIsCentre_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = Substitute.For<Texture2D>(graphicsDevice, 100, 200);
            var sprite = new Sprite(texture);

            Assert.AreEqual(new Vector2(0.5f, 0.5f), sprite.OriginNormalized);
            Assert.AreEqual(new Vector2(50, 100), sprite.Origin);
        }

        [Test]
        public void Sprite_PreserveNormalizedOriginWhenTextureRegionChanges_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = Substitute.For<Texture2D>(graphicsDevice, 100, 100);
            var textureRegion = new TextureRegion2D(texture, 10, 20, 30, 40);
            var sprite = new Sprite(textureRegion);

            Assert.AreEqual(new Vector2(0.5f, 0.5f), sprite.OriginNormalized);
            Assert.AreEqual(new Vector2(15, 20), sprite.Origin);

            sprite.TextureRegion = new TextureRegion2D(texture, 30, 40, 50, 60);

            Assert.AreEqual(new Vector2(0.5f, 0.5f), sprite.OriginNormalized);
            Assert.AreEqual(new Vector2(25, 30), sprite.Origin);
        }

        [Test]
        public void Sprite_TextureRegionIsFullTextureWhenTextureConstructorIsUsed_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = Substitute.For<Texture2D>(graphicsDevice, 100, 200);
            var sprite = new Sprite(texture);

            Assert.AreEqual(new Rectangle(0, 0, 100, 200), sprite.TextureRegion.Bounds);
        }
    }
}