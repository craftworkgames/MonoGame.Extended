namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledObjectLayer : TiledLayer
    {
        public TiledObjectLayer(string name, TiledObject[] objects) 
            : base(name)
        {
            Objects = objects;
        }

        public override void Dispose()
        {
        }

        public TiledObject[] Objects { get; }
    }
}