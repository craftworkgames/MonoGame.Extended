using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapObject
    {
        public int Identifier { get; }
        public string Name { get; set; }
        public string Type { get; }
        public TiledMapProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float Rotation { get; set; }
        public Vector2 Position { get; }
        public Size2 Size { get; set; }

        internal TiledMapObject(ContentReader input)
        {
            Identifier = input.ReadInt32();
            Name = input.ReadString();
            Type = input.ReadString();
            Position = new Vector2(input.ReadSingle(), input.ReadSingle());
            var width = input.ReadSingle();
            var height = input.ReadSingle();
            Size = new Size2(width, height);
            Rotation = input.ReadSingle();
            IsVisible = input.ReadBoolean();

            Properties = new TiledMapProperties();
            input.ReadTiledMapProperties(Properties);
        }

        public override string ToString()
        {
            return $"{Identifier}";
        }
    }
}