using MonoGame.Extended.Tiled.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapContentItem : ContentItem<TiledMapContent>
    {
        public TiledMapContentItem(TiledMapContent data) 
            : base(data)
        {
        }
    }
}