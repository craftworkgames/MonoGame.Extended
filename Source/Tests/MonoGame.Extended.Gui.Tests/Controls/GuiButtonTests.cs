//using System.Collections.Generic;
//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.BitmapFonts;
//using MonoGame.Extended.Gui.Controls;
//using MonoGame.Extended.Tests;
//using MonoGame.Extended.TextureAtlases;
//using NSubstitute;
//using Xunit;

//namespace MonoGame.Extended.Gui.Tests.Controls
//{
//    public class GuiButtonTests
//    {
//        [Fact]
//        public void DesiredSizeShouldBeEmptyByDefault()
//        {
//            var availableSize = new Size2(800, 480);
//            var context = Substitute.For<IGuiContext>();
//            var button = new Button();
//            var desiredSize = button.GetDesiredSize(context, availableSize);

//            Assert.That(desiredSize, Is.EqualTo(Size2.Empty));
//        }

//        [Fact]
//        public void DesiredSizeShouldBeTheSizeOfTheBackgroundRegion()
//        {
//            var availableSize = new Size2(800, 480);
//            var context = Substitute.For<IGuiContext>();
//            var backgroundRegion = MockTextureRegion();
//            var button = new GuiButton { BackgroundRegion = backgroundRegion };
//            var desiredSize = button.GetDesiredSize(context, availableSize);

//            Assert.That(desiredSize, Is.EqualTo(backgroundRegion.Size));
//        }

//        [Fact]
//        public void DesiredSizeShouldBeTheSizeOfTheMarginsInANinePatchRegion()
//        {
//            var availableSize = new Size2(800, 480);
//            var context = Substitute.For<IGuiContext>();
//            var texture = new Texture2D(new TestGraphicsDevice(), 512, 512);
//            var backgroundRegion = new NinePatchRegion2D(new TextureRegion2D(texture), new Thickness(10, 20));
//            var button = new GuiButton() { BackgroundRegion = backgroundRegion };
//            var desiredSize = button.GetDesiredSize(context, availableSize);

//            Assert.That(desiredSize, Is.EqualTo(new Size2(20, 40)));
//        }

//        [Fact]
//        public void DesiredSizeShouldAtLeastBeTheSizeOfTheText()
//        {
//            const string text = "abcdefg";

//            var availableSize = new Size2(800, 480);
//            var context = Substitute.For<IGuiContext>();
//            var font = CreateMockFont(text, lineHeight: 32);
//            var expectedSize = font.MeasureString(text);
//            var button = new GuiButton {Text = text, Font = font};

//            var desiredSize = button.GetDesiredSize(context, availableSize);

//            Assert.That(desiredSize, Is.EqualTo(expectedSize));
//        }

//        [Fact]
//        public void DesiredSizeShouldAtLeastBeTheSizeOfTheIcon()
//        {
//            var texture = new Texture2D(new TestGraphicsDevice(), 35, 38);
//            var icon = new TextureRegion2D(texture);

//            var availableSize = new Size2(800, 480);
//            var context = Substitute.For<IGuiContext>();
//            var button = new GuiButton { IconRegion = icon };
//            var desiredSize = button.GetDesiredSize(context, availableSize);

//            Assert.That(desiredSize, Is.EqualTo(icon.Size));
//        }

//        [Fact]
//        public void DesiredSizeShouldBeTheSizeOfTheBiggestTextOrIcon()
//        {
//            const string text = "abcdefg";

//            var texture = new Texture2D(new TestGraphicsDevice(), 35, 38);
//            var icon = new TextureRegion2D(texture);
//            var iconExpectedSize = icon.Size;

//            var availableSize = new Size2(800, 480);
//            var context = Substitute.For<IGuiContext>();

//            var font = CreateMockFont(text, 32);
//            var fontExpectedSize = font.MeasureString(text);

//            var button = new GuiButton { Text = text, Font = font, IconRegion = icon };
//            var desiredSize = button.GetDesiredSize(context, availableSize);

//            Assert.That(desiredSize.Width, Is.EqualTo(fontExpectedSize.Width));
//            Assert.That(desiredSize.Height, Is.EqualTo(iconExpectedSize.Height));
//        }

//        private static BitmapFont CreateMockFont(string text, int lineHeight)
//        {
//            var regions = new List<BitmapFontRegion>();
//            var xOffset = 0;

//            foreach (var character in text)
//            {
//                regions.Add(new BitmapFontRegion(MockTextureRegion(10, 10), character, xOffset, yOffset: 0, xAdvance: 0));
//                xOffset += 10;
//            }

//            return new BitmapFont("font", regions, lineHeight);
//        }

//        private static TextureRegion2D MockTextureRegion(int width = 100, int height = 200)
//        {
//            var texture = new Texture2D(new TestGraphicsDevice(), width, height);
//            return new TextureRegion2D(texture, 0, 0, width, height);
//        }
//    }
//}