using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.TextureAtlases;
using NSubstitute;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.TextureAtlases
{
    [TestFixture]
    public class TextureAtlasKenneyXmlTests
    {
        [Test]
        public void TextureAtlas_LoadFromKenneyXml_Test()
        {
            const string path = @"D:\Github\Public\MonoGame.Extended\Source\Demos\Demo.Gui\Content\kenney-gui-blue-atlas.xml";
            var serviceProvider = Substitute.For<IServiceProvider>();
            var contentManager = new ContentManager(serviceProvider);

            using (var stream = File.OpenRead(path))
            {
                var textureAtlas = TextureAtlasReader.FromRawXml(contentManager, stream);
            }
        }
    }
}