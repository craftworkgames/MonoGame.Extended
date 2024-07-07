using System.Collections.Generic;

namespace MonoGame.Extended.Tiled
{
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
