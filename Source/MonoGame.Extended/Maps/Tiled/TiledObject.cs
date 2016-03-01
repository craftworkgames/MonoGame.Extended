namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledObject
    {
        public TiledObject(int id, int x, int y, int width, int height)
        {
            Id = id;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Properties = new TiledProperties();
        }

        public int Id { get; }
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        public TiledProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}
