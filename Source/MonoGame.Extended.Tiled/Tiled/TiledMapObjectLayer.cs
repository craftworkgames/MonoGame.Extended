using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapObjectLayer : TiledMapLayer
    {
        public Color Color { get; }
        public TiledMapObjectDrawOrder DrawOrder { get; }
        public TiledMapObject[] Objects { get; }

        internal TiledMapObjectLayer(ContentReader input, TiledMap map)
            : base(input)
        {
            Color = input.ReadColor();
            DrawOrder = (TiledMapObjectDrawOrder)input.ReadByte();

            var objectCount = input.ReadInt32();

            Objects = new TiledMapObject[objectCount];

            for (var i = 0; i < objectCount; i++)
            {
                TiledMapObject @object;

                var objectType = (TiledMapObjectType)input.ReadByte();
                switch (objectType)
                {
                    case TiledMapObjectType.Rectangle:
                        @object = new TiledMapRectangleObject(input);
                        break;
                    case TiledMapObjectType.Tile:
                        @object = new TiledMapTileObject(input, map);
                        break;
                    case TiledMapObjectType.Ellipse:
                        @object = new TiledMapEllipseObject(input);
                        break;
                    case TiledMapObjectType.Polygon:
                        @object = new TiledMapPolygonObject(input);
                        break;
                    case TiledMapObjectType.Polyline:
                        @object = new TiledMapPolylineObject(input);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Objects[i] = @object;
            }
        }
    }
}