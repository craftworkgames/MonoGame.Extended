using MonoGame.Extended.Gui.Controls;
using NSubstitute;
using NUnit.Framework;

namespace MonoGame.Extended.Gui.Tests.Controls
{
    [TestFixture]
    public class GuiPanelTests
    {
        [Test]
        public void GuiPanel_CanHaveChildren_Test()
        {
            var panel = new GuiPanel();
            var child = Substitute.For<GuiControl>();

            panel.Controls.Add(child);

            Assert.That(child.Parent, Is.SameAs(panel));
        }
    }
}