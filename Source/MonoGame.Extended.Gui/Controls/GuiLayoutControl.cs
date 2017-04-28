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
            GuiLayoutHelper.PlaceControl(context, control, x, y, width, height);
        }
    }
}