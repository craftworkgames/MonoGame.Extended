using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui
{
    public class Cursor
    {
        public TextureRegion2D TextureRegion { get; set; }
        public Color Color { get; set; } = Color.White;
    }
}