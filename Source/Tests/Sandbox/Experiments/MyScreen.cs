using System;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace Sandbox
{
    public class MyScreen : GuiScreen
    {
        public MyScreen(GuiSkin skin)
            : base(skin)
        {
            Controls.Add(new GuiUniformGrid(Skin)
            {
                Controls =
                {
                    new GuiComboBox(Skin)
                    {
                        Name = "ComboBox",
                        Items =
                        {
                            new { Name = "one", Number = 1 },
                            new { Name = "two", Number = 2 },
                            new { Name = "three", Number = 3 }
                        },
                        SelectedIndex = 0,
                        NameProperty = "Number"
                    },
                    new GuiListBox(Skin)
                    {
                        Name = "ListBox",
                        Items =
                        {
                            "one",
                            "two",
                            "three"
                        }
                    }
                }
            });

            var listBox = FindControl<GuiListBox>("ListBox");
            var comboBox = FindControl<GuiComboBox>("ComboBox");
            comboBox.SelectedIndexChanged += (sender, args) => listBox.SelectedIndex = comboBox.SelectedIndex;
            listBox.SelectedIndexChanged += (sender, args) => comboBox.SelectedIndex = listBox.SelectedIndex;
        }

        private void OpenDialog_Clicked(object sender, EventArgs eventArgs)
        {
            var dialog = new MyWindow(this);
            dialog.Show();
        }
    }
}