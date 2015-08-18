namespace MonoGame.Extended.Maps.Tiled
{
    public abstract class TiledLayer
    {
        protected TiledLayer(string name)
        {
            Name = name;
            Properties = new TiledProperties();
        }

        public string Name { get; private set; }
        public TiledProperties Properties { get; private set; }

        public abstract void Draw(Camera2D camera);
    }
}