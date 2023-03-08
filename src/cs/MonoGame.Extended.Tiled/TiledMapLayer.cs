using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapLayer
    {
        public string Name { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 ParallaxFactor { get; set; }
        public TiledMapProperties Properties { get; }

        protected TiledMapLayer(string name, Vector2? offset = null, Vector2? parallaxFactor = null, float opacity = 1.0f, bool isVisible = true)
        {
            Name = name;
            Offset = offset ?? Vector2.Zero;
            ParallaxFactor = parallaxFactor ?? Vector2.One;
            Opacity = opacity;
            IsVisible = isVisible;
            Properties = new TiledMapProperties();
        }
    }
}
