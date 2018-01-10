using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input.InputListeners;
using NUnit.Framework;

namespace MonoGame.Extended.NuclexGui.Tests
{
    [TestFixture]
    public class GuiManagerTests
    {
        [Test]
        public void SampleGuiManagerTest()
        {
            var gameServices = new GameServiceContainer();
            var guiInputService = new GuiInputService(new InputListenerComponent(null));
            var gui = new GuiManager(gameServices, guiInputService);

            gui.Update(new GameTime());
        }
    }
}