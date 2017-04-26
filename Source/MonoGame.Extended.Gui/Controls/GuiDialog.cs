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

        public override void Layout(IGuiContext context, RectangleF rectangle)
        {
            foreach (var control in Controls)
                PlaceControl(context, control, Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right, Height - Padding.Top - Padding.Bottom);
        }
    }
}