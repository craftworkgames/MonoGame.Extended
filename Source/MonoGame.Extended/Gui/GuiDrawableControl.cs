using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Gui
{
    public abstract class GuiDrawableControl : GuiControl
    {
        protected GuiDrawableControl()
        {
        }

        public abstract GuiControlStyle CurrentStyle { get; }

        private IShapeF _shape;
        public override IShapeF Shape
        {
            get { return _shape ?? CurrentStyle.BoundingShape; }
            set { _shape = value; }
        }

        public Vector2 Position { get; set; }

        public abstract override void Update(GameTime gameTime);

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentStyle.Draw(this, spriteBatch);
        }
    }
}