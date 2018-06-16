using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Features.Demos;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Features.Screens
{
    public class SelectDemoScreen : Screen
    {
        private readonly IDictionary<string, DemoBase> _demos;
        private readonly Action<string> _loadDemo;

        public SelectDemoScreen(IDictionary<string, DemoBase> demos, Action<string> loadDemo, Action exitGameAction)
        {
            _demos = demos;
            _loadDemo = loadDemo;

            var grid = new UniformGrid();

            foreach (var demo in _demos.Values.OrderBy(i => i.Name))
            {
                var button = new Button()
                {
                    Content = demo.Name,
                    Margin = new Thickness(4),
                };
                button.Clicked += (sender, args) => LoadDemo(demo);
                grid.Items.Add(button);
            }

            var closeButton = new Button()
            {
                Margin = 4,
                Content = "Close",
            };
            closeButton.Clicked += (sender, args) => exitGameAction();
            grid.Items.Add(closeButton);

            this.Content = grid;
        }

        public override void Dispose()
        {
            foreach (var demo in _demos.Values)
                demo.Dispose();

            base.Dispose();
        }

        private void LoadDemo(DemoBase demo)
        {
            _loadDemo(demo.Name);
            Hide();
        }

        private void DialogDemo()
        {
            //var dialog = Skin.Create<GuiDialog>("dialog");
            
            //var stackPanel = new GuiStackPanel
            //{
            //    Controls =
            //    {
            //        Skin.Create<GuiLabel>("label", c =>
            //        {
            //            c.Text = "Are you sure you want to do that?";
            //            c.Margin = new Thickness(0, 20, 0, 0);
            //        }),
            //        Skin.Create<GuiLabel>("label", c => { c.Text = "If you do you'll be in some serious trouble."; }),
            //        new GuiStackPanel
            //        {
            //            Orientation = GuiOrientation.Horizontal,
            //            VerticalAlignment = VerticalAlignment.Bottom,
            //            HorizontalAlignment = HorizontalAlignment.Centre,
            //            Controls =
            //            {
            //                Skin.Create<GuiButton>("white-button", c =>
            //                {
            //                    c.Text = "Yes";
            //                    c.Width = 100;
            //                    c.Margin = new Thickness(2);
            //                    c.Offset = new Vector2(0, 20);
            //                }),
            //                Skin.Create<GuiButton>("white-button", c =>
            //                {
            //                    c.Text = "No";
            //                    c.Width = 100;
            //                    c.Margin = new Thickness(2);
            //                    c.Offset = new Vector2(0, 20);
            //                })
            //            }
            //        }
            //    }
            //};

            //dialog.Controls.Add(stackPanel);
            //dialog.Controls.Add(Skin.Create<Label>("label", c =>
            //{
            //    c.Text = "Please confirm";
            //    c.VerticalAlignment = VerticalAlignment.Top;
            //    c.HorizontalAlignment = HorizontalAlignment.Centre;
            //    c.Offset = new Vector2(0, -30);
            //    c.BackgroundRegion = Skin.NinePatches.FirstOrDefault(i => i.Name == "progress-bar-blue");
            //}));
            //dialog.Controls.Add(Skin.Create<Button>("close-button", c =>
            //{
            //    c.HorizontalAlignment = HorizontalAlignment.Right;
            //    c.VerticalAlignment = VerticalAlignment.Top;
            //    c.Offset = new Vector2(20, -20);
            //}));

            //dialog.Show(this);
        }
    }
}