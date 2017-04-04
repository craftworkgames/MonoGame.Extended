using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Demo.Features.Demos
{
    public abstract class DemoBase
    {
        private readonly Game _game;
        private bool _isInitialized;

        protected DemoBase(Game game)
        {
            _game = game;

            Window.AllowUserResizing = true;
        }

        protected bool IsMouseVisible
        {
            get { return _game.IsMouseVisible; }
            set { _game.IsMouseVisible = value; }
        }

        protected ContentManager Content { get; private set; }
        protected GameWindow Window => _game.Window;
        protected GraphicsDevice GraphicsDevice => _game.GraphicsDevice;
        protected GameComponentCollection Components => _game.Components;

        protected virtual void Initialize() { }
        protected virtual void LoadContent() { }
        protected virtual void UnloadContent() { }
        protected virtual void Update(GameTime gameTime) { }
        protected virtual void Draw(GameTime gameTime) { }
        
        protected void Exit()
        {
        }

        public void Load()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }

            if (Content == null)
            {
                Content = new ContentManager(_game.Services, "Content");
                LoadContent();
            }
        }

        public void Unload()
        {
            if (Content != null)
            {
                Components.Clear();
                UnloadContent();
                Content.Unload();
                Content.Dispose();
                Content = null;
            }
        }

        public void OnUpdate(GameTime gameTime)
        {
            Update(gameTime);
        }

        public void OnDraw(GameTime gameTime)
        {
            Draw(gameTime);
        }
    }
}