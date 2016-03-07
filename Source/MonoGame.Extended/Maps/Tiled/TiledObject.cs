namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledObject
    {
        public TiledObject(int id, float x, float y, float width, float height)
        {
            Id = id;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Properties = new TiledProperties();
        }

        public int Id { get; }
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }
        public TiledProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}
