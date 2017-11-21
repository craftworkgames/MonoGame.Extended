using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Features.Demos;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Features.Screens
{
    public class SelectDemoScreen : GuiScreen
    {
        private readonly IDictionary<string, DemoBase> _demos;
        private readonly Action<string> _loadDemo;

        public SelectDemoScreen(GuiSkin skin, IDictionary<string, DemoBase> demos, Action<string> loadDemo, Action exitGameAction)
            : base(skin)
        {
            _demos = demos;
            _loadDemo = loadDemo;

            var dialog = Skin.Create<GuiDialog>("dialog");
            var grid = new GuiUniformGrid { Columns = 3 };

            foreach (var demo in _demos.Values.OrderBy(i => i.Name))
            {
                var button = Skin.Create<GuiButton>("white-button", c =>
                {
                    c.Text = demo.Name;
                    c.Margin = new Thickness(4);
                    c.Clicked += (sender, args) => LoadDemo(demo);
                });
                grid.Controls.Add(button);
            }

            // Close button
            var atlas = skin.TextureAtlases[0];
            var closeButtonRegion = atlas?.GetRegion("buttonRound_close");
            if (closeButtonRegion != null)
            {
                var closeButton = Skin.Create<GuiButton>("white-button", c =>
                {
                    c.IconRegion = closeButtonRegion;
                    c.Margin = new Thickness(4);
                    c.Clicked += (sender, args) => exitGameAction();
                });
                grid.Controls.Add(closeButton);
            }

            dialog.Controls.Add(grid);
            Controls.Add(dialog);
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
            var dialog = Skin.Create<GuiDialog>("dialog");
            
            var stackPanel = new GuiStackPanel
            {
                Controls =
                {
                    Skin.Create<GuiLabel>("label", c =>
                    {
                        c.Text = "Are you sure you want to do that?";
                        c.Margin = new Thickness(0, 20, 0, 0);
                    }),
                    Skin.Create<GuiLabel>("label", c => { c.Text = "If you do you'll be in some serious trouble."; }),
                    new GuiStackPanel
                    {
                        Orientation = GuiOrientation.Horizontal,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Centre,
                        Controls =
                        {
                            Skin.Create<GuiButton>("white-button", c =>
                            {
                                c.Text = "Yes";
                                c.Width = 100;
                                c.Margin = new Thickness(2);
                                c.Offset = new Vector2(0, 20);
                            }),
                            Skin.Create<GuiButton>("white-button", c =>
                            {
                                c.Text = "No";
                                c.Width = 100;
                                c.Margin = new Thickness(2);
                                c.Offset = new Vector2(0, 20);
                            })
                        }
                    }
                }
            };

            dialog.Controls.Add(stackPanel);
            dialog.Controls.Add(Skin.Create<GuiLabel>("label", c =>
            {
                c.Text = "Please confirm";
                c.VerticalAlignment = VerticalAlignment.Top;
                c.HorizontalAlignment = HorizontalAlignment.Centre;
                c.Offset = new Vector2(0, -30);
                c.BackgroundRegion = Skin.NinePatches.FirstOrDefault(i => i.Name == "progress-bar-blue");
            }));
            dialog.Controls.Add(Skin.Create<GuiButton>("close-button", c =>
            {
                c.HorizontalAlignment = HorizontalAlignment.Right;
                c.VerticalAlignment = VerticalAlignment.Top;
                c.Offset = new Vector2(20, -20);
            }));

            dialog.Show(this);
        }
    }
}