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
        private GuiTemplate _template;

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
            const string json =
@"{ 
    'TextureAtlas': 'Content/kenney-gui-blue-atlas.xml',
    'Fonts': [ 'montserrat-32' ],
    'Templates': {
        'BlueButton': { 
            'Type': 'Button',
            'Drawables': [
                { 
                    'Type': 'Sprite', 
                    'TextureRegion': 'blue_button00', 
                    'Color': '#aaffaa'
                },
                { 'Type': 'Text', 'Font': 'montserrat-32', 'Text': 'Hello World', 'Color': '#ffffff55' }
            ]
        }
    }
  }";
            var guiDefinition = JsonConvert.DeserializeObject<GuiDefinition>(json);
            var bitmapFonts = guiDefinition.Fonts
                .Select(f => Content.Load<BitmapFont>(f))
                .ToArray();

            using (var stream = TitleContainer.OpenStream(guiDefinition.TextureAtlas))
            {
                _textureAtlas = TextureAtlasReader.FromRawXml(Content, stream);

                var converterService = new GuiJsonConverterService(_textureAtlas, bitmapFonts);
                var jsonSerializer = new JsonSerializer();

                jsonSerializer.Converters.Add(new GuiDrawableJsonConverter(converterService));
                jsonSerializer.Converters.Add(new MonoGameColorJsonConverter());

                _template = guiDefinition.Templates["BlueButton"].ToObject<GuiTemplate>(jsonSerializer);
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
            var rectangleF = new RectangleF(100, 100, 300, 100);
            _template.Draw(_spriteBatch, rectangleF);
            _spriteBatch.DrawRectangle(rectangleF, Color.Red);
            _spriteBatch.End();
        }
    }
}