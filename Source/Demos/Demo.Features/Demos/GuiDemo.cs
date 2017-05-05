using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;

namespace Demo.Features.Demos
{
    public class GuiDemo : DemoBase
    {
        public override string Name => "GUI";

        private readonly GameMain _game;
        private SpriteBatch _spriteBatch;
        private ViewportAdapter _viewportAdapter;
        private Texture2D _backgroundTexture;
        private Camera2D _camera;
        private GuiSystem _guiSystem;
        
        public GuiDemo(GameMain game) : base(game)
        {
            _game = game;
        }
        
        protected override void LoadContent()
        {
            IsMouseVisible = false;

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("Textures/colored_castle");

            var titleScreen = GuiScreen.FromFile(Content, @"Content/title-screen.json");
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, _camera.GetViewMatrix);
            _guiSystem = new GuiSystem(_viewportAdapter, guiRenderer) { Screen = titleScreen };

            var quitButton = titleScreen.FindControl<GuiButton>("QuitButton");
            quitButton.Clicked += (sender, args) => _game.Back();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _guiSystem.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin(transformMatrix: _viewportAdapter.GetScaleMatrix());
            _spriteBatch.Draw(_backgroundTexture, _viewportAdapter.BoundingRectangle, Color.White);
            _spriteBatch.End();

            _guiSystem.Draw(gameTime);
        }
    }
}