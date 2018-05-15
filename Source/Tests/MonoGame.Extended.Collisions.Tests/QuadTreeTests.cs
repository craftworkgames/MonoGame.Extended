using System.Runtime.InteropServices;
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
            var bounds = new RectangleF(-10f, -15, 20.0f, 30.0f);
            var tree = new QuadTree(bounds);

            Assert.AreEqual(bounds, tree.NodeBounds);
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

        [Test]
        public void InsertOneTest()
        {
            var tree = MakeTree();
            var actor = new BasicActor();

            tree.Insert(new QuadTreeData(actor));

            Assert.AreEqual(1, tree.NumTargets());
        }

        [Test]
        public void InsertOneOverlappingQuadrantsTest()
        {
            var tree = MakeTree();
            var actor = new BasicActor
            {
                BoundingBox = new RectangleF(-2.5f, -2.5f, 5f, 5f)
            };

            tree.Insert(new QuadTreeData(actor));

            Assert.AreEqual(1, tree.NumTargets());
        }

        [Test]
        public void InsertMultipleTest()
        {
            var tree = MakeTree();

            for (int i = 0; i < 10; i++)
            {
                tree.Insert(new QuadTreeData(new BasicActor()
                {
                    BoundingBox = new RectangleF(0, 0, 1, 1)
                }));
            }

            Assert.AreEqual(10, tree.NumTargets());
        }

        [Test]
        public void InsertMultipleOverlappingQuadrantsTest()
        {
            var tree = MakeTree();

            for (int i = 0; i < 10; i++)
            {
                var actor = new BasicActor()
                {
                    BoundingBox = new RectangleF(-10f, -15, 20.0f, 30.0f)
                };
                tree.Insert(new QuadTreeData(actor));
            }

            Assert.AreEqual(10, tree.NumTargets());
        }
    }
}