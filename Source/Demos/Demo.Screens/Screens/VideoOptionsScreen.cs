using System;

namespace Demo.Screens.Screens
{
    public class VideoOptionsScreen : MenuScreen
    {
        public VideoOptionsScreen(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            AddMenuItem("Back", Show<OptionsScreen>);
        }
    }
}