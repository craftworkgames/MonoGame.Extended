using System.Collections.Generic;
using Xunit;

namespace MonoGame.Extended.Collisions.Tests
{
    public class QuadTreeTests
    {
        private Quadtree MakeTree()
        {
            // Bounds set to ensure actors will fit inside the tree with default bounds.
            var bounds = _quadTreeArea;
            var tree = new Quadtree(bounds);

            return tree;
        }

        private readonly RectangleF _quadTreeArea = new RectangleF(-10f, -15, 20.0f, 30.0f);

        [Fact]
        public void ConstructorTest()
        {
            var bounds = new RectangleF(-10f, -15, 20.0f, 30.0f);
            var tree = new Quadtree(bounds);

            Assert.Equal(bounds, tree.NodeBounds);
            Assert.True(tree.IsLeaf);
        }

        [Fact]
        public void NumTargetsEmptyTest()
        {
            var tree = MakeTree();

            Assert.Equal(0, tree.NumTargets());
        }

        [Fact]
        public void NumTargetsOneTest()
        {
            var tree = MakeTree();
            var actor = new BasicActor();

            tree.Insert(new QuadtreeData(actor));
            
            Assert.Equal(1, tree.NumTargets());
        }


        [Fact]
        public void NumTargetsMultipleTest()
        {
            var tree = MakeTree();
            for (int i = 0; i < 5; i++)
            {
                tree.Insert(new QuadtreeData(new BasicActor()));
            }

            Assert.Equal(5, tree.NumTargets());
        }

        [Fact]
        public void NumTargetsManyTest()
        {
            var tree = MakeTree();
            for (int i = 0; i < 1000; i++)
            {
                tree.Insert(new QuadtreeData(new BasicActor()));
                Assert.Equal(i + 1, tree.NumTargets());
            }

            Assert.Equal(1000, tree.NumTargets());
        }

        [Fact]
        public void InsertOneTest()
        {
            var tree = MakeTree();
            var actor = new BasicActor();

            tree.Insert(new QuadtreeData(actor));

            Assert.Equal(1, tree.NumTargets());
        }

        [Fact]
        public void InsertOneOverlappingQuadrantsTest()
        {
            var tree = MakeTree();
            var actor = new BasicActor
            {
                Bounds = new RectangleF(-2.5f, -2.5f, 5f, 5f)
            };

            tree.Insert(new QuadtreeData(actor));

            Assert.Equal(1, tree.NumTargets());
        }

        [Fact]
        public void InsertMultipleTest()
        {
            var tree = MakeTree();

            for (int i = 0; i < 10; i++)
            {
                tree.Insert(new QuadtreeData(new BasicActor()
                {
                    Bounds = new RectangleF(0, 0, 1, 1)
                }));
            }

            Assert.Equal(10, tree.NumTargets());
        }

        [Fact]
        public void InsertManyTest()
        {
            var tree = MakeTree();

            for (int i = 0; i < 1000; i++)
            {
                tree.Insert(new QuadtreeData(new BasicActor()
                {
                    Bounds = new RectangleF(0, 0, 1, 1)
                }));
            }

            Assert.Equal(1000, tree.NumTargets());
        }

        [Fact]
        public void InsertMultipleOverlappingQuadrantsTest()
        {
            var tree = MakeTree();

            for (int i = 0; i < 10; i++)
            {
                var actor = new BasicActor()
                {
                    Bounds = new RectangleF(-10f, -15, 20.0f, 30.0f)
                };
                tree.Insert(new QuadtreeData(actor));
            }

            Assert.Equal(10, tree.NumTargets());
        }

        [Fact]
        public void RemoveToEmptyTest()
        {
            var actor = new BasicActor()
            {
                Bounds = new RectangleF(-5f, -7f, 10.0f, 15.0f)
            };
            var data = new QuadtreeData(actor);

            var tree = MakeTree();
            tree.Insert(data);

            tree.Remove(data);

            Assert.Equal(0, tree.NumTargets());
        }

        [Fact]
        public void RemoveTwoTest()
        {
            var tree = MakeTree();
            var inserted = new List<QuadtreeData>();
            var numTargets = 2;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadtreeData(new BasicActor()
                {
                    Bounds = new RectangleF(0, 0, 1, 1)
                });
                tree.Insert(data);
                inserted.Add(data);
            }


            var inTree = numTargets;
            Assert.Equal(inTree, tree.NumTargets());

            foreach (var data in inserted)
            {
                tree.Remove(data);
                Assert.Equal(--inTree, tree.NumTargets());
            }
        }

        [Fact]
        public void RemoveThreeTest()
        {
            var tree = MakeTree();
            var inserted = new List<QuadtreeData>();
            var numTargets = 3;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadtreeData(new BasicActor()
                {
                    Bounds = new RectangleF(0, 0, 1, 1)
                });
                tree.Insert(data);
                inserted.Add(data);
            }


            var inTree = numTargets;
            Assert.Equal(inTree, tree.NumTargets());

            foreach (var data in inserted)
            {
                tree.Remove(data);
                Assert.Equal(--inTree, tree.NumTargets());
            }
        }

        [Fact]
        public void RemoveManyTest()
        {
            var tree = MakeTree();
            var inserted = new List<QuadtreeData>();
            var numTargets = 1000;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadtreeData(new BasicActor()
                {
                    Bounds = new RectangleF(0, 0, 1, 1)
                });
                tree.Insert(data);
                inserted.Add(data);
            }


            var inTree = numTargets;
            Assert.Equal(inTree, tree.NumTargets());

            foreach (var data in inserted)
            {
                tree.Remove(data);
                Assert.Equal(--inTree, tree.NumTargets());
            }
        }


        [Fact]
        public void ShakeWhenEmptyTest()
        {
            var tree = MakeTree();
            tree.Shake();

            Assert.Equal(0, tree.NumTargets());
        }

        [Fact]
        public void ShakeAfterSplittingWhenEmptyTest()
        {
            var tree = MakeTree();

            tree.Split();
            tree.Shake();
            Assert.Equal(0, tree.NumTargets());
        }

        [Fact]
        public void ShakeAfterSplittingNotEmptyTest()
        {
            var tree = MakeTree();

            tree.Split();
            var data = new QuadtreeData(new BasicActor());
            tree.Insert(data);
            tree.Shake();
            Assert.Equal(1, tree.NumTargets());
        }

        [Fact]
        public void ShakeWhenContainingOneTest()
        {
            var tree = MakeTree();
            var numTargets = 1;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadtreeData(new BasicActor());
                tree.Insert(data);
            }

            tree.Shake();
            Assert.Equal(numTargets, tree.NumTargets());
        }

        [Fact]
        public void ShakeWhenContainingTwoTest()
        {
            var tree = MakeTree();
            var numTargets = 2;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadtreeData(new BasicActor());
                tree.Insert(data);
            }

            tree.Shake();
            Assert.Equal(numTargets, tree.NumTargets());
        }

        [Fact]
        public void ShakeWhenContainingThreeTest()
        {
            var tree = MakeTree();
            var numTargets = 3;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadtreeData(new BasicActor());
                tree.Insert(data);
            }

            tree.Shake();
            Assert.Equal(numTargets, tree.NumTargets());
        }

        [Fact]
        public void ShakeWhenContainingManyTest()
        {
            var tree = MakeTree();
            var numTargets = Quadtree.DefaultMaxObjectsPerNode + 1;

            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadtreeData(new BasicActor());
                tree.Insert(data);
            }

            tree.Shake();
            Assert.Equal(numTargets, tree.NumTargets());
        }

        [Fact]
        public void QueryWhenEmptyTest()
        {
            var tree = MakeTree();

            var query = tree.Query(_quadTreeArea);
            
            Assert.Empty(query);
            Assert.Equal(0, tree.NumTargets());
        }

        [Fact]
        public void QueryNotOverlappingTest()
        {
            var tree = MakeTree();

            var query = tree.Query(new RectangleF(100f, 100f, 1f, 1f));

            Assert.Empty(query);
            Assert.Equal(0, tree.NumTargets());
        }

        [Fact]
        public void QueryLeafNodeNotEmptyTest()
        {
            var tree = MakeTree();
            var actor = new BasicActor();
            tree.Insert(new QuadtreeData(actor));

            var query = tree.Query(_quadTreeArea);
            Assert.Single(query);
            Assert.Equal(tree.NumTargets(), query.Count);
        }

        [Fact]
        public void QueryLeafNodeNoOverlapTest()
        {
            var tree = MakeTree();
            var actor = new BasicActor();
            tree.Insert(new QuadtreeData(actor));

            var query = tree.Query(new RectangleF(100f, 100f, 1f, 1f));
            Assert.Empty(query);
        }

        [Fact]
        public void QueryLeafNodeMultipleTest()
        {
            var tree = MakeTree();
            var numTargets = Quadtree.DefaultMaxObjectsPerNode;
            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadtreeData(new BasicActor());
                tree.Insert(data);
            }


            var query = tree.Query(_quadTreeArea);
            Assert.Equal(numTargets, query.Count);
            Assert.Equal(tree.NumTargets(), query.Count);
        }

        [Fact]
        public void QueryNonLeafManyTest()
        {
            var tree = MakeTree();
            var numTargets = 2*Quadtree.DefaultMaxObjectsPerNode;
            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadtreeData(new BasicActor());
                tree.Insert(data);
            }


            var query = tree.Query(_quadTreeArea);
            Assert.Equal(numTargets, query.Count);
            Assert.Equal(tree.NumTargets(), query.Count);
        }

        [Fact]
        public void QueryTwiceConsecutiveTest()
        {
            var tree = MakeTree();
            var numTargets = 2 * Quadtree.DefaultMaxObjectsPerNode;
            for (int i = 0; i < numTargets; i++)
            {
                var data = new QuadtreeData(new BasicActor());
                tree.Insert(data);
            }


            var query1 = tree.Query(_quadTreeArea);
            var query2 = tree.Query(_quadTreeArea);
            Assert.Equal(numTargets, query1.Count);
            Assert.Equal(tree.NumTargets(), query1.Count);
            Assert.Equal(query1.Count, query2.Count);
        }
    }
}