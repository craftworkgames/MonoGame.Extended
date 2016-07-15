using System.IO;
using MonoGame.Extended.Gui;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Gui
{
    [TestFixture]
    public class GuiFileTests
    {
        [Test]
        public void GuiFile_Load_Test()
        {
            const string dataPath = @"Gui\Data\three-buttons.gui";

            using (var streamReader = new StreamReader(dataPath))
            {
                var guiFile = GuiFile.Load(streamReader);

                Assert.AreEqual("demo.styles", guiFile.StyleSheet);
                Assert.AreEqual(3, guiFile.Controls.Count);
            }
        }
    }
}
