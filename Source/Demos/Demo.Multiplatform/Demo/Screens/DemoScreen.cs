using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace Demo.Screens
{
    public class DemoScreen : GuiScreen
    {
        public DemoScreen(GuiSkin skin, Action onNextDemo)
            : base(skin)
        {
            var button = Skin.Create<GuiButton>("white-button", new Vector2(100, 32), text: "Next Demo");
            button.Clicked += (s, e) => onNextDemo();
            Controls.Add(button);
        }
    }
}