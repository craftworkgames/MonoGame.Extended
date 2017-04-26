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

        protected override Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            return availableSize;
        }

        public abstract void Layout(IGuiContext context, RectangleF rectangle);

        protected static void PlaceControl(IGuiContext context, GuiControl control, float x, float y, float width, float height)
        {
            var rectangle = new Rectangle((int)x, (int)y, (int)width, (int)height);
            var minimumSize = control.GetDesiredSize(context, new Size2(width, height));
            var destinationRectangle = GuiAlignmentHelper.GetDestinationRectangle(control.HorizontalAlignment, control.VerticalAlignment, minimumSize, rectangle);

            control.Position = new Vector2(destinationRectangle.X + control.Margin.Left, destinationRectangle.Y + control.Margin.Top);
            control.Size = new Size2(destinationRectangle.Width - control.Margin.Left - control.Margin.Right, destinationRectangle.Height - control.Margin.Top - control.Margin.Bottom);

            var layoutControl = control as GuiLayoutControl;
            layoutControl?.Layout(context, new RectangleF(x, y, width, height));
        }
    }
}