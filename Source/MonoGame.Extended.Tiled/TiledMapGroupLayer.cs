using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapGroupLayer : TiledMapLayer
    {
        public ReadOnlyCollection<TiledMapLayer> Layers { get; }

        internal TiledMapGroupLayer(ContentReader input, TiledMap map, TiledMapGroupLayer parent) 
            : base(input, parent)
        {
            TiledMapReader.ReadLayers(input, map, true, this);
        }
    }
}
