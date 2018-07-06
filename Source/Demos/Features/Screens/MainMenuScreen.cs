using System;
using Microsoft.Xna.Framework;

namespace Demo.Features.Screens
{
    public class MainMenuScreen : MenuScreen
    {
        private readonly Game _game;

        public MainMenuScreen(IServiceProvider serviceProvider, Game game)
            : base(serviceProvider)
        {
            _game = game;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            AddMenuItem("New Game", () => { });
            AddMenuItem("Load Game", Show<LoadGameScreen>);
            AddMenuItem("Options", Show<OptionsScreen>);
            AddMenuItem("Exit", _game.Exit);
        }
    }
}
