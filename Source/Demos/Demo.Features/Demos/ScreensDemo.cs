using Demo.Features.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace Demo.Features.Demos
{
    public class ScreensDemo : DemoBase
    {
        public override string Name => "Screens";

        public ScreensDemo(GameMain game) : base(game)
        {
            ScreenGameComponent screenGameComponent;
            Components.Add(screenGameComponent = new ScreenGameComponent(game));

            screenGameComponent.Register(new MainMenuScreen(game.Services, game));
            screenGameComponent.Register(new LoadGameScreen(game.Services));
            screenGameComponent.Register(new OptionsScreen(game.Services));
            screenGameComponent.Register(new AudioOptionsScreen(game.Services));
            screenGameComponent.Register(new VideoOptionsScreen(game.Services));
            screenGameComponent.Register(new KeyboardOptionsScreen(game.Services));
            screenGameComponent.Register(new MouseOptionsScreen(game.Services));
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}