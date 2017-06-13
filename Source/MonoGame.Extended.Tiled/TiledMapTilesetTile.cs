using Microsoft.Xna.Framework.Content;
using System;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapTilesetTile
    {
        internal TiledMapTilesetTile(int localTileIdentifier, ContentReader input)
        {
            var objectCount = input.ReadInt32();

            var objects = new TiledMapObject[objectCount];

            for (var i = 0; i < objectCount; i++)
            {
                TiledMapObject @object;

                var objectType = (TiledMapObjectType)input.ReadByte();
                switch (objectType)
                {
                    case TiledMapObjectType.Rectangle:
                        @object = new TiledMapRectangleObject(input);
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

                objects[i] = @object;
            }

            Objects = objects;
            LocalTileIdentifier = localTileIdentifier;
            Properties = new TiledMapProperties();
        }

        public int LocalTileIdentifier { get; set; }
        public TiledMapProperties Properties { get; private set; }
        public TiledMapObject[] Objects { get; set; }
    }
}