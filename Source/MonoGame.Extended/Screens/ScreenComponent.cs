using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Screens
{
    public class ScreenComponent : DrawableGameComponent
    {
        public ScreenComponent(Game game) 
            : base(game)
        {
            _screens = new List<Screen>();
        }

        private readonly List<Screen> _screens;

        public void Register(Screen screen)
        {
            _screens.Add(screen);
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (var screen in _screens)
                screen.Initialize();
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
