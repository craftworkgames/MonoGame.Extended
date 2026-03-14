using System;
using MonoGame.Extended.Content.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
    public class TiledMapTilesetContentItem : TiledContentItem<TiledMapTilesetContent>
    {
        public TiledMapTilesetContentItem(TiledMapTilesetContent data)
            : base(data)
        {
        }
    }
}
