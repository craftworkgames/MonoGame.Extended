using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledImageLayer : TiledLayer, IMovable
    {
        public TiledImageLayer(string name, Texture2D texture, Vector2 position)
            : base(name)
        {
            Position = position;
            Texture = texture;
        }

        public Texture2D Texture { get; }

        public Vector2 Position { get; set; }

        public override void Dispose()
        {
        }
    }
}