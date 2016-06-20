using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;

namespace Demo.Gui
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Camera2D _camera;
        private List<GuiControl> _controls;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var logoTexture = Content.Load<Texture2D>("logo-square-128");
            _sprite = new Sprite(logoTexture)
            {
                Position = viewportAdapter.Center.ToVector2()
            };

            var json = File.ReadAllText("Content/demo.styles");
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                Converters = new List<JsonConverter> { new MonoGameColorJsonConverter() }
            };

            var buttonStyle = JsonConvert.DeserializeObject<GuiButtonStyle>(json, settings);
            var contentService = new GuiContentService(Content);
            _controls = new List<GuiControl>();

            using (var reader = new StreamReader(@"Content/three-buttons.gui"))
            {
                var controlDatas = GuiFile.Load(reader);

                foreach (var controlData in controlDatas)
                {
                    var control = buttonStyle.CreateControl(contentService);

                    if (control != null)
                    {
                        control.Name = controlData.Name;
                        control.Position = new Vector2(controlData.X, controlData.Y);
                        control.Size = new Vector2(controlData.Width, controlData.Height);
                        _controls.Add(control);
                    }
                }
            }
            
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _sprite.Rotation += deltaTime;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());

            foreach (var control in _controls)
                control.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}