using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    [Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
    public sealed class TiledMapPolygonObject : TiledMapObject
    {
        public TiledMapPolygonObject(int identifier, string name, Vector2[] points, SizeF size, Vector2 position, float rotation = 0, float opacity = 1, bool isVisible = true, string type = null)
            : base(identifier, name, size, position, rotation, opacity, isVisible, type)
        {
            Points = points;
        }

        public Vector2[] Points { get; }
    }
}
