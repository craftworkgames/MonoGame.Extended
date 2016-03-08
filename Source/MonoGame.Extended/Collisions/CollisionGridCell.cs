using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public class CollisionGridCell : ICollidable
    {
        private readonly CollisionGrid _parentGrid;

        public RectangleF BoundingBox => _parentGrid.GetCellRectangle(Column, Row).ToRectangleF();

        public int Column { get; }
        public int Data { get; private set; }
        public CollisionGridCellFlag Flag { get; set; }
        public int Row { get; }
        public object Tag { get; set; }

        public CollisionGridCell(CollisionGrid parentGrid, int column, int row, int data)
        {
            _parentGrid = parentGrid;
            Column = column;
            Row = row;
            Data = data;
            Flag = data == 0 ? CollisionGridCellFlag.Empty : CollisionGridCellFlag.Solid;
        }
    }
}