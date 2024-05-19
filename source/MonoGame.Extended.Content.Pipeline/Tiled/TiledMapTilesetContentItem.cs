using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Extended.Tiled.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapTilesetContentItem : TiledContentItem<TiledMapTilesetContent>
    {
        public TiledMapTilesetContentItem(TiledMapTilesetContent data)
            : base(data)
        {
        }
    }
}
