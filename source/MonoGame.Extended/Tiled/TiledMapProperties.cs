using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Tiled
{
    [Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
    public class TiledMapProperties : Dictionary<string, TiledMapPropertyValue>
    {
        public bool TryGetValue(string key, out string value)
        {
            bool result = TryGetValue(key, out TiledMapPropertyValue tmpVal);
            value = result ? tmpVal.Value : null;
            return result;
        }
    }
}
