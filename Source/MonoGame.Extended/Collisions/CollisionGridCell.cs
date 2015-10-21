namespace MonoGame.Extended.Collisions
{
    public class CollisionGridCell
    {
        public CollisionGridCell(int column, int row, int data)
        {
            Column = column;
            Row = row;
            Data = data;
            Flag = data == 0 ? CollisionGridCellFlag.Empty : CollisionGridCellFlag.Solid;
        }

        public int Column { get; private set; }
        public int Row { get; private set; }
        public int Data { get; private set; }
        public object Tag { get; set; }
        public CollisionGridCellFlag Flag { get; set; }
    }
}