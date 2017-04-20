using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiLayoutControl : GuiControl
    {
        protected GuiLayoutControl()
            : this(null)
        {
        }

        protected GuiLayoutControl(TextureRegion2D backgroundRegion) 
            : base(backgroundRegion)
        {
            Origin = Vector2.Zero;
        }

        public abstract void Layout(RectangleF rectangle);

        protected static void PlaceControl(GuiControl control, float x, float y, float width, float height)
        {
            control.Origin = Vector2.Zero;
            control.Position = new Vector2(x + control.Margin.Left, y + control.Margin.Top);
            control.Size = new Size2(width - control.Margin.Left - control.Margin.Right, height - control.Margin.Top - control.Margin.Bottom);

            var layoutControl = control as GuiLayoutControl;
            layoutControl?.Layout(new RectangleF(x, y, width, height));
        }
    }
}