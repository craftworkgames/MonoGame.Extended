using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class LayoutControl : ItemsControl
    {
        protected LayoutControl()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            BackgroundColor = Color.Transparent;
        }

        private bool _isLayoutValid;

        public override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
            _isLayoutValid = false;
        }

        public override void Update(IGuiContext context, float deltaSeconds)
        {
            base.Update(context, deltaSeconds);

            if (!_isLayoutValid)
            {
                Layout(context, new Rectangle(Padding.Left, Padding.Top, ContentRectangle.Width, ContentRectangle.Height));
                _isLayoutValid = true;
            }
        }

        protected abstract void Layout(IGuiContext context, Rectangle rectangle);

        protected static void PlaceControl(IGuiContext context, Control control, float x, float y, float width, float height)
        {
            LayoutHelper.PlaceControl(context, control, x, y, width, height);
        }
    }
}