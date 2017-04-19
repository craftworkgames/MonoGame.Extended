using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace Demo.Features.Screens
{
    public class DemoScreen : GuiScreen
    {
        public DemoScreen(GuiSkin skin, Action onNextDemo)
            : base(skin)
        {
            var button = Skin.Create<GuiButton>("white-button", c =>
            {
                c.Position = new Vector2(670, 430);
                c.Size = new Size2(120, 42);
                c.Text = "Next Demo";
            });
            var canvas = new GuiCanvas
            {
                Controls = { button }
            };

            Controls.Add(canvas);
            button.Clicked += (sender, args) => onNextDemo();
        }
    }
}