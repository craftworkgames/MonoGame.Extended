namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledObjectGroup
    {
        public TiledObjectGroup(string name, TiledObject[] objects)
        {
            Name = name;
            Objects = objects;
            Properties = new TiledProperties();
        }

        public string Name { get; }
        public TiledObject[] Objects { get; }
        public TiledProperties Properties { get; }
        public bool IsVisible { get; set; }
    }
}
