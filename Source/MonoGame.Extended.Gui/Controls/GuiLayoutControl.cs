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
            var rectangle = new Rectangle((int)x, (int)y, (int)width, (int)height);
            var minimumSize = control.GetMinimumSize(new Size2(width, height));
            var finalRect = GuiAlignmentHelper.GetDestinationRectangle(control.HorizontalAlignment, control.VerticalAlignment, minimumSize, rectangle);
            control.Origin = Vector2.Zero;
            control.Position = new Vector2(finalRect.X + control.Margin.Left, finalRect.Y + control.Margin.Top);
            control.Size = new Size2(finalRect.Width - control.Margin.Left - control.Margin.Right, finalRect.Height - control.Margin.Top - control.Margin.Bottom);

            var layoutControl = control as GuiLayoutControl;
            layoutControl?.Layout(new RectangleF(x, y, width, height));
        }
    }
}