using MonoGame.Extended.Gui.Controls;
using NSubstitute;
using NUnit.Framework;

namespace MonoGame.Extended.Gui.Tests.Controls
{
    [TestFixture]
    public class GuiControlCollectionTests
    {
        [Test]
        public void GuiControlCollection_Add_SetsTheParent_Test()
        {
            var parent = Substitute.For<GuiControl>();
            var child = Substitute.For<GuiControl>();

            var controls = new GuiControlCollection(parent) { child };
            Assert.IsTrue(controls.Contains(child));
            Assert.AreSame(parent, child.Parent);
        }

        [Test]
        public void GuiControlCollection_Remove_SetsTheParentToNull_Test()
        {
            var parent = Substitute.For<GuiControl>();
            var child = Substitute.For<GuiControl>();

            new GuiControlCollection(parent) { child }.Remove(child);

            Assert.IsNull(child.Parent);
        }

        [Test]
        public void GuiControlCollection_Insert_SetsTheParent_Test()
        {
            var parent = Substitute.For<GuiControl>();
            var child = Substitute.For<GuiControl>();

            var controls = new GuiControlCollection(parent);

            controls.Insert(0, child);
            Assert.IsTrue(controls.Contains(child));
            Assert.AreSame(parent, child.Parent);
        }

        [Test]
        public void GuiControlCollection_Clear_SetsAllTheParentsToNull_Test()
        {
            var parent = Substitute.For<GuiControl>();
            var child0 = Substitute.For<GuiControl>();
            var child1 = Substitute.For<GuiControl>();

            new GuiControlCollection(parent) { child0, child1 }.Clear();

            Assert.IsNull(child0.Parent);
            Assert.IsNull(child1.Parent);
        }
    }
}
