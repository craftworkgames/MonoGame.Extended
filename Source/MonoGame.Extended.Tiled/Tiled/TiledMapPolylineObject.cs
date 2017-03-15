using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled
{
    public sealed class TiledMapPolylineObject : TiledMapObject
    {
        public Point2[] Points { get; }

        internal TiledMapPolylineObject(ContentReader input)
            : base(input)
        {
            var pointCount = input.ReadInt32();
            Points = new Point2[pointCount];

            for (var i = 0; i < pointCount; i++)
            {
                var x = input.ReadSingle();
                var y = input.ReadSingle();
                Points[i] = new Point2(x, y);
            }
        }
    }
}