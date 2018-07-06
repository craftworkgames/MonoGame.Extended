using System;

namespace Demo.Features.Screens
{
    public class OptionsScreen : MenuScreen
    {
        public OptionsScreen(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            AddMenuItem("Audio Options", Show<AudioOptionsScreen>);
            AddMenuItem("Video Options", Show<VideoOptionsScreen>);
            AddMenuItem("Keyboard Options", Show<KeyboardOptionsScreen>);
            AddMenuItem("Mouse Options", Show<MouseOptionsScreen>);
            AddMenuItem("Back", Show<MainMenuScreen>);
        }
    }
}