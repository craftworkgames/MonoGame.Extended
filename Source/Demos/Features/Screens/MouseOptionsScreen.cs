using System;

namespace Demo.Features.Screens
{
    public class MouseOptionsScreen : MenuScreen
    {
        public MouseOptionsScreen(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            AddMenuItem("Back", Show<OptionsScreen>);
        }
    }
}