using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions.QuadTree;
using NUnit.Framework;

namespace MonoGame.Extended.Gui.Tests
{
    [TestFixture]
    public class QuadTreeTests
    {
        private QuadTree MakeTree()
        {
            // Bounds set to ensure actors will fit inside the tree with default bounds.
            var bounds = new RectangleF(-10f, -15, 20.0f, 30.0f);
            var tree = new QuadTree(bounds);

            return tree;
        }

        [Test]
        public void ConstructorTest()
        {
            var tree = MakeTree();

            Assert.AreEqual(true, tree.IsLeaf);
        }

        [Test]
        public void NumTargetsEmptyTest()
        {
            var tree = MakeTree();

            Assert.AreEqual(0, tree.NumTargets());
        }

        [Test]
        public void NumTargetsOneTest()
        {
            var tree = MakeTree();
            var actor = new BasicActor();

            tree.Insert(new QuadTreeData(actor));

            Assert.AreEqual(1, tree.NumTargets());
        }


        [Test]
        public void NumTargetsMultipleTest()
        {
            var tree = MakeTree();
            for (int i = 0; i < 5; i++)
            {
                tree.Insert(new QuadTreeData(new BasicActor()));
            }

            Assert.AreEqual(5, tree.NumTargets());
        }
    }
}