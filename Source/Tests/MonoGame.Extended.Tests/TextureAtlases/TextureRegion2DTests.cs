//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.TextureAtlases;
//using Xunit;

//namespace MonoGame.Extended.Tests.TextureAtlases
//{
//    
//    public class TextureRegion2DTests
//    {
//        [Fact]
//        public void TextureRegion2D_FromTexture_Test()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = new Texture2D(graphicsDevice, 100, 200);
//            var textureRegion = new TextureRegion2D(texture);

//            Assert.AreSame(texture, textureRegion.Texture);
//            Assert.Equal(0, textureRegion.X);
//            Assert.Equal(0, textureRegion.Y);
//            Assert.Equal(100, textureRegion.Width);
//            Assert.Equal(200, textureRegion.Height);
//            Assert.IsNull(textureRegion.Tag);
//        }

//        [Fact]
//        public void TextureRegion2D_Specified_Test()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var texture = new Texture2D(graphicsDevice, 100, 200);
//            var textureRegion = new TextureRegion2D(texture, 10, 20, 30, 40);

//            Assert.AreSame(texture, textureRegion.Texture);
//            Assert.Equal(10, textureRegion.X);
//            Assert.Equal(20, textureRegion.Y);
//            Assert.Equal(30, textureRegion.Width);
//            Assert.Equal(40, textureRegion.Height);
//            Assert.IsNull(textureRegion.Tag);
//        }
//    }
//}