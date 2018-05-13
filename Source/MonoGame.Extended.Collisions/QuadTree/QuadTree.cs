using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.QuadTree
{
    /// <summary>
    /// Class for doing collision handling with a quad tree.
    /// </summary>
    public class QuadTree
    {
        protected List<QuadTree> Children = new List<QuadTree>();
        protected List<QuadTreeData> Contents = new List<QuadTreeData>();
        protected int CurrentDepth { get; set; }
        protected int MaxDepth { get; set; }
        protected int MaxObjectsPerNode { get; set; }
        protected RectangleF NodeBounds { get; set; }

        /// <summary>
        /// Gets whether the current node is a leaf node.
        /// </summary>
        public bool IsLeaf => Children.Count == 0;

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
        /// Counts the number of targets in the current QuadTree.
        /// </summary>
        /// <returns>Returns the targets of objects found.</returns>
        public int NumTargets()
        {
            Reset();

            int objectCount = Contents.Count;
            for (int i = 0; i < Contents.Count; ++i)
            {
                var data = Contents[i];
                data.Flag = true;
                Contents[i] = data;
            }


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
                    var contents = processing.Contents;
                    for (int i = 0; i < contents.Count; i++)
                    {
                        var obj = contents[i];
                        if (!obj.Flag)
                        {
                            objectCount++;
                            obj.Flag = true;
                            contents[i] = obj;
                        }
                    }
                }

            }

            Reset();
            return objectCount;
        }

        public void Insert(QuadTreeData data)
        {
            var actorBounds = data.Target.BoundingBox;
            if (!NodeBounds.Intersects(actorBounds))
            {
                return; // Object doesn't fit into this node.
            }

            if (IsLeaf&& Contents.Count >= MaxObjectsPerNode)
            {
                Split();
            }

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
                int removeIndex = -1;

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
                    Contents.Remove(data);
                }
            }
            else
            {
                for (int i = 0, size = Children.Count; i < size; i++)
                {
                    Children[i].Remove(data);
                }
            }
            Shake();
        }

        public void Update(QuadTreeData data)
        {
            Remove(data);
            Insert(data);
        }

        public void Reset()
        {
            if (IsLeaf)
            {
                for (int i = 0, size = Contents.Count; i < size; i++)
                {
                    var quadTreeData = Contents[i];
                    quadTreeData.Flag = false;
                    Contents[i] = quadTreeData;
                }
            }
            else
            {
                for (int i = 0, size = Contents.Count; i < size; i++)
                {
                    Children[i].Reset();
                }
            }
        }
        public void Shake()
        {
            if (!IsLeaf)
            {
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
                            for (int i = 0, size = processing.Children.Count; i < size; i++)
                            {
                                process.Enqueue(processing.Children[i]);
                            }
                        }
                        else
                        {
                            Contents.AddRange(processing.Contents);
                        }
                    }
                    Children.Clear();
                }
            }
        }
        public void Split()
        {
            if (CurrentDepth + 1 >= MaxDepth)
            {
                return;
            }

            Point2 min = NodeBounds.TopLeft;
            Point2 max = NodeBounds.BottomRight;
            Point2 center = NodeBounds.Center;

            RectangleF[] childAreas =
            {
                RectangleF.CreateFrom(min, center),
                RectangleF.CreateFrom(new Point2(center.X, min.Y), new Point2(max.X, center.Y)),
                RectangleF.CreateFrom(center, max),
                RectangleF.CreateFrom(new Point2(min.X, center.Y), new Point2(center.X, max.Y)), 
            };

            for (int i = 0; i < 4; ++i)
            {
                var node = new QuadTree(childAreas[i]);
                Children[i].CurrentDepth = CurrentDepth + 1;
            }
            for (int i = 0, size = Contents.Count; i < size; ++i)
            {
                Children[i].Insert(Contents[i]);
            }
            Contents.Clear();
        }

        public List<QuadTreeData> Query(RectangleF area)
        {
            List<QuadTreeData> result = new List<QuadTreeData>();
            if (!RectangleF.Intersects(area, NodeBounds))
            {
                return result;
            }

            if (IsLeaf)
            {
                for (int i = 0, size = Contents.Count; i < size; i++)
                {
                    List<QuadTreeData> recurse = Children[i].Query(area);
                    result.AddRange(recurse);
                }
            }

            return result;
        }
    }
}