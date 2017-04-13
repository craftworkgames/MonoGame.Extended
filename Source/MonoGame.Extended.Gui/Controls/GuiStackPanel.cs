using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiStackPanel : GuiControl
    {
        public GuiStackPanel()
        {
        }

        public GuiStackPanel(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
        }


        public void PerformLayout()
        {
            var y = Position.Y;
            var x = Position.X;

            foreach (var control in Controls)
            {
                control.Origin = Vector2.Zero;
                control.Position = new Vector2(x, y);
                control.Size = new Size2(Size.Width, control.Size.Height);
                y += control.Size.Height;
            }
        }

    }
}
