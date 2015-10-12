namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTile
    {
        public TiledTile (int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public int Id { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}", Id);
        }
    }
}