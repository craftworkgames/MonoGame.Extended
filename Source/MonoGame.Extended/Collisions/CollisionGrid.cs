using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public class CollisionGrid
    {
        public CollisionGrid(int[] data, int columns, int rows, int cellWidth, int cellHeight)
        {
            _data = new CollisionGridCell[data.Length];

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < columns; x++)
                {
                    var index = x + y * columns;
                    _data[index] = new CollisionGridCell(x, y, data[index]);
                }
            }

            Columns = columns;
            Rows = rows;
            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }

        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }

        private readonly CollisionGridCell[] _data;

        public CollisionGridCell GetCellAtIndex(int column, int row)
        {
            var index = column + row * Columns;

            if (index < 0 || index >= _data.Length)
                return new CollisionGridCell(column, row, 0);

            return _data[index];
        }

        public CollisionGridCell GetCellAtPosition(Vector3 position)
        {
            var column = (int)(position.X / CellWidth);
            var row = (int)(position.Y / CellHeight);

            return GetCellAtIndex(column, row);
        }

        public IEnumerable<CollisionGridCell> GetCellsOverlappingRectangle(RectangleF rectangle)
        {
            var sx = (int) (rectangle.Left/CellWidth);
            var sy = (int) (rectangle.Top/CellHeight);
            var ex = (int) ((rectangle.Right/CellWidth) + 1);
            var ey = (int) ((rectangle.Bottom/CellHeight) + 1);

            for (var y = sy; y < ey; y++)
            {
                for (var x = sx; x < ex; x++)
                    yield return GetCellAtIndex(x, y);
            }
        }

        public Rectangle GetCellRectangle(int column, int row)
        {
            return new Rectangle(column * CellWidth, row * CellHeight, CellWidth, CellHeight);
        }

        public void CollidesWith(RectangleF boundingBox, Action<CollisionInfo> onCollision)
        {
            foreach (var cell in GetCellsOverlappingRectangle(boundingBox))
            {
                if (cell.Flag == CollisionGridCellFlag.Empty)
                    continue;
                
                var cellRectangle = GetCellRectangle(cell.Column, cell.Row);
                var intersectingRectangle = RectangleF.Intersect(cellRectangle.ToRectangleF(), boundingBox);

                if (intersectingRectangle.IsEmpty)
                    continue;

                var collisionInfo = new CollisionInfo
                {
                    Row = cell.Row,
                    Column = cell.Column,
                    IntersectingRectangle = intersectingRectangle,
                    CellRectangle = cellRectangle
                };

                onCollision(collisionInfo);
            }
        }
    }
}