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
            button.ClipPadding = new MonoGame.Extended.Thickness(15);
            Controls.Add(button);

            button.SetBinding(nameof(GuiButton.Text), nameof(viewModel.Name));
            button.Clicked += (sender, args) =>
            {
                viewModel.Name = "alliestraszaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            };

            BindingContext = viewModel;
        }
    }
}