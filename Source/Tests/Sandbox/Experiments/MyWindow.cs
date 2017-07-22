using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace Sandbox
{
    public class MyWindow : GuiWindow
    {
        public MyWindow(GuiScreen parent)
            : base(parent)
        {
            Width = 300;
            Height = 200;

            Controls.Add(new MyPanel(Skin) { Text = "My Panel", HorizontalTextAlignment = HorizontalAlignment.Right });

            var button = new GuiButton(Skin)
            {
                Width = 100,
                Height = 32,
                Text = "Yay",
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Controls.Add(button);

            Controls.Add(new GuiStackPanel(Skin)
            {
                Controls =
                {
                    new GuiLabel(Skin, "propertyName"),
                    new GuiStackPanel
                    {
                        HorizontalAlignment = HorizontalAlignment.Centre,
                        VerticalAlignment = VerticalAlignment.Centre,
                        Orientation = GuiOrientation.Horizontal,
                        Controls =
                        {
                            new GuiLabel(Skin, "propertyName"),
                            new GuiTextBox(Skin, "Hello"),
                            new GuiTextBox(Skin, "World")
                        }
                    }
                }
            });

            var label = new GuiLabel(Skin, "This is just a boring dialog.");
            Controls.Add(label);
        }
    }
}