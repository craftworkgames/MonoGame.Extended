using System;

namespace MonoGame.Extended.Maps.Tiled
{
    public abstract class TiledLayer : IDisposable
    {
        protected TiledLayer(string name)
        {
            Name = name;
            Properties = new TiledProperties();
            IsVisible = true;
            Opacity = 1.0f;
        }

        public string Name { get; }
        public float Depth { get; set; }
        public TiledProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }

        public abstract void Dispose();
    }
}