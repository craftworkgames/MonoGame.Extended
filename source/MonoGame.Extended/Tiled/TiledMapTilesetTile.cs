using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled
{
    [Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
    [DebuggerDisplay("{LocalTileIdentifier}: Type: {Type}, Properties: {Properties.Count}, Objects: {Objects.Count}")]
    public class TiledMapTilesetTile
    {
        // For remove libraries
        public TiledMapTilesetTile(int localTileIdentifier, string type = null,
            TiledMapObject[] objects = null)
        {
            LocalTileIdentifier = localTileIdentifier;
            Type = type;
            Objects = objects != null ? new List<TiledMapObject>(objects) : new List<TiledMapObject>();
            Properties = new TiledMapProperties();
        }

        public TiledMapTilesetTile(int localTileIdentifier, string type = null,
            TiledMapObject[] objects = null, Texture2D texture = null)
        {
            Texture = texture;
            LocalTileIdentifier = localTileIdentifier;
            Type = type;
            Objects = objects != null ? new List<TiledMapObject>(objects) : new List<TiledMapObject>();
            Properties = new TiledMapProperties();
        }

        public int LocalTileIdentifier { get; }
        public string Type { get; }
        public TiledMapProperties Properties { get; }
        public List<TiledMapObject> Objects { get; }
        public Texture2D Texture { get; }
    }
}
