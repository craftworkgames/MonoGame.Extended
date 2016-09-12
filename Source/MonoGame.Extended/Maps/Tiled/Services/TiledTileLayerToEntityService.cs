using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Maps.Tiled.Components;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.Maps.Tiled.Services
{
    public class TiledTileLayerToEntityService
    {
        private readonly EntityComponentSystem _entityComponentSystem;
        private readonly TiledMap _tiledMap;

        public TiledTileLayerToEntityService(EntityComponentSystem entityComponentSystem, TiledMap tiledMap)
        {
            _entityComponentSystem = entityComponentSystem;
            _tiledMap = tiledMap;
        }

        public void ConvertLayer(string layerName)
        {
            var layer = _tiledMap.GetLayer<TiledTileLayer>(layerName);
            if (layer == null) throw new ArgumentException("Layer not found");
            foreach (var tile in layer.Tiles)
            {
                if (tile.Id == 0) continue;
                var tileX = tile.X * _tiledMap.TileWidth;
                var tileY = tile.Y * _tiledMap.TileHeight;
                var entity = _entityComponentSystem.CreateEntity(new Vector2(tileX, tileY));

                var tileSprite = GenerateSpriteForTile(tile);
                if (tileSprite != null) entity.AttachComponent(tileSprite);

                entity.AttachComponent(new DepthComponent() { Level = layer.Id });
            }
            layer.IsVisible = false;
        }

        private Sprite GenerateSpriteForTile(TiledTile tile)
        {
            var tileX = tile.X * _tiledMap.TileWidth;
            var tileY = tile.Y * _tiledMap.TileHeight;
            var tileRegion = _tiledMap.GetTileRegion(tile.Id);
            if (tileRegion == null) return null;
            var sprite = new Sprite(tileRegion);
            sprite.OriginNormalized = Vector2.Zero;
            return sprite;
        }
    }
}
