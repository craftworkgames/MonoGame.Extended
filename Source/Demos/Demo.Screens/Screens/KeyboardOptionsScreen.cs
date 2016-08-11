using System;
using MonoGame.Extended.Screens;

namespace Demo.Screens.Screens
{
    public class KeyboardOptionsScreen : MenuScreen
    {
        public KeyboardOptionsScreen(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
        }
        
        public override void LoadContent()
        {
            base.LoadContent();

            AddMenuItem("Back", Show<OptionsScreen>);
        }
    }
}