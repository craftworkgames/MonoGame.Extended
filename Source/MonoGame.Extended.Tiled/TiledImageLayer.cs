using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled
{
    public class TiledImageLayer : TiledLayer, IMovable
    {
        public Texture2D Texture { get; }

        public TiledImageLayer(string name, Texture2D texture, Vector2 position)
            : base(name)
        {
            Position = position;
            Texture = texture;
        }

        public Vector2 Position { get; set; }

        public override void Dispose()
        {
        }
    }
}