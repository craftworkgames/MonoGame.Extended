using System;

namespace Demo.Screens
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