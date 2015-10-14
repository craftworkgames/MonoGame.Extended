using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public class CollisionGrid
    {
        public CollisionGrid(byte[] data, int columns, int rows, int cellWidth, int cellHeight)
        {
            _data = data;

            Columns = columns;
            Rows = rows;
            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }

        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }

        private readonly byte[] _data;

        public byte GetDataAt(int column, int row)
        {
            var index = column + row * Columns;

            if (index < 0 || index >= _data.Length)
                return 0;

            return _data[index];
        }

        public Rectangle GetCellRectangle(int column, int row)
        {
            return new Rectangle(column * CellWidth, row * CellHeight, CellWidth, CellHeight);
        }

        public void CollidesWith(RectangleF boundingBox, Action<CollisionInfo> onCollision)
        {
            var sx = (int)(boundingBox.Left / CellWidth);
            var sy = (int)(boundingBox.Top / CellHeight);
            var ex = (int)((boundingBox.Right / CellWidth) + 1);
            var ey = (int)((boundingBox.Bottom / CellHeight) + 1);

            for (var y = sy; y < ey; y++)
            {
                for (var x = sx; x < ex; x++)
                {
                    if (GetDataAt(x, y) == 0)
                        continue;

                    var cellRectangle = GetCellRectangle(x, y);
                    var intersectingRectangle = RectangleF.Intersect(cellRectangle.ToRectangleF(), boundingBox);

                    if (intersectingRectangle.IsEmpty)
                        continue;

                    var collisionInfo = new CollisionInfo
                    {
                        Row = y,
                        Column = x,
                        IntersectingRectangle = intersectingRectangle,
                        CellRectangle = cellRectangle
                    };

                    onCollision(collisionInfo);
                }
            }
        }
    }

    public class CollisionInfo
    {
        public int Column { get; internal set; }
        public int Row { get; internal set; }
        public RectangleF IntersectingRectangle { get; internal set; }
        public Rectangle CellRectangle { get; internal set; }
    }
}