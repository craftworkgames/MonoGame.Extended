using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapImageLayer : TiledMapLayer, IMovable
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; }

        internal TiledMapImageLayer(ContentReader input)
            : base(input)
        {
            var textureAssetName = input.GetRelativeAssetName(input.ReadString());
            Texture = input.ContentManager.Load<Texture2D>(textureAssetName);
            var x = input.ReadSingle();
            var y = input.ReadSingle();
            Position = new Vector2(x, y);
        }
    }
}