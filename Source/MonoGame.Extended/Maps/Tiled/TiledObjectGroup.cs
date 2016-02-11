using System;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledObjectGroup
    {
        public TiledObjectGroup(string name, TiledObject[] objects)
        {
            Name = name;
            Objects = objects;
            Properties = new TiledProperties();
        }

        public string Name { get; private set; }
        public TiledObject[] Objects { get; private set; }
        public TiledProperties Properties { get; private set; }
    }
}
