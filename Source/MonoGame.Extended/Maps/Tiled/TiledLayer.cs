using System;

namespace MonoGame.Extended.Maps.Tiled
{
    public abstract class TiledLayer : IDisposable
    {
        protected TiledLayer(string name)
        {
            Name = name;
            Properties = new TiledProperties();
        }

        public abstract void Dispose();

        public string Name { get; private set; }
        public TiledProperties Properties { get; private set; }

        public abstract void Draw(Camera2D camera);
    }
}