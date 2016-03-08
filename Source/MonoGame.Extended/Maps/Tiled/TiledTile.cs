namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTile
    {
        public int Id { get; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public TiledTile(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}