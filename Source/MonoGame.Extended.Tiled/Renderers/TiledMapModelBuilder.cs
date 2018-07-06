﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    public class TiledMapModelBuilder
    {
        private readonly GraphicsDevice _graphicsDevice;

        public TiledMapModelBuilder(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        private IEnumerable<TiledMapLayerModel> CreateLayerModels(TiledMap map, TiledMapLayer layer)
        {
			switch(layer)
			{
				case TiledMapTileLayer tileLayer:
					return CreateTileLayerModels(map, tileLayer);
				case TiledMapImageLayer imageLayer:
					return CreateImageLayerModels(imageLayer);
				default:
					return new List<TiledMapLayerModel>();
			}

        }

        private IEnumerable<TiledMapLayerModel> CreateImageLayerModels(TiledMapImageLayer imageLayer)
        {
            var modelBuilder = new TiledMapStaticLayerModelBuilder();
            modelBuilder.AddSprite(imageLayer.Image, imageLayer.Position, imageLayer.Image.Bounds, TiledMapTileFlipFlags.None);
            yield return modelBuilder.Build(_graphicsDevice, imageLayer.Image);
        }

        private IEnumerable<TiledMapLayerModel> CreateTileLayerModels(TiledMap map, TiledMapTileLayer tileLayer)
        {
            var layerModels = new List<TiledMapLayerModel>();
            var staticLayerBuilder = new TiledMapStaticLayerModelBuilder();
            var animatedLayerBuilder = new TiledMapAnimatedLayerModelBuilder();

            foreach (var tileset in map.Tilesets)
            {
				var firstGlobalIdentifier = map.GetTilesetFirstGlobalIdentifier(tileset);
				var lastGlobalIdentifier = tileset.TileCount + firstGlobalIdentifier - 1;
                var texture = tileset.Texture;

                foreach (var tile in tileLayer.Tiles.Where(t => firstGlobalIdentifier <= t.GlobalIdentifier && t.GlobalIdentifier <= lastGlobalIdentifier))
                {
                    var tileGid = tile.GlobalIdentifier;
                    var localTileIdentifier = tileGid - firstGlobalIdentifier;
                    var position = GetTilePosition(map, tile);
                    var tilesetColumns = tileset.Columns == 0 ? 1 : tileset.Columns; // fixes a problem (what problem exactly?)
                    var sourceRectangle = TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, tileset.TileWidth, tileset.TileHeight, tilesetColumns, tileset.Margin, tileset.Spacing);
                    var flipFlags = tile.Flags;

                    // animated tiles
                    var tilesetTile = tileset.Tiles.FirstOrDefault(x => x.LocalTileIdentifier == localTileIdentifier);
                    var animatedTilesetTile = tilesetTile as TiledMapTilesetAnimatedTile;

                    if (animatedTilesetTile != null)
                    {
                        animatedLayerBuilder.AddSprite(texture, position, sourceRectangle, flipFlags);
                        animatedLayerBuilder.AnimatedTilesetTiles.Add(animatedTilesetTile);

                        if (animatedLayerBuilder.IsFull)
                            layerModels.Add(animatedLayerBuilder.Build(_graphicsDevice, texture));
                    }
                    else
                    {
                        staticLayerBuilder.AddSprite(texture, position, sourceRectangle, flipFlags);

                        if (staticLayerBuilder.IsFull)
                            layerModels.Add(staticLayerBuilder.Build(_graphicsDevice, texture));
                    }
                }

                if (staticLayerBuilder.IsBuildable)
                    layerModels.Add(staticLayerBuilder.Build(_graphicsDevice, texture));

                if (animatedLayerBuilder.IsBuildable)
                    layerModels.Add(animatedLayerBuilder.Build(_graphicsDevice, texture));
            }

            return layerModels;
        }

        public TiledMapModel Build(TiledMap map)
        {
			var dictionary = new Dictionary<TiledMapLayer, TiledMapLayerModel[]>();
			foreach (var layer in map.Layers)
				BuildLayer(map, layer, dictionary);

            return new TiledMapModel(map, dictionary);
        }

		private void BuildLayer(TiledMap map, TiledMapLayer layer, Dictionary<TiledMapLayer, TiledMapLayerModel[]> dictionary)
		{
			if (layer is TiledMapGroupLayer groupLayer)
				foreach (var subLayer in groupLayer.Layers)
					BuildLayer(map, subLayer, dictionary);
			else
				dictionary.Add(layer, CreateLayerModels(map, layer).ToArray());
		}

		private static Point2 GetTilePosition(TiledMap map, TiledMapTile mapTile)
        {
            switch (map.Orientation)
            {
                case TiledMapOrientation.Orthogonal:
                    return TiledMapHelper.GetOrthogonalPosition(mapTile.X, mapTile.Y, map.TileWidth, map.TileHeight);
                case TiledMapOrientation.Isometric:
                    return TiledMapHelper.GetIsometricPosition(mapTile.X, mapTile.Y, map.TileWidth, map.TileHeight);
                default:
                    throw new NotSupportedException($"{map.Orientation} Tiled Maps are not yet implemented.");
            }
        }
    }
}