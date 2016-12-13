namespace MonoGame.Extended.Tiled
{
    public class TiledObjectLayer : TiledLayer
    {
        public TiledObject[] Objects { get; }

        public TiledObjectLayer(string name, TiledObject[] objects)
            : base(name)
        {
            Objects = objects;
        }

        public override void Dispose()
        {
        }
    }
}