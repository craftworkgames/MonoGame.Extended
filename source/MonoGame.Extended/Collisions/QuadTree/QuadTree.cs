using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.QuadTree
{
    /// <summary>
    /// Class for doing collision handling with a quad tree.
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
        /// Contains the children of this node.
        /// </summary>
        protected List<QuadTree> Children = new List<QuadTree>();

        /// <summary>
        /// Contains the data for this node in the quadtree.
        /// </summary>
        protected HashSet<QuadtreeData> Contents = new HashSet<QuadtreeData>();

        /// <summary>
        /// Creates a quad tree with the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds of the new quad tree.</param>
        public QuadTree(RectangleF bounds)
        {
            CurrentDepth = 0;
            NodeBounds = bounds;
        }

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
        /// Gets the bounds of the area contained in this quad tree.
        /// </summary>
        public  RectangleF NodeBounds { get; protected set; }

        /// <summary>
        /// Gets whether the current node is a leaf node.
        /// </summary>
        public bool IsLeaf => Children.Count == 0;

        /// <summary>
        /// Counts the number of unique targets in the current Quadtree.
        /// </summary>
        /// <returns>Returns the targets of objects found.</returns>
        public int NumTargets()
        {
            List<QuadtreeData> dirtyItems = new List<QuadtreeData>();
            var objectCount = 0;

            // Do BFS on nodes to count children.
            var process = new Queue<QuadTree>();
            process.Enqueue(this);
            while (process.Count > 0)
            {
                var processing = process.Dequeue();
                if (!processing.IsLeaf)
                {
                    foreach (var child in processing.Children)
                    {
                        process.Enqueue(child);
                    }
                }
                else
                {
                    foreach (var data in processing.Contents)
                    {
                        if (data.Dirty == false)
                        {
                            objectCount++;
                            data.MarkDirty();
                            dirtyItems.Add(data);
                        }
                    }
                }
            }
            foreach (var quadtreeData in dirtyItems)
            {
                quadtreeData.MarkClean();
            }
            return objectCount;
        }

        /// <summary>
        /// Inserts the data into the tree.
        /// </summary>
        /// <param name="data">Data being inserted.</param>
        public void Insert(QuadtreeData data)
        {
            var actorBounds = data.Bounds;

            // Object doesn't fit into this node.
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
                foreach (var child in Children)
                {
                    child.Insert(data);
                }
            }
        }

        /// <summary>
        /// Removes data from the Quadtree
        /// </summary>
        /// <param name="data">The data to be removed.</param>
        public void Remove(QuadtreeData data)
        {
            if (IsLeaf)
            {
                data.RemoveParent(this);
                Contents.Remove(data);
            }
            else
            {
                throw new InvalidOperationException($"Cannot remove from a non leaf {nameof(QuadTree)}");
            }
        }

        /// <summary>
        /// Removes unnecessary leaf nodes and simplifies the quad tree.
        /// </summary>
        public void Shake()
        {
            if (IsLeaf)
            {
                return;
            }

            List<QuadtreeData> dirtyItems = new List<QuadtreeData>();

            var numObjects = NumTargets();
            if (numObjects == 0)
            {
                Children.Clear();
            }
            else if (numObjects < MaxObjectsPerNode)
            {
                var process = new Queue<QuadTree>();
                process.Enqueue(this);
                while (process.Count > 0)
                {
                    var processing = process.Dequeue();
                    if (!processing.IsLeaf)
                    {
                        foreach (var subTree in processing.Children)
                        {
                            process.Enqueue(subTree);
                        }
                    }
                    else
                    {
                        foreach (var data in processing.Contents)
                        {
                            if (data.Dirty == false)
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

            foreach (var quadtreeData in dirtyItems)
            {
                quadtreeData.MarkClean();
            }
        }

        private void AddToLeaf(QuadtreeData data)
        {
            data.AddParent(this);
            Contents.Add(data);
        }

        /// <summary>
        /// Splits a quadtree into quadrants.
        /// </summary>
        public void Split()
        {
            if (CurrentDepth + 1 >= MaxDepth) return;

            var min = NodeBounds.TopLeft;
            var max = NodeBounds.BottomRight;
            var center = NodeBounds.Center;

            RectangleF[] childAreas =
            {
                RectangleF.CreateFrom(min, center),
                RectangleF.CreateFrom(new Vector2(center.X, min.Y), new Vector2(max.X, center.Y)),
                RectangleF.CreateFrom(center, max),
                RectangleF.CreateFrom(new Vector2(min.X, center.Y), new Vector2(center.X, max.Y))
            };

            for (var i = 0; i < childAreas.Length; ++i)
            {
                var node = new QuadTree(childAreas[i]);
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
        /// Clear current node and all children
        /// </summary>
        public void ClearAll()
        {
            foreach (QuadTree childQuadtree in Children)
                childQuadtree.ClearAll();
            Clear();
        }

        private void Clear()
        {
            foreach (QuadtreeData quadtreeData in Contents)
            {
                quadtreeData.RemoveParent(this);
            }
            Contents.Clear();
        }

        /// <summary>
        /// Queries the quadtree for targets that intersect with the given area.
        /// </summary>
        /// <param name="area">The area to query for overlapping targets</param>
        /// <returns>A unique list of targets intersected by area.</returns>
        public List<QuadtreeData> Query(ref RectangleF area)
        {
            var recursiveResult = new List<QuadtreeData>();
            QueryWithoutReset(ref area, recursiveResult);
            foreach (var quadtreeData in recursiveResult)
            {
                quadtreeData.MarkClean();
            }
            return recursiveResult;
        }

        private void QueryWithoutReset(ref RectangleF area, List<QuadtreeData> recursiveResult)
        {
            if (!NodeBounds.Intersects(area))
                return;

            if (IsLeaf)
            {
                foreach (QuadtreeData quadtreeData in Contents)
                {
                    if (quadtreeData.Dirty == false && quadtreeData.Bounds.Intersects(area))
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
                    Children[i].QueryWithoutReset(ref area, recursiveResult);
                }
            }
        }
    }
}
