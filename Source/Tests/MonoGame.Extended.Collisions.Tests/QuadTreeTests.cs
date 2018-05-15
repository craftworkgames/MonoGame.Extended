﻿using System.Collections.Generic;
using NUnit.Framework;

namespace MonoGame.Extended.Collisions.Tests
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
        public void NumTargetsManyTest()
        {
            var tree = MakeTree();
            for (int i = 0; i < 1000; i++)
            {
                tree.Insert(new QuadTreeData(new BasicActor()));
                Assert.AreEqual(i + 1, tree.NumTargets());
            }

            Assert.AreEqual(1000, tree.NumTargets());
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
        public void InsertManyTest()
        {
            var tree = MakeTree();

            for (int i = 0; i < 1000; i++)
            {
                tree.Insert(new QuadTreeData(new BasicActor()
                {
                    BoundingBox = new RectangleF(0, 0, 1, 1)
                }));
            }

            Assert.AreEqual(1000, tree.NumTargets());
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

        [Test]
        public void RemoveToEmptyTest()
        {
            var actor = new BasicActor()
            {
                BoundingBox = new RectangleF(-5f, -7f, 10.0f, 15.0f)
            };
            var data = new QuadTreeData(actor);

            var tree = MakeTree();
            tree.Insert(data);

            tree.Remove(data);

            Assert.AreEqual(0, tree.NumTargets());
        }

        [Test]
        public void RemoveTwoTest()
        {
            var tree = MakeTree();
            var inserted = new List<QuadTreeData>();
            var numTargets = 2;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadTreeData(new BasicActor()
                {
                    BoundingBox = new RectangleF(0, 0, 1, 1)
                });
                tree.Insert(data);
                inserted.Add(data);
            }


            var inTree = numTargets;
            Assert.AreEqual(inTree, tree.NumTargets());

            foreach (var data in inserted)
            {
                tree.Remove(data);
                Assert.AreEqual(--inTree, tree.NumTargets());
            }
        }

        [Test]
        public void RemoveThreeTest()
        {
            var tree = MakeTree();
            var inserted = new List<QuadTreeData>();
            var numTargets = 3;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadTreeData(new BasicActor()
                {
                    BoundingBox = new RectangleF(0, 0, 1, 1)
                });
                tree.Insert(data);
                inserted.Add(data);
            }


            var inTree = numTargets;
            Assert.AreEqual(inTree, tree.NumTargets());

            foreach (var data in inserted)
            {
                tree.Remove(data);
                Assert.AreEqual(--inTree, tree.NumTargets());
            }
        }

        [Test]
        public void RemoveManyTest()
        {
            var tree = MakeTree();
            var inserted = new List<QuadTreeData>();
            var numTargets = 1000;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadTreeData(new BasicActor()
                {
                    BoundingBox = new RectangleF(0, 0, 1, 1)
                });
                tree.Insert(data);
                inserted.Add(data);
            }


            var inTree = numTargets;
            Assert.AreEqual(inTree, tree.NumTargets());

            foreach (var data in inserted)
            {
                tree.Remove(data);
                Assert.AreEqual(--inTree, tree.NumTargets());
            }
        }


        [Test]
        public void ShakeWhenEmptyTest()
        {
            var tree = MakeTree();
            tree.Shake();

            Assert.AreEqual(0, tree.NumTargets());
        }

        [Test]
        public void ShakeAfterSplittingTest()
        {
            var tree = MakeTree();

            tree.Split();
            var data = new QuadTreeData(new BasicActor());
            tree.Insert(data);
            tree.Shake();
            Assert.AreEqual(1, tree.NumTargets());
        }

        [Test]
        public void ShakeWhenContainingOneTest()
        {
            var tree = MakeTree();
            var numTargets = 1;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadTreeData(new BasicActor());
                tree.Insert(data);
            }

            tree.Shake();
            Assert.AreEqual(numTargets, tree.NumTargets());
        }

        [Test]
        public void ShakeWhenContainingTwoTest()
        {
            var tree = MakeTree();
            var numTargets = 2;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadTreeData(new BasicActor());
                tree.Insert(data);
            }

            tree.Shake();
            Assert.AreEqual(numTargets, tree.NumTargets());
        }

        [Test]
        public void ShakeWhenContainingThreeTest()
        {
            var tree = MakeTree();
            var numTargets = 3;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadTreeData(new BasicActor());
                tree.Insert(data);
            }

            tree.Shake();
            Assert.AreEqual(numTargets, tree.NumTargets());
        }

        [Test]
        public void ShakeWhenContainingManyTest()
        {
            var tree = MakeTree();
            var numTargets = QuadTree.DefaultMaxObjectsPerNode + 1;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadTreeData(new BasicActor());
                tree.Insert(data);
            }

            tree.Shake();
            Assert.AreEqual(numTargets, tree.NumTargets());
        }
    }
}