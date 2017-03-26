using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace Demo.Screens
{
    public class DemoScreen : GuiScreen
    {
        public DemoScreen(GuiSkin skin)
            : base(skin)
        {
            Controls.Add(Skin.Create<GuiButton>("white-button", new Vector2(100, 32), text: "Demo 1"));
        }
    }
}