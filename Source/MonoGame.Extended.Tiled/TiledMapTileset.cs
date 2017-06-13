using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapTileset
    {
        private readonly Dictionary<int, TiledMapTilesetAnimatedTile> _animatedTilesByLocalTileIdentifier;

        public string Name => Texture.Name;
        public Texture2D Texture { get; }
        public int FirstGlobalIdentifier { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public int Spacing { get; }
        public int Margin { get; }
        public int TileCount { get; }
        public int Columns { get; }
        public ReadOnlyCollection<TiledMapTilesetTile> Tiles { get; private set; }
        public ReadOnlyCollection<TiledMapTilesetAnimatedTile> AnimatedTiles { get; private set; }
        public TiledMapProperties Properties { get; }

        internal TiledMapTileset(ContentReader input)
        {
            var textureAssetName = input.GetRelativeAssetName(input.ReadString());
            var animatedTiles = new List<TiledMapTilesetAnimatedTile>();
            var tiles = new List<TiledMapTilesetTile>();

            _animatedTilesByLocalTileIdentifier = new Dictionary<int, TiledMapTilesetAnimatedTile>();

            Texture = input.ContentManager.Load<Texture2D>(textureAssetName);
            FirstGlobalIdentifier = input.ReadInt32();
            TileWidth = input.ReadInt32();
            TileHeight = input.ReadInt32();
            TileCount = input.ReadInt32();
            Spacing = input.ReadInt32();
            Margin = input.ReadInt32();
            Columns = input.ReadInt32();
            Properties = new TiledMapProperties();

            var explicitTileCount = input.ReadInt32();

            for (var tileIndex = 0; tileIndex < explicitTileCount; tileIndex++)
            {
                var localTileIdentifier = input.ReadInt32();
                var animationFramesCount = input.ReadInt32();

                TiledMapTilesetTile tilesetTile;

                if (animationFramesCount <= 0)
                {
                    tilesetTile = new TiledMapTilesetTile(localTileIdentifier,input);
                }
                else
                {
                    var animatedTilesetTile = new TiledMapTilesetAnimatedTile(this, input, localTileIdentifier, animationFramesCount);
                    animatedTiles.Add(animatedTilesetTile);
                    _animatedTilesByLocalTileIdentifier.Add(localTileIdentifier, animatedTilesetTile);
                    tilesetTile = animatedTilesetTile;
                }

                tiles.Add(tilesetTile);

                input.ReadTiledMapProperties(tilesetTile.Properties);
            }

            input.ReadTiledMapProperties(Properties);

            Tiles = new ReadOnlyCollection<TiledMapTilesetTile>(tiles);
            AnimatedTiles = new ReadOnlyCollection<TiledMapTilesetAnimatedTile>(animatedTiles);
        }

        public Rectangle GetTileRegion(int localTileIdentifier)
        {
            return TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, TileWidth, TileHeight, Columns, Margin, Spacing);
        }

        public TiledMapTilesetAnimatedTile GetAnimatedTilesetTileByLocalTileIdentifier(int localTileIdentifier)
        {
            TiledMapTilesetAnimatedTile animatedTile;
            _animatedTilesByLocalTileIdentifier.TryGetValue(localTileIdentifier, out animatedTile);
            return animatedTile;
        }

        public bool ContainsGlobalIdentifier(int globalIdentifier)
        {
            return globalIdentifier >= FirstGlobalIdentifier && globalIdentifier < FirstGlobalIdentifier + TileCount;
        }
    }
}