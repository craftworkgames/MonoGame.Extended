using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Sandbox
{
    public interface ICollidable
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        bool Contains(Vector2 position);
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

            if (index > _data.Length)
                return 0;

            return _data[index];
        }

        public Rectangle GetCellRectangle(int x, int y)
        {
            return new Rectangle(x * CellWidth, y * CellHeight, CellWidth, CellHeight);
        }

        public IEnumerable<CollisionInfo> CollidesWith(ICollidable collidable)
        {
            var collisions = new List<CollisionInfo>();

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    if (GetDataAtIndex(x, y) != 0)
                    {
                        var rectangle = GetCellRectangle(x, y);

                        if (rectangle.Contains(collidable.Position))
                        {
                            var collisionPoint = collidable.Position - collidable.Velocity;
                            var collision = new CollisionInfo(collidable, collisionPoint);

                            collisions.Add(collision);
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