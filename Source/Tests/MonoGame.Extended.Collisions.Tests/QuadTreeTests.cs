using MonoGame.Extended.Collisions.QuadTree;
using NUnit.Framework;

namespace MonoGame.Extended.Gui.Tests
{
    [TestFixture]
    public class QuadTreeTests
    {
        private QuadTree MakeTree()
        {
            var bounds = new RectangleF(0, 0, 20.0f, 30.0f);
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
            tree.Insert(new QuadTreeData(new BasicActor()));

            Assert.AreEqual(1, tree.NumTargets());
        }


        [Test]
        public void NumTargetsMultipleTest()
        {
            var tree = MakeTree();
            tree.Insert(new QuadTreeData(new BasicActor()));
            tree.Insert(new QuadTreeData(new BasicActor()));
            tree.Insert(new QuadTreeData(new BasicActor()));
            tree.Insert(new QuadTreeData(new BasicActor()));
            tree.Insert(new QuadTreeData(new BasicActor()));

            Assert.AreEqual(1, tree.NumTargets());
        }
    }
}