using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Gui
{
    public abstract class GuiDrawableControl : GuiControl
    {
        protected GuiDrawableControl(GuiStyle defaultStyle)
        {
            SetStyle(defaultStyle);
        }

        public Sprite Sprite { get; private set; }

        public Vector2 Position
        {
            get { return Sprite.Position; }
            set { Sprite.Position = value; }
        }

        private GuiStyle _currentStyle;

        protected void SetStyle(GuiStyle style)
        {
            if(style == _currentStyle)
                return;

            _currentStyle = style;

            Shape = style.TextureRegion.Bounds.ToRectangleF();

            if (Sprite == null)
                Sprite = new Sprite(style.TextureRegion);
            else
                Sprite.TextureRegion = style.TextureRegion;

            Sprite.Color = style.Color;
            Sprite.Effect = style.Effect;
            Sprite.Rotation = style.Rotation;
            Sprite.Scale = style.Scale;
        }

        public abstract override void Update(GameTime gameTime);
    }
}