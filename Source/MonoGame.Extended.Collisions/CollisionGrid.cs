using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public class CollisionGrid
    {
        private readonly CollisionGridCell[] _data;

        public CollisionGrid(int[] data, int columns, int rows, int cellWidth, int cellHeight)
        {
            _data = new CollisionGridCell[data.Length];

            for (var y = 0; y < rows; y++)
                for (var x = 0; x < columns; x++)
                {
                    var index = x + y*columns;
                    _data[index] = new CollisionGridCell(this, x, y, data[index]);
                }

            Columns = columns;
            Rows = rows;
            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }

        public int Columns { get; }
        public int Rows { get; private set; }
        public int CellWidth { get; }
        public int CellHeight { get; }

        public CollisionGridCell GetCellAtIndex(int column, int row)
        {
            var index = column + row*Columns;

            if ((index < 0) || (index >= _data.Length))
                return new CollisionGridCell(this, column, row, 0);

            return _data[index];
        }

        public CollisionGridCell GetCellAtPosition(Vector3 position)
        {
            var column = (int) (position.X/CellWidth);
            var row = (int) (position.Y/CellHeight);

            return GetCellAtIndex(column, row);
        }

        public IEnumerable<CollisionGridCell> GetCellsOverlappingRectangle(RectangleF rectangle)
        {
            var sx = (int) (rectangle.Left/CellWidth);
            var sy = (int) (rectangle.Top/CellHeight);
            var ex = (int) (rectangle.Right/CellWidth + 1);
            var ey = (int) (rectangle.Bottom/CellHeight + 1);

            for (var y = sy; y < ey; y++)
                for (var x = sx; x < ex; x++)
                    yield return GetCellAtIndex(x, y);
        }

        public IEnumerable<ICollidable> GetCollidables(RectangleF overlappingRectangle)
        {
            return GetCellsOverlappingRectangle(overlappingRectangle)
                .Where(cell => cell.Flag != CollisionGridCellFlag.Empty);
        }

        public Rectangle GetCellRectangle(int column, int row)
        {
            return new Rectangle(column*CellWidth, row*CellHeight, CellWidth, CellHeight);
        }
    }
}