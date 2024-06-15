using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Gui
{
    public class Cursor
    {
        public Texture2DRegion TextureRegion { get; set; }
        public Color Color { get; set; } = Color.White;
    }
}