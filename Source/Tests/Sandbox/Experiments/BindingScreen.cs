using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace Sandbox.Experiments
{
    public class BindingScreen : GuiScreen
    {
        public BindingScreen(GuiSkin skin, ViewModel viewModel) 
            : base(skin)
        {
            var button = new GuiButton(skin);
            Controls.Add(button);

            button.SetBinding(nameof(GuiButton.Text), nameof(viewModel.Name));
            button.Clicked += (sender, args) =>
            {
                viewModel.Name = "alliestrasza";
            };

            BindingContext = viewModel;
        }
    }
}