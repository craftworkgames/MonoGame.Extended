using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Screens
{
    public class ScreenManager : IDraw, IUpdate
    {
        private readonly Game _game;

        public ScreenManager(Game game)
        {
            _game = game;
            _game.Window.ClientSizeChanged += WindowOnClientSizeChanged;
            _game.Window.OrientationChanged += WindowOnClientSizeChanged;
            _game.Activated += (s, e) => Resume();
            _game.Deactivated += (s, e) => Pause();

            IsPaused = !_game.IsActive;
        }

        public bool IsPaused { get; private set; }

        private void WindowOnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            if (_currentScreen != null)
            {
                var bounds = _game.Window.ClientBounds;
                _currentScreen.Resize(bounds.Width, bounds.Height);
            }
        }

        private Screen _currentScreen;
        public Screen CurrentScreen
        {
            get { return _currentScreen; }
            set
            {
                if (_currentScreen != null)
                    _currentScreen.Hide();

                _currentScreen = value;

                if(_currentScreen != null)
                    _currentScreen.Show();
            }
        }

        public void Pause()
        {
            if (!IsPaused)
            {
                IsPaused = true;

                if (_currentScreen != null)
                    _currentScreen.Pause();
            }
        }

        public void Resume()
        {
            if (IsPaused)
            {
                IsPaused = false;

                if (_currentScreen != null)
                    _currentScreen.Resume();
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (_currentScreen != null)
                _currentScreen.Draw(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            if (_currentScreen != null)
                _currentScreen.Update(gameTime);
        }
    }
}
