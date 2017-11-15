using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapObjectLayer : TiledMapLayer
    {
        public TiledMapObjectLayer(string name, TiledMapObject[] objects, Color? color = null, TiledMapObjectDrawOrder drawOrder = TiledMapObjectDrawOrder.TopDown, 
            Vector2? offset = null, float opacity = 1.0f, bool isVisible = true)
            : base(name, offset, opacity, isVisible)
        {
            Color = color;
            DrawOrder = drawOrder;
            Objects = objects;
        }

        public Color? Color { get; }
        public TiledMapObjectDrawOrder DrawOrder { get; }
        public TiledMapObject[] Objects { get; }
    }
}