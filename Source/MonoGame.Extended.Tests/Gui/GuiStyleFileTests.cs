using System.Collections.Generic;
using System.IO;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Gui
{
    [TestFixture]
    public class GuiStyleFileTests
    {
        [Test]
        public void GuiStyleFile_Load_Test()
        {
            const string dataPath = @"Gui\Data\demo.styles";

            using (var streamReader = new StreamReader(dataPath))
            {
                var guiFile = GuiStyleFile.Load(streamReader);

            }
        }

        [Test]
        public void GuiStyleFile_Dictionary_Test()
        {
            var styles = new Dictionary<string, GuiControlStyle>
            {
                {"my-button", new GuiButtonStyle()}
            };

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                Converters = { new MonoGameColorJsonConverter() }
            }; 
            var json = JsonConvert.SerializeObject(styles, settings);
        }
    }
}