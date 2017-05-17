using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

// The Sandbox project is used for experiementing outside the normal demos.
// Any code found here should be considered experimental work in progress.
namespace Sandbox
{
    public class MyWindow : GuiWindow
    {
        public MyWindow(GuiScreen parent)
            : base(parent)
        {
            Width = 300;
            Height = 200;
        }

        protected override void Initialize()
        {
            var button = Skin.Create<GuiButton>("white-button", text: "I'm in a dialog");
            button.Width = 100;
            button.Height = 32;
            Controls.Add(button);
        }
    }

    public class MyScreen : GuiScreen
    {
        public MyScreen(GuiSkin skin) 
            : base(skin)
        {
        }

        public override void Initialize()
        {
            var button = new GuiButton() {Text = "Press Me"};//Skin.Create<GuiButton>("white-button", text: "Open Dialog");}
            button.Clicked += OpenDialog_Clicked;
            Controls.Add(button);
        }

        private void OpenDialog_Clicked(object sender, EventArgs eventArgs)
        {
            var dialog = new MyWindow(this);
            dialog.Show();
        }
    }

    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private ViewportAdapter _viewportAdapter;
        private Camera2D _camera;
        private GuiSystem _guiSystem;

        private SpriteBatch _spriteBatch;
        //private ParticleEffect _particleEffect;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            IsMouseVisible = false;

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter);

            var skin = GuiSkin.FromFile(Content, @"Content/adventure-gui-skin.json");
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, _viewportAdapter.GetScaleMatrix);

            _guiSystem = new GuiSystem(_viewportAdapter, guiRenderer, skin);
            _guiSystem.Screens.Add(new MyScreen(skin));

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            var particleTexture = new Texture2D(GraphicsDevice, 1, 1);
            particleTexture.SetData(new[] { Color.White });

            var textureRegionService = new TextureRegionService();
            textureRegionService.TextureAtlases.Add(Content.Load<TextureAtlas>("adventure-gui-atlas"));

            //_particleEffect = ParticleEffect.FromFile(textureRegionService, @"Content/particle-effect.json");
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var p = _camera.ScreenToWorld(mouseState.X, mouseState.Y);

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            //_particleEffect.Update(deltaTime);

            //if (mouseState.LeftButton == ButtonState.Pressed)
            //    _particleEffect.Trigger(new Vector2(p.X, p.Y));

            //_particleEffect.Trigger(new Vector2(400, 240));

            _guiSystem.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            //_spriteBatch.Draw(_particleEffect);
            _spriteBatch.End();

            _guiSystem.Draw(gameTime);
        }
    }
}
