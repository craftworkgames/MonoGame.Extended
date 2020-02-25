using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    ///     Class for doing collision handling with a quad tree.
    /// </summary>
    public class Quadtree
    {
        public const int DefaultMaxDepth = 7;
        public const int DefaultMaxObjectsPerNode = 25;

        protected List<Quadtree> Children = new List<Quadtree>(4);
        protected HashSet<QuadTreeData> Contents = new HashSet<QuadTreeData>();

        /// <summary>
        ///     Creates a quad tree with the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds of the new quad tree.</param>
        public Quadtree(RectangleF bounds)
        {
            CurrentDepth = 0;
            NodeBounds = bounds;
        }

        protected int CurrentDepth { get; set; }
        protected int MaxDepth { get; set; } = DefaultMaxDepth;

        protected int MaxObjectsPerNode { get; set; } = DefaultMaxObjectsPerNode;

        /// <summary>
        /// Layers that are currently in the QuadTree
        /// </summary>
        public int LayerMask;

        /// <summary>
        ///     Gets the bounds of the area contained in this quad tree.
        /// </summary>
        public  RectangleF NodeBounds { get; protected set; }

        /// <summary>
        ///     Gets whether the current node is a leaf node.
        /// </summary>
        public bool IsLeaf => Children.Count == 0;

        public void UpdateLayerMask()
        {
            LayerMask = 0;
            foreach (QuadTreeData quadTreeData in Contents)
            {
                LayerMask |= quadTreeData.CollisionLayerFlags;
            }
        }

        /// <summary>
        ///     Counts the number of unique targets in the current Quadtree.
        /// </summary>
        /// <param name="max">Max amount of items to count</param>
        /// <returns>Returns the targets of objects found.</returns>
        public int NumTargets(int max = -1)
        {
            List<QuadTreeData> dirtyItems = new List<QuadTreeData>();
            var objectCount = 0;

            // Do BFS on nodes to count children.
            var process = new Queue<Quadtree>();
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

                        if (max > 0 && objectCount >= max)
                        {
                            break;
                        }
                    }
                    if (max > 0 && objectCount >= max)
                    {
                        break;
                    }
                }
            }

            for (var i = 0; i < dirtyItems.Count; i++)
            {
                dirtyItems[i].MarkClean();
            }
            return objectCount;
        }

        /// <summary>
        ///     Inserts the data into the tree.
        /// </summary>
        /// <param name="data">Data being inserted.</param>
        public void Insert(QuadTreeData data)
        {
            // Object doesn't fit into this node.
            if (!NodeBounds.Intersects(data.Target.Bounds))
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
                for (var index = 0; index < 4; index++)
                {
                    Children[index].Insert(data);
                }
            }
        }

        /// <summary>
        ///     Removes data from the QuadTree
        /// </summary>
        /// <param name="data">The data to be removed.</param>
        public void Remove(QuadTreeData data)
        {
            if (IsLeaf)
            {
                data.RemoveParent(this);
                Contents.Remove(data);
                UpdateLayerMask();
            }
            else
            {
                throw new InvalidOperationException($"Cannot remove from a non leaf {nameof(Quadtree)}"); 
            }
        }

        /// <summary>
        ///     Removes unnecessary leaf nodes and simplifies the quad tree.
        /// </summary>
        public void Shake()
        {
            if (!IsLeaf)
            {
                var numObjects = NumTargets(MaxObjectsPerNode);
                if (numObjects == 0)
                {
                    Children.Clear();
                }
                else if (numObjects < MaxObjectsPerNode)
                {
                    List<QuadTreeData> dirtyItems = new List<QuadTreeData>();
                    var process = new Queue<Quadtree>();
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
                                data.Parents.Remove(processing);
                                if (data.Dirty == false)
                                {
                                    AddToLeaf(data);
                                    data.MarkDirty();
                                    dirtyItems.Add(data);
                                }
                            }
                        }
                    }
                    for (var i = 0; i < dirtyItems.Count; i++)
                    {
                        dirtyItems[i].MarkClean();
                    }
                    Children.Clear();
                }
                else
                {
                    for (var index = 0; index < Children.Count; index++)
                    {
                        Quadtree quadTree = Children[index];
                        quadTree.Shake();
                    }
                }
            }
        }

        private void AddToLeaf(QuadTreeData data)
        {
            data.AddParent(this);
            Contents.Add(data);
            LayerMask |= data.CollisionLayerFlags;
        }

        /// <summary>
        ///     Splits a quadtree into quadrants.
        /// </summary>
        public void Split()
        {
            if (CurrentDepth + 1 >= MaxDepth)
                return;

            var min = NodeBounds.TopLeft;
            var max = NodeBounds.BottomRight;
            var center = NodeBounds.Center;

            RectangleF[] childAreas =
            {
                RectangleF.CreateFrom(min, center),
                RectangleF.CreateFrom(new Point2(center.X, min.Y), new Point2(max.X, center.Y)),
                RectangleF.CreateFrom(center, max),
                RectangleF.CreateFrom(new Point2(min.X, center.Y), new Point2(center.X, max.Y))
            };

            for (var i = 0; i < childAreas.Length; ++i)
            {
                var node = new Quadtree(childAreas[i]);
                Children.Add(node);
                Children[i].CurrentDepth = CurrentDepth + 1;
            }

            foreach (QuadTreeData quadTreeData in Contents)
            {
                quadTreeData.RemoveParent(this);
                for (var index = 0; index < 4; index++)
                {
                    Children[index].Insert(quadTreeData);
                }
            }
            Contents.Clear();
        }

        private void Clear()
        {
            foreach (QuadTreeData quadTreeData in Contents)
            {
                quadTreeData.RemoveParent(this);
            }
            Contents.Clear();
        }

        public List<QuadTreeData> Query(QuadTreeData area)
        {
            var collisions = new List<QuadTreeData>();
            QueryWithoutReset(area, collisions);
            for (var i = 0; i < collisions.Count; i++)
            {
                collisions[i].MarkClean();
            }
            return collisions;
        }

        private void QueryWithoutReset(QuadTreeData queryQuadTreeData, List<QuadTreeData> recursiveResult)
        {
            if (queryQuadTreeData.CollisionMaskFlags == 0 || !NodeBounds.Intersects(queryQuadTreeData.Target.Bounds))
                return;

            if (IsLeaf)
            {
                // Check if this quad contains items with target layer
                if ((queryQuadTreeData.CollisionMaskFlags & LayerMask) == 0)
                    return;

                foreach (QuadTreeData quadTreeData in Contents)
                {
                    if (quadTreeData.Dirty == false
                        && (queryQuadTreeData.CollisionMaskFlags & quadTreeData.CollisionLayerFlags) != 0
                        && quadTreeData.Bounds.Intersects(queryQuadTreeData.Target.Bounds))
                    {
                        recursiveResult.Add(quadTreeData);
                        quadTreeData.MarkDirty();
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Children[i].QueryWithoutReset(queryQuadTreeData, recursiveResult);
                }
            }
        }
    }
}