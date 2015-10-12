namespace Sandbox
{
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

        public byte GetDataAt(int x, int y)
        {
            return _data[x + y * Width];
        }
    }
}