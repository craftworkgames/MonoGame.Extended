using System.Linq;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiDialog : GuiLayoutControl
    {
        public GuiDialog()
            : this(null)
        {
        }

        public GuiDialog(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
            HorizontalAlignment = HorizontalAlignment.Centre;
            VerticalAlignment = VerticalAlignment.Centre;
        }

        public Thickness Padding { get; set; }
        public GuiScreen Owner { get; private set; }

        public void Show(GuiScreen owner)
        {
            Owner = owner;
            Owner.Controls.Add(this);
        }

        public void Hide()
        {
            Owner.Controls.Remove(this);
        }

        protected override Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            var sizes = Controls.Select(control => GuiLayoutHelper.GetSizeWithMargins(control, context, availableSize)).ToArray();
            var width = sizes.Max(s => s.Width);
            var height = sizes.Max(s => s.Height);
            return new Size2(width, height) + Padding.Size;
        }

        public override void Layout(IGuiContext context, RectangleF rectangle)
        {
            foreach (var control in Controls)
                PlaceControl(context, control, Padding.Left, Padding.Top, Width - Padding.Size.Width, Height - Padding.Size.Height);
        }
    }
}