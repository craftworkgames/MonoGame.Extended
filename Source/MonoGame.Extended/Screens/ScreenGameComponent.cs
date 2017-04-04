using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Screens
{
    public interface IScreenManager
    {
        T FindScreen<T>() where T : Screen;
    }

    public class ScreenGameComponent : DrawableGameComponent, IScreenManager
    {
        private readonly List<Screen> _screens;

        public ScreenGameComponent(Game game, IEnumerable<Screen> screens)
            : this(game)
        {
            foreach (var screen in screens)
                Register(screen);
        }

        public ScreenGameComponent(Game game)
            : base(game)
        {
            _screens = new List<Screen>();
        }

        public T FindScreen<T>() where T : Screen
        {
            var screen = _screens.OfType<T>().FirstOrDefault();

            if (screen == null)
                throw new InvalidOperationException($"{typeof(T).Name} not registered");

            return screen;
        }

        public T Register<T>(T screen)
            where T : Screen
        {
            screen.ScreenManager = this;
            _screens.Add(screen);

            if (_screens.Count == 1)
                _screens[0].Show();

            return screen;
        }

        public override void Initialize()
        {
            foreach (var screen in _screens)
                screen.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            foreach (var screen in _screens)
                screen.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (var screen in _screens)
                screen.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var screen in _screens.Where(s => s.IsVisible))
                screen.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (var screen in _screens.Where(s => s.IsVisible))
                screen.Draw(gameTime);
        }
    }
}