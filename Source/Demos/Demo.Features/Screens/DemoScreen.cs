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

        public override void Initialize()
        {
            //var dialog = Skin.Create<GuiDialog>("dialog");
            //var okayButton = Skin.Create<GuiButton>("white-button", c => 
            //{
            //    c.Text = "Yes";
            //    c.Width = 100;
            //    c.Margin = new Thickness(5);
            //});
            //var cancelButton = Skin.Create<GuiButton>("white-button", c =>
            //{
            //    c.Text = "No";
            //    c.Width = 100;
            //    c.Margin = new Thickness(5);
            //});

            //var stackPanel = new GuiStackPanel
            //{
            //    Orientation = GuiOrientation.Horizontal,
            //    VerticalAlignment = VerticalAlignment.Bottom,
            //    HorizontalAlignment = HorizontalAlignment.Centre,
            //    Controls = { okayButton, cancelButton }
            //};

            //dialog.Controls.Add(Skin.Create<GuiLabel>("label", c => c.Text = "Are you sure you want to do that?"));
            //dialog.Controls.Add(stackPanel);

            //dialog.Show(this);
        }
    }
}