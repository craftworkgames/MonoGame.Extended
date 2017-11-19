using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    public class TiledMapModelBuilder
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly TiledMap _map;

        public TiledMapModelBuilder(GraphicsDevice graphicsDevice, TiledMap map)
        {
            _graphicsDevice = graphicsDevice;
            _map = map;
        }

        public TiledMapModel Build()
        {
            var layerModels = new List<TiledMapLayerModel>();
            var staticLayerBuilder = new TiledMapStaticLayerModelBuilder();
            var animatedLayerBuilder = new TiledMapAnimatedLayerModelBuilder();

            foreach (var tileLayer in _map.Layers.OfType<TiledMapTileLayer>())
            {
                foreach (var tileset in _map.Tilesets)
                {
                    var texture = tileset.Texture;

                    foreach (var tile in tileLayer.Tiles.Where(t => tileset.ContainsGlobalIdentifier(t.GlobalIdentifier)))
                    {
                        var tileGid = tile.GlobalIdentifier;
                        var localTileIdentifier = tileGid - tileset.FirstGlobalIdentifier;
                        var position = GetTilePosition(_map, tile);
                        var tilesetColumns = tileset.Columns == 0 ? 1 : tileset.Columns; // fixes a problem (what problem exactly?)
                        var sourceRectangle = TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, tileset.TileWidth, tileset.TileHeight, tilesetColumns, tileset.Margin, tileset.Spacing);
                        var flipFlags = tile.Flags;

                        // animated tiles
                        var tilesetTile = tileset.Tiles.FirstOrDefault(x => x.LocalTileIdentifier == localTileIdentifier);
                        var animatedTilesetTile = tilesetTile as TiledMapTilesetAnimatedTile;

                        if (animatedTilesetTile != null)
                        {
                            animatedLayerBuilder.AddTile(texture, position, sourceRectangle, flipFlags);
                            animatedLayerBuilder.AnimatedTilesetTiles.Add(animatedTilesetTile);

                            if (animatedLayerBuilder.IsFull)
                                layerModels.Add(animatedLayerBuilder.Build(_graphicsDevice, texture));
                        }
                        else
                        {
                            staticLayerBuilder.AddTile(texture, position, sourceRectangle, flipFlags);

                            if (staticLayerBuilder.IsFull)
                                layerModels.Add(staticLayerBuilder.Build(_graphicsDevice, texture));
                        }
                    }

                    if (staticLayerBuilder.IsBuildable)
                        layerModels.Add(staticLayerBuilder.Build(_graphicsDevice, texture));

                    if (animatedLayerBuilder.IsBuildable)
                        layerModels.Add(animatedLayerBuilder.Build(_graphicsDevice, texture));
                }
            }

            return new TiledMapModel(layerModels.ToArray());
        }

        private static Point2 GetTilePosition(TiledMap map, TiledMapTile mapTile)
        {
            switch (map.Orientation)
            {
                case TiledMapOrientation.Orthogonal:
                    return TiledMapHelper.GetOrthogonalPosition(mapTile.X, mapTile.Y, map.TileWidth, map.TileHeight);
                case TiledMapOrientation.Isometric:
                    return TiledMapHelper.GetIsometricPosition(mapTile.X, mapTile.Y, map.TileWidth, map.TileHeight);
                case TiledMapOrientation.Staggered:
                    throw new NotImplementedException("Staggered maps are not yet implemented.");
                default:
                    throw new NotSupportedException($"Tiled Map {map.Orientation} is not supported.");
            }
        }
    }
}