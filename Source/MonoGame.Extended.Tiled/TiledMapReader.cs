using System;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tiled.Graphics;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapReader : ContentTypeReader<TiledMap>
    {
        protected override TiledMap Read(ContentReader input, TiledMap existingInstance)
        {
            if (existingInstance != null)
                return existingInstance;
            ;

            var value = ReadMetaData(input);
            ReadTilesets(input, value);
            ReadLayers(input, value);

            return value;
        }

        private static TiledMap ReadMetaData(ContentReader input)
        {
            var name = input.AssetName;
            var width = input.ReadInt32();
            var height = input.ReadInt32();
            var tileWidth = input.ReadInt32();
            var tileHeight = input.ReadInt32();
            var backgroundColor = input.ReadColor();
            var renderOrder = (TiledMapTileDrawOrder)input.ReadByte();
            var orientation = (TiledMapOrientation)input.ReadByte();

            var map = new TiledMap(name, width, height, tileWidth, tileHeight, renderOrder, orientation)
            {
                BackgroundColor = backgroundColor
            };

            input.ReadTiledMapProperties(map.Properties);

            return map;
        }

        private static void ReadTilesets(ContentReader input, TiledMap map)
        {
            var tilesetCount = input.ReadInt32();

            for (var i = 0; i < tilesetCount; i++)
            {
                var tileset = new TiledMapTileset(input);
                map.AddTileset(tileset);
            }
        }

        internal static void ReadLayers(ContentReader input, TiledMap map, bool nested = false, TiledMapGroupLayer parent = null)
        {
            var layerCount = input.ReadInt32();

            for (var i = 0; i < layerCount; i++)
            {
                TiledMapLayer layer;

                var layerType = (TiledMapLayerType)input.ReadByte();
                switch (layerType)
                {
                    case TiledMapLayerType.ImageLayer:
                        layer = new TiledMapImageLayer(input, parent);
                        break;
                    case TiledMapLayerType.TileLayer:
                        layer = new TiledMapTileLayer(input, map, parent);
                        break;
                    case TiledMapLayerType.ObjectLayer:
                        layer = new TiledMapObjectLayer(input, map, parent);
                        break;
                    case TiledMapLayerType.GroupLayer:
                        layer = new TiledMapGroupLayer(input, map, parent);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (layerType != TiledMapLayerType.ObjectLayer)
                    ReadModels(input, layer, map);

                map.AddLayer(layer, nested);
            }
        }

        private static void ReadModels(ContentReader input, TiledMapLayer layer, TiledMap map)
        {
            var modelCount = input.ReadInt32();
            var animatedModelCount = input.ReadInt32();

            var models = layer.Models = new TiledMapLayerModel[modelCount];
            var animatedModels = layer.AnimatedModels = new TiledMapLayerAnimatedModel[animatedModelCount];

            var animatedModelIndex = 0;

            for (var modelIndex = 0; modelIndex < modelCount; modelIndex++)
            {
                var isAnimated = input.ReadBoolean();
                var model = isAnimated ? new TiledMapLayerAnimatedModel(input, map) : new TiledMapLayerModel(input);

                models[modelIndex] = model;
                if (isAnimated)
                    animatedModels[animatedModelIndex++] = (TiledMapLayerAnimatedModel)model;
            }
        }
    }
}
