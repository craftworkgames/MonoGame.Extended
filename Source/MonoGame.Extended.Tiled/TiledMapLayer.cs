using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapLayer
    {
        public string Name { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public Vector2 Offset { get; set; }
        public TiledMapProperties Properties { get; }

        protected TiledMapLayer(string name, Vector2? offset = null, float opacity = 1.0f, bool isVisible = true)
        {
            Name = name;
            Offset = offset ?? Vector2.Zero;
            Opacity = opacity;
            IsVisible = isVisible;
            Properties = new TiledMapProperties();
        }
    }
}