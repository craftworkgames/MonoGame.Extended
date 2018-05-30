using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// Represents a collision grid. This is used to break the game world into
    /// chunks to detect collisions efficiently.
    /// </summary>
    public class CollisionGrid
    {
        private readonly CollisionGridCell[] _data;

        /// <summary>
        /// Creates a new collision grid of specified size.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">Number of columns in the grid.</param>
        /// <param name="rows">Number of rows in the grid.</param>
        /// <param name="cellWidth">The width of each individual cell.</param>
        /// <param name="cellHeight">The height of each individual cell.</param>
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

        /// <summary>
        /// Gets the number of columns in this grid.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// Gets the number of rows in this grid.
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// Gets the width of each cell.
        /// </summary>
        public int CellWidth { get; }

        /// <summary>
        /// Gets the height of each cell.
        /// </summary>
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