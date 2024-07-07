using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Extended.Content.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled;

public class TiledContentItem<T> : ContentItem<T>
{
    public TiledContentItem(T data) : base(data)
    {
    }

    public void BuildExternalReference<T>(ContentProcessorContext context, TiledMapImageContent image)
    {
        var parameters = new OpaqueDataDictionary
        {
            { "ColorKeyColor", image.TransparentColor },
            { "ColorKeyEnabled", true }
        };
        BuildExternalReference<Texture2DContent>(context, image.Source, parameters);
    }
}
