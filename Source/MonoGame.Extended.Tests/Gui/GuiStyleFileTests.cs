using System.IO;
using System.Linq;
using MonoGame.Extended.Gui;
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
                var styleFile = GuiStyleFile.Load(streamReader);

                Assert.IsNotNull(styleFile);
                Assert.AreEqual(1, styleFile.Styles.Count());
            }
        }
    }
}