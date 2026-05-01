using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.QuadTree
{
    /// <summary>
    /// Stores collision actors in a hierarchical quadtree for broadphase overlap queries.
    /// </summary>
    public class QuadTree
    {
        /// <summary>
        /// The default maximum depth.
        /// </summary>
        public const int DefaultMaxDepth = 7;

        /// <summary>
        /// The default maximum objects per node.
        /// </summary>
        public const int DefaultMaxObjectsPerNode = 25;

        /// <summary>
        /// Contains the child nodes of this node.
        /// </summary>
        protected List<QuadTree> Children = new List<QuadTree>();

        /// <summary>
        /// Contains the quadtree data stored directly in this node.
        /// </summary>
        protected HashSet<QuadtreeData> Contents = new HashSet<QuadtreeData>();

        /// <summary>
        /// Gets or sets the current depth for this node in the quadtree.
        /// </summary>
        protected int CurrentDepth { get; set; }

        /// <summary>
        /// Gets or sets the maximum depth of the quadtree.
        /// </summary>
        protected int MaxDepth { get; set; } = DefaultMaxDepth;

        /// <summary>
        /// Gets or sets the maximum objects per node in this quadtree.
        /// </summary>
        protected int MaxObjectsPerNode { get; set; } = DefaultMaxObjectsPerNode;

        /// <summary>
        /// Gets the bounds of the area covered by this quadtree node.
        /// </summary>
        public BoundingBox2D NodeBounds { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the current node has no child nodes.
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                return Children.Count == 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTree"/> class.
        /// </summary>
        /// <param name="bounds">The bounds covered by this quadtree node.</param>
        public QuadTree(BoundingBox2D bounds)
        {
            CurrentDepth = 0;
            NodeBounds = bounds;
        }

        /// <summary>
        /// Counts the number of distinct actors stored beneath this node.
        /// </summary>
        /// <returns>The number of unique actors stored beneath this node.</returns>
        public int NumTargets()
        {
            List<QuadtreeData> dirtyItems = new List<QuadtreeData>();
            int objectCount = 0;
            Queue<QuadTree> process = new Queue<QuadTree>();
            process.Enqueue(this);

            while (process.Count > 0)
            {
                QuadTree processing = process.Dequeue();

                if (!processing.IsLeaf)
                {
                    foreach (QuadTree child in processing.Children)
                    {
                        process.Enqueue(child);
                    }
                }
                else
                {
                    foreach (QuadtreeData data in processing.Contents)
                    {
                        if (!data.Dirty)
                        {
                            objectCount++;
                            data.MarkDirty();
                            dirtyItems.Add(data);
                        }
                    }
                }
            }

            foreach (QuadtreeData quadtreeData in dirtyItems)
            {
                quadtreeData.MarkClean();
            }

            return objectCount;
        }

        /// <summary>
        /// Inserts the specified quadtree data into this node or one of its descendants.
        /// </summary>
        /// <param name="data">The quadtree data to insert.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
        public void Insert(QuadtreeData data)
        {
            ArgumentNullException.ThrowIfNull(data);

            BoundingBox2D actorBounds = data.Bounds;

            if (!NodeBounds.Intersects(actorBounds))
            {
                return;
            }

            if (IsLeaf && Contents.Count >= MaxObjectsPerNode)
            {
                Split();
            }

            if (IsLeaf)
            {
                AddToLeaf(data);
            }
            else
            {
                foreach (QuadTree child in Children)
                {
                    child.Insert(data);
                }
            }
        }

        /// <summary>
        /// Removes the specified quadtree data from this node.
        /// </summary>
        /// <param name="data">The quadtree data to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The current node is not a leaf node.</exception>
        public void Remove(QuadtreeData data)
        {
            ArgumentNullException.ThrowIfNull(data);

            if (IsLeaf)
            {
                data.RemoveParent(this);
                Contents.Remove(data);
            }
            else
            {
                throw new InvalidOperationException($"Cannot remove from a non-leaf {nameof(QuadTree)}.");
            }
        }

        /// <summary>
        /// Removes unnecessary leaf nodes and simplifies the quadtree.
        /// </summary>
        public void Shake()
        {
            if (IsLeaf)
            {
                return;
            }

            List<QuadtreeData> dirtyItems = new List<QuadtreeData>();
            int numObjects = NumTargets();

            if (numObjects == 0)
            {
                Children.Clear();
            }
            else if (numObjects < MaxObjectsPerNode)
            {
                Queue<QuadTree> process = new Queue<QuadTree>();
                process.Enqueue(this);

                while (process.Count > 0)
                {
                    QuadTree processing = process.Dequeue();

                    if (!processing.IsLeaf)
                    {
                        foreach (QuadTree subTree in processing.Children)
                        {
                            process.Enqueue(subTree);
                        }
                    }
                    else
                    {
                        foreach (QuadtreeData data in processing.Contents)
                        {
                            if (!data.Dirty)
                            {
                                AddToLeaf(data);
                                data.MarkDirty();
                                dirtyItems.Add(data);
                            }
                        }
                    }
                }

                Children.Clear();
            }

            foreach (QuadtreeData quadtreeData in dirtyItems)
            {
                quadtreeData.MarkClean();
            }
        }

        /// <summary>
        /// Splits the current leaf node into four child quadrants when additional depth is available.
        /// </summary>
        public void Split()
        {
            if (CurrentDepth + 1 >= MaxDepth)
            {
                return;
            }

            Vector2 min = NodeBounds.Min;
            Vector2 max = NodeBounds.Max;
            Vector2 center = NodeBounds.Center;

            BoundingBox2D[] childAreas =
            {
                new BoundingBox2D(min, center),
                new BoundingBox2D(new Vector2(center.X, min.Y), new Vector2(max.X, center.Y)),
                new BoundingBox2D(center, max),
                new BoundingBox2D(new Vector2(min.X, center.Y), new Vector2(center.X, max.Y))
            };

            for (int i = 0; i < childAreas.Length; ++i)
            {
                QuadTree node = new QuadTree(childAreas[i]);
                Children.Add(node);
                Children[i].CurrentDepth = CurrentDepth + 1;
            }

            foreach (QuadtreeData contentQuadtree in Contents)
            {
                foreach (QuadTree childQuadtree in Children)
                {
                    childQuadtree.Insert(contentQuadtree);
                }
            }

            Clear();
        }

        /// <summary>
        /// Removes all contents from this node and every descendant node.
        /// </summary>
        public void ClearAll()
        {
            foreach (QuadTree childQuadtree in Children)
            {
                childQuadtree.ClearAll();
            }

            Clear();
        }

        /// <summary>
        /// Queries the quadtree for targets that intersect with the given area.
        /// </summary>
        /// <param name="area">The area to query for overlapping targets.</param>
        /// <returns>A unique list of targets intersected by <paramref name="area"/>.</returns>
        public List<QuadtreeData> Query(BoundingBox2D area)
        {
            List<QuadtreeData> recursiveResult = new List<QuadtreeData>();
            QueryWithoutReset(area, recursiveResult);

            foreach (QuadtreeData quadtreeData in recursiveResult)
            {
                quadtreeData.MarkClean();
            }

            return recursiveResult;
        }

        private void AddToLeaf(QuadtreeData data)
        {
            data.AddParent(this);
            Contents.Add(data);
        }

        private void Clear()
        {
            foreach (QuadtreeData quadtreeData in Contents)
            {
                quadtreeData.RemoveParent(this);
            }

            Contents.Clear();
        }

        private void QueryWithoutReset(BoundingBox2D area, List<QuadtreeData> recursiveResult)
        {
            if (!NodeBounds.Intersects(area))
            {
                return;
            }

            if (IsLeaf)
            {
                foreach (QuadtreeData quadtreeData in Contents)
                {
                    if (!quadtreeData.Dirty && quadtreeData.Bounds.Intersects(area))
                    {
                        recursiveResult.Add(quadtreeData);
                        quadtreeData.MarkDirty();
                    }
                }
            }
            else
            {
                for (int i = 0, size = Children.Count; i < size; i++)
                {
                    Children[i].QueryWithoutReset(area, recursiveResult);
                }
            }
        }
    }
}
