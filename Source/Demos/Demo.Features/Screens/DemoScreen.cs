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
                c.Position = new Vector2(700, 450);
                c.Size = new Size2(100, 32);
                c.Text = "Next Demo";
            });
            //var stackPanel = new GuiStackPanel
            //{
            //    Origin = Vector2.Zero,
            //    Controls = { button }
            //};

            //PerformLayout(stackPanel);
            Controls.Add(button);

            button.Clicked += (sender, args) => onNextDemo();
        }

        //private static void PerformLayout(GuiStackPanel control)
        //{
        //    control.Position = Vector2.Zero;
        //    control.Size = new Size2(100, 380);
        //    control.PerformLayout();
        //}
    }
}