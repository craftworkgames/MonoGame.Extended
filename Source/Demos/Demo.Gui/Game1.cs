using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Demo.Gui
{
    public interface IGuiDrawable
    {
        void Draw(SpriteBatch spriteBatch, RectangleF rectangle);
    }

    public class GuiSprite : IGuiDrawable
    {
        public TextureRegion2D TextureRegion { get; set; }

        public void Draw(SpriteBatch spriteBatch, RectangleF rectangle)
        {
            spriteBatch.Draw(TextureRegion, rectangle.Location, Color.White);
        }
    }

    public class GuiText : IGuiDrawable
    {
        public BitmapFont Font { get; set; }
        public string Text { get; set; }

        public void Draw(SpriteBatch spriteBatch, RectangleF rectangle)
        {
            spriteBatch.DrawString(Font, Text, rectangle.Location, Color.White);
        }
    }

    public class GuiTemplate
    {
        public GuiTemplate()
        {
            Drawables = new List<IGuiDrawable>();
        }

        public string Name { get; set; }
        public List<IGuiDrawable> Drawables { get; }

        public void Draw(SpriteBatch spriteBatch, RectangleF rectangle)
        {
            foreach (var drawable in Drawables)
                drawable.Draw(spriteBatch, rectangle);
        }
    }

    public class GuiDefinition
    {
        public string TextureAtlas { get; set; }
        public string[] Fonts { get; set; }
        public JObject[] Templates { get; set; }
    }

    public class GuiDrawableJsonConverter : JsonConverter
    {
        private readonly GuiJsonConverterService _converterService;

        public GuiDrawableJsonConverter(GuiJsonConverterService converterService)
        {
            _converterService = converterService;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(IGuiDrawable))
                return true;

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var type = (string)jObject.Property("Type");

            if (type == "Sprite")
            {
                var textureRegion = (string)jObject.Property("TextureRegion");
                return new GuiSprite { TextureRegion = _converterService.GetTextureRegion(textureRegion) };
            }

            if (type == "Text")
            {
                var font = (string)jObject.Property("Font");
                var text = (string)jObject.Property("Text");
                return new GuiText { Font = _converterService.GetFont(font), Text = text };
            }

            return null;
        }
    }

    public class GuiJsonConverterService
    {
        private readonly TextureAtlas _textureAtlas;
        private readonly Dictionary<string, BitmapFont> _bitmapFonts;

        public GuiJsonConverterService(TextureAtlas textureAtlas, IEnumerable<BitmapFont> bitmapFonts)
        {
            _textureAtlas = textureAtlas;
            _bitmapFonts = bitmapFonts.ToDictionary(f => f.Name);
        }

        public TextureRegion2D GetTextureRegion(string name)
        {
            return _textureAtlas[name];
        }

        public BitmapFont GetFont(string name)
        {
            return _bitmapFonts[name];
        }
    }

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
    'Templates': [
        { 
            'Name': 'BlueButton',
            'Drawables': [
                { 'Type': 'Sprite', 'TextureRegion': 'blue_button00' },
                { 'Type': 'Text', 'Font': 'montserrat-32', 'Text': 'Hello' }
            ]
        }
    ]
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

                foreach (var templateDefinition in guiDefinition.Templates)
                {
                    _template = templateDefinition.ToObject<GuiTemplate>(jsonSerializer);
                }
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

            _spriteBatch.Begin();
            var rectangleF = new RectangleF(100, 100, 200, 100);
            _template.Draw(_spriteBatch, rectangleF);
            _spriteBatch.DrawRectangle(rectangleF, Color.Red);
            _spriteBatch.End();
        }
    }
}