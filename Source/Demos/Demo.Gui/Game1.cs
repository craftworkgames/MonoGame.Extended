using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Content;

namespace Demo.Gui
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private Camera2D _camera;
        private GuiManager _guiManager;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var skin = LoadSkin(@"Content/adventure-gui-skin.json");
            var renderer = new GuiSpriteBatchRenderer(GraphicsDevice, skin.DefaultFont);
            _guiManager = new GuiManager(viewportAdapter, renderer);

            var controlFactory = new GuiControlFactory(skin);
            var screen = new GuiScreen
            {
                Controls =
                {
                    controlFactory.CreateControl<GuiPanel>("brown-panel", new Vector2(400, 240), "Panel"),
                    controlFactory.CreateControl<GuiPanel>("beige-inset-panel", new Vector2(400, 240), "Panel"),
                    controlFactory.CreateControl<GuiButton>("white-button", new Vector2(400, 190), "PlayButton", "Play"),
                    controlFactory.CreateControl<GuiButton>("white-button", new Vector2(400, 240), "OptionsButton", "Options"),
                    controlFactory.CreateControl<GuiButton>("white-button", new Vector2(400, 290), "QuitButton", "Quit")                    
                }
            };

            _guiManager.Screen = screen;
        }

        private JsonSerializer CreateSerializer()
        {
            var textureRegionService = new TextureRegionService();
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new ColorJsonConverter());
            serializer.Converters.Add(new ContentConverter<BitmapFont>(Content, font => font.Name));
            serializer.Converters.Add(new TextureAtlasConverter(Content, textureRegionService));
            serializer.Converters.Add(new GuiControlStyleConverter());
            serializer.Converters.Add(new NinePatchRegion2DConveter(textureRegionService));
            serializer.Converters.Add(new Size2JsonConverter());
            serializer.Converters.Add(new TextureRegion2DConveter(textureRegionService));
            //serializer.Converters.Add(new TypeJsonConverter());
            serializer.Converters.Add(new Vector2JsonConverter());
            serializer.ContractResolver = new GuiJsonContractResolver();
            serializer.Formatting = Formatting.Indented;
            return serializer;
        }

        private GuiSkin LoadSkin(string name)
        {
            var serializer = CreateSerializer();
            
            using (var stream = TitleContainer.OpenStream(name))
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var skin = serializer.Deserialize<GuiSkin>(jsonReader);
                return skin;
            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _guiManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _guiManager.Draw(gameTime);

        }
    }

    public class GuiControlStyleConverter : JsonConverter
    {
        private readonly Dictionary<string, Type> _controlTypes;

        public GuiControlStyleConverter()
        {
            _controlTypes = typeof(GuiControl)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(GuiControl)))
                .ToDictionary(t => t.Name);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var style = (GuiControlStyle) value;
            var dictionary = new Dictionary<string, object> {{"TargetType", style.TargetType.Name}};

            foreach (var keyValuePair in style)
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);

            serializer.Serialize(writer, dictionary);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            const string targetTypeKey = "TargetType";
            var dictionary = serializer.Deserialize<Dictionary<string, object>>(reader);
            var typeName = (string) dictionary[targetTypeKey];
            var targetType = _controlTypes[typeName];
            var properties = targetType.GetProperties().ToDictionary(p => p.Name);
            var style = new GuiControlStyle(targetType);

            foreach (var keyValuePair in dictionary.Where(i => i.Key != targetTypeKey))
            {
                var property = properties[keyValuePair.Key];
                var json = JsonConvert.SerializeObject(keyValuePair.Value);

                using (var textReader = new StringReader(json))
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    var value = serializer.Deserialize(jsonReader, property.PropertyType);
                    style.Add(keyValuePair.Key, value);
                }
            }

            return style;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiControlStyle);
        }
    }

    public class ContentConverter<T> : JsonConverter
    {
        private readonly ContentManager _contentManager;
        private readonly Func<T, string> _getAssetName;

        public ContentConverter(ContentManager contentManager, Func<T, string> getAssetName)
        {
            _contentManager = contentManager;
            _getAssetName = getAssetName;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var asset = (T) value;
            var assetName = _getAssetName(asset);
            writer.WriteValue(assetName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var assetName = (string) reader.Value;
            return _contentManager.Load<T>(assetName);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
    }

    public class TextureAtlasConverter : ContentConverter<TextureAtlas>
    {
        private readonly ITextureRegionService _textureRegionService;

        public TextureAtlasConverter(ContentManager contentManager, ITextureRegionService textureRegionService) 
            : base(contentManager, atlas => atlas.Name)
        {
            _textureRegionService = textureRegionService;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var textureAtlas = base.ReadJson(reader, objectType, existingValue, serializer) as TextureAtlas;

            if (textureAtlas != null)
                _textureRegionService.TextureAtlases.Add(textureAtlas);

            return textureAtlas;
        }
    }
}
