using System.IO;
using System.Linq;
using Demo.Gui.Wip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace Demo.Gui
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private TextureAtlas _textureAtlas;
        private SpriteBatch _spriteBatch;
        private GuiTemplate _buttonTemplate;
        private GuiTemplate _labelTemplate;
        private GuiTemplate _textBoxTemplate;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            //Components.Add(new GuiComponent(this, @"title-screen.gui"));
        }

        protected override void LoadContent()
        {
            var json = File.ReadAllText(@"Content/default.gss");
            var guiDefinition = JsonConvert.DeserializeObject<GuiDefinition>(json);
            var bitmapFonts = guiDefinition.Fonts
                .Select(f => Content.Load<BitmapFont>(f))
                .ToArray();

            using (var stream = TitleContainer.OpenStream(guiDefinition.TextureAtlas))
            {
                _textureAtlas = TextureAtlasReader.FromRawXml(Content, stream);

                var converterService = new GuiJsonConverterService(_textureAtlas, bitmapFonts);
                var jsonSerializer = new JsonSerializer();

                jsonSerializer.Converters.Add(new GuiJsonConverter(converterService));
                jsonSerializer.Converters.Add(new MonoGameColorJsonConverter());

                _buttonTemplate = guiDefinition.Styles["BlueButton"].ToObject<GuiTemplate>(jsonSerializer);
                _labelTemplate = guiDefinition.Styles["BlueLabel"].ToObject<GuiTemplate>(jsonSerializer);
                _textBoxTemplate = guiDefinition.Styles["BlueTextBox"].ToObject<GuiTemplate>(jsonSerializer);
            }



            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);
            _buttonTemplate.Draw(_spriteBatch, new RectangleF(100, 100, 300, 100));
            _labelTemplate.Draw(_spriteBatch, new RectangleF(200, 200, 300, 100));
            _textBoxTemplate.Draw(_spriteBatch, new RectangleF(200, 300, 300, 50));
            //_spriteBatch.DrawRectangle(rectangleF, Color.Red);
            _spriteBatch.End();
        }
    }
}