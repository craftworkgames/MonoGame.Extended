using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapLayer
    {
        public string Name { get; }
        public string Type { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 ParallaxFactor { get; set; }
        public TiledMapProperties Properties { get; }

        protected TiledMapLayer(string name, string type, Vector2? offset = null, Vector2? parallaxFactor = null, float opacity = 1.0f, bool isVisible = true)
        {
            Name = name;
            Type = type;
            Offset = offset ?? Vector2.Zero;
            ParallaxFactor = parallaxFactor ?? Vector2.One;
            Opacity = opacity;
            IsVisible = isVisible;
            Properties = new TiledMapProperties();
        }
    }
}
