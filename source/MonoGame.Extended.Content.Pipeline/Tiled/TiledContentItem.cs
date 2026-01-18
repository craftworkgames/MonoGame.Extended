using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Extended.Content.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled;

#if MONOGAME_385_OR_NEWER && !KNI && !FNA

/// <summary>
/// Base class for Tiled content items.
/// </summary>
/// <typeparam name="T">The type of Tiled content data.</typeparam>
public class TiledContentItem<T> : ContentItem<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TiledContentItem{T}"/> class.
    /// </summary>
    /// <param name="data">The content data.</param>
    public TiledContentItem(T data) : base(data)
    {
    }
}

#else

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

#endif
