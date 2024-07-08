using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapObject
    {
        protected TiledMapObject(int identifier, string name, SizeF size, Vector2 position, float rotation = 0, float opacity = 1, bool isVisible = true, string type = null)
        {
            Identifier = identifier;
            Name = name;
            IsVisible = isVisible;
            Rotation = rotation;
            Position = position;
            Size = size;
            Opacity = opacity;
            Type = type;
            Properties = new TiledMapProperties();
        }

        public int Identifier { get; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float Rotation { get; set; }
        public Vector2 Position { get; }
        public SizeF Size { get; set; }
        public TiledMapProperties Properties { get; }

        public override string ToString()
        {
            return $"{Identifier}";
        }
    }
}
