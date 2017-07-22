namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiLayoutControl : GuiControl
    {
        protected GuiLayoutControl()
            : this(null)
        {
        }

        protected GuiLayoutControl(GuiSkin skin) 
            : base(skin)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
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

        protected override void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            if (BackgroundRegion != null)
                renderer.DrawRegion(BackgroundRegion, BoundingRectangle, Color);
        }
    }
}