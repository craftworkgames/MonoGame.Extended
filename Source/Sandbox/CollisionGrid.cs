using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Sandbox
{
    public interface ICollidable
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        Rectangle GetAxisAlignedBoundingBox();
    }

    public class CollisionGrid
    {
        public CollisionGrid(byte[] data, int width, int height, int cellWidth, int cellHeight)
        {
            _data = data;

            Width = width;
            Height = height;
            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }

        private readonly byte[] _data;

        public byte GetDataAtIndex(int x, int y)
        {
            var index = x + y * Width;

            if (index < 0 || index >= _data.Length)
                return 0;

            return _data[index];
        }

        public Rectangle GetCellRectangle(int x, int y)
        {
            return new Rectangle(x * CellWidth, y * CellHeight, CellWidth, CellHeight);
        }

        public IEnumerable<CollisionInfo> CollidesWith(ICollidable collidable)
        {
            var boundingBox = collidable.GetAxisAlignedBoundingBox();
            var sx = boundingBox.Left / CellWidth;
            var sy = boundingBox.Top / CellHeight;
            var ex = (boundingBox.Right / CellWidth) + 1;
            var ey = (boundingBox.Bottom / CellHeight) + 1;
            var collisions = new List<CollisionInfo>();

            for (var y = sy; y < ey; y++)
            {
                for (var x = sx; x < ex; x++)
                {
                    if (GetDataAtIndex(x, y) != 0)
                    {
                        var cellRectangle = GetCellRectangle(x, y);
                        var intersectingRectangle = Rectangle.Intersect(cellRectangle, boundingBox);

                        if(!intersectingRectangle.IsEmpty)
                        {
                            var w = intersectingRectangle.Width;
                            var h = intersectingRectangle.Height;

                            if (w < h)
                            {
                                collidable.Position = new Vector2((float)Math.Round(collidable.Position.X - w, 0), collidable.Position.Y);
                                collidable.Velocity = new Vector2(0, collidable.Velocity.Y);
                            }
                            else
                            {
                                collidable.Position = new Vector2(collidable.Position.X, (float)Math.Round(collidable.Position.Y - h));
                                collidable.Velocity = new Vector2(collidable.Velocity.X, 0);
                            }
                        }
                    }
                }
            }

            if (collisions.Count > 1)
                return collisions;

            return collisions;
        }
    }

    public class CollisionInfo
    {
        public CollisionInfo(ICollidable collidable, Vector2 collisionPoint)
        {
            Collidable = collidable;
            CollisionPoint = collisionPoint;
        }

        public ICollidable Collidable { get; private set; }
        public Vector2 CollisionPoint { get; set; }
    }
}