using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Screens
{
    public class ScreenManager : SimpleDrawableGameComponent
    {
        public ScreenManager()
        {
        }

        private Screen _activeScreen;
        private bool _isInitialized;
        private bool _isLoaded;

        public void LoadScreen(Screen screen)
        {
            _activeScreen?.UnloadContent();
            _activeScreen?.Dispose();

            screen.ScreenManager = this;

            if (_isInitialized)
                screen.Initialize();

            if (_isLoaded)
                screen.LoadContent();

            _activeScreen = screen;
        }

        public override void Initialize()
        {
            base.Initialize();
            _activeScreen?.Initialize();
            _isInitialized = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _activeScreen?.LoadContent();
            _isLoaded = true;
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            _activeScreen?.UnloadContent();
            _isLoaded = false;
        }

        public override void Update(GameTime gameTime)
        {
            _activeScreen?.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _activeScreen.Draw(gameTime);
        }
    }
}