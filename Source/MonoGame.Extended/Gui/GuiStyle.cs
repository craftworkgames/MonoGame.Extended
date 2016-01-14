using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui
{
    public class GuiStyle
    {
        public GuiStyle(TextureRegion2D textureRegion)
        {
            TextureRegion = textureRegion;
            Color = Color.White;
            Effect = SpriteEffects.None;
            Rotation = 0;
            Scale = Vector2.One;
        }

        public TextureRegion2D TextureRegion { get; set; }
        public Color Color { get; set; }
        public SpriteEffects Effect { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
    }
}