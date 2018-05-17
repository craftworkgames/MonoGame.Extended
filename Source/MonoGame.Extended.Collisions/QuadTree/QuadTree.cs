﻿using System.Collections.Generic;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    ///     Class for doing collision handling with a quad tree.
    /// </summary>
    public class QuadTree
    {
        public const int DefaultMaxDepth = 7;
        public const int DefaultMaxObjectsPerNode = 25;

        protected List<QuadTree> Children = new List<QuadTree>();
        protected List<QuadTreeData> Contents = new List<QuadTreeData>();

        /// <summary>
        ///     Creates a quad tree with the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds of the new quad tree.</param>
        public QuadTree(RectangleF bounds)
        {
            CurrentDepth = 0;
            NodeBounds = bounds;
        }

        protected int CurrentDepth { get; set; }
        protected int MaxDepth { get; set; } = DefaultMaxDepth;

        protected int MaxObjectsPerNode { get; set; } = DefaultMaxObjectsPerNode;

        /// <summary>
        /// Gets the bounds of the area contained in this quad tree.
        /// </summary>
        public  RectangleF NodeBounds { get; protected set; }

        /// <summary>
        ///     Gets whether the current node is a leaf node.
        /// </summary>
        public bool IsLeaf => Children.Count == 0;

        /// <summary>
        ///     Counts the number of unique targets in the current QuadTree.
        /// </summary>
        /// <returns>Returns the targets of objects found.</returns>
        public int NumTargets()
        {
            Reset();

            var objectCount = 0;

            // Do BFS on nodes to count children.
            var process = new Queue<QuadTree>();
            process.Enqueue(this);
            while (process.Count > 0)
            {
                var processing = process.Dequeue();
                if (!processing.IsLeaf)
                {
                    foreach (var child in processing.Children) process.Enqueue(child);
                }
                else
                {
                    var contents = processing.Contents;
                    foreach (var data in contents)
                    {
                        if (!data.Flag)
                        {
                            objectCount++;
                            data.Flag = true;
                        }
                    }
                }
            }

            Reset();
            return objectCount;
        }

        /// <summary>
        /// Inserts the data into the tree.
        /// </summary>
        /// <param name="data">Data being inserted.</param>
        public void Insert(QuadTreeData data)
        {
            var actorBounds = data.Target.BoundingBox;
            
            // Object doesn't fit into this node.
            if (!RectangleF.Intersects(NodeBounds, actorBounds))
            {
                return;
            }

            if (IsLeaf && Contents.Count >= MaxObjectsPerNode) Split();

            if (IsLeaf)
            {
                Contents.Add(data);
            }
            else
            {
                foreach (var child in Children)
                {
                    child.Insert(data);
                }
            }
        }

        public void Remove(QuadTreeData data)
        {
            if (IsLeaf)
            {
                var removeIndex = -1;

                for (int i = 0, size = Contents.Count; i < size; i++)
                {
                    if (Contents[i].Target == data.Target)
                    {
                        removeIndex = i;
                        break;
                    }
                }

                if (removeIndex != -1)
                {
                    Contents.RemoveAt(removeIndex);
                }
            }
            else
            {
                foreach (var quadTree in Children)
                {
                    quadTree.Remove(data);
                }
            }

            Shake();
        }

        public void Update(QuadTreeData data)
        {
            Remove(data);
            Insert(data);
        }

        /// <summary>
        /// Prepare the quad tree 
        /// </summary>
        public void Reset()
        {
            if (IsLeaf)
                for (int i = 0, size = Contents.Count; i < size; i++)
                {
                    var quadTreeData = Contents[i];
                    quadTreeData.Flag = false;
                    Contents[i] = quadTreeData;
                }
            else
            {
                for (int i = 0, size = Children.Count; i < size; i++)
                    Children[i].Reset();
            }
        }

        /// <summary>
        /// Removes unneccesary leaf nodes and simplifies the quad tree.
        /// </summary>
        public void Shake()
        {
            if (!IsLeaf)
            {
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
                            foreach (var subTree in processing.Children)
                            {
                                process.Enqueue(subTree);
                            }
                        else
                        {
                            foreach (var data in processing.Contents)
                            {
                                if (!data.Flag)
                                {
                                    Contents.Add(data);
                                    data.Flag = true;
                                }
                            }
                        }
                    }

                    Children.Clear();
                }
            }
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
                RectangleF.CreateFrom(new Point2(center.X, min.Y), new Point2(max.X, center.Y)),
                RectangleF.CreateFrom(center, max),
                RectangleF.CreateFrom(new Point2(min.X, center.Y), new Point2(center.X, max.Y))
            };

            for (var i = 0; i < childAreas.Length; ++i)
            {
                var node = new QuadTree(childAreas[i]);
                Children.Add(node);
                Children[i].CurrentDepth = CurrentDepth + 1;
            }

            for (int i = 0, size = Contents.Count; i < size; ++i)
            {
                for (int j = 0; j < Children.Count; j++)
                {
                    Children[j].Insert(Contents[i]);
                }
            }

            Contents.Clear();
        }

        /// <summary>
        /// Queries the quadtree for targets that intersect with the given area.
        /// </summary>
        /// <param name="area">The area to query for overlapping targets</param>
        /// <returns>A unique list of targets intersected by area.</returns>
        public List<QuadTreeData> Query(RectangleF area)
        {
            Reset();
            return QueryWithoutReset(area);
        }

        private List<QuadTreeData> QueryWithoutReset(RectangleF area)
        {
            var result = new List<QuadTreeData>();

            if (!RectangleF.Intersects(area, NodeBounds)) return result;

            if (IsLeaf)
            {
                for (int i = 0, size = Contents.Count; i < size; i++)
                {
                    if (RectangleF.Intersects(Contents[i].BoundingBox, area) 
                        && !Contents[i].Flag)
                    {
                        result.Add(Contents[i]);
                        Contents[i].Flag = true;
                    }
                }
            }
            else
            {
                for (int i = 0, size = Children.Count; i < size; i++)
                {
                    var recurse = Children[i].QueryWithoutReset(area);
                    result.AddRange(recurse);
                }
            }


            return result;
        }
    }
}