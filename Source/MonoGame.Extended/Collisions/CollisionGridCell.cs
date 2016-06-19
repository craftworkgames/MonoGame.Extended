using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public class CollisionGridCell : ICollidable
    {
        public CollisionGridCell(CollisionGrid parentGrid, int column, int row, int data)
        {
            _parentGrid = parentGrid;
            Column = column;
            Row = row;
            Data = data;
            Flag = data == 0 ? CollisionGridCellFlag.Empty : CollisionGridCellFlag.Solid;
        }

        private readonly CollisionGrid _parentGrid;

        public int Column { get; }
        public int Row { get; }
        public int Data { get; private set; }
        public object Tag { get; set; }
        public CollisionGridCellFlag Flag { get; set; }

        public RectangleF BoundingBox => _parentGrid.GetCellRectangle(Column, Row).ToRectangleF();
    }
}