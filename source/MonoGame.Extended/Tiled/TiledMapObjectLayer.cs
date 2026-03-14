using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    [Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
    public class TiledMapObjectLayer : TiledMapLayer
    {
        public TiledMapObjectLayer(string name, string type, TiledMapObject[] objects, Color? color = null, TiledMapObjectDrawOrder drawOrder = TiledMapObjectDrawOrder.TopDown,
            Vector2? offset = null, Vector2? parallaxFactor = null, float opacity = 1.0f, bool isVisible = true)
            : base(name, type, offset, parallaxFactor, opacity, isVisible)
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
