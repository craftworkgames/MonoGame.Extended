using System;
using System.IO;
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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json.Linq;

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
            var skin0 = LoadSkin(@"Content/adventure-gui-skin.json");

            var textureAtlas = Content.Load<TextureAtlas>("adventure-gui-atlas");
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);

            _camera = new Camera2D(viewportAdapter);

            var buttonRegion = textureAtlas["buttonLong_grey"];
            var buttonRegionPressed = textureAtlas["buttonLong_grey_pressed"];
            var panelRegion = new NinePatchRegion2D(textureAtlas["panel_brown"], 10);
            var panelInsetRegion = new NinePatchRegion2D(textureAtlas["panelInset_beige"], 10);

            var skin = new GuiSkin
            {
                TextureAtlases = new Dictionary<string, TextureAtlas>()
                {
                    { "adventure-gui-atlas", Content.Load<TextureAtlas>("adventure-gui-atlas") }
                },
                Fonts = new Dictionary<string, BitmapFont>
                {
                    { "small-font", Content.Load<BitmapFont>("small-font") }
                },
                Templates =
                {
                    {
                        "white-button",
                        new GuiControlStyle(typeof(GuiButton))
                        {
                            { "TextureRegion", buttonRegion },
                            { "TextOffset", new Vector2(0, -2) },
                            { "TextColor", Color.SandyBrown },
                            { "Color", Color.SaddleBrown },
                            { "PressedStyle", new GuiControlStyle(typeof(GuiButton))
                                {
                                    { "TextureRegion", buttonRegionPressed },
                                    { "TextOffset", new Vector2(0, 2) }
                                }
                            },
                            { "HoverStyle", new GuiControlStyle(typeof(GuiButton))
                                {
                                    { "Color", Color.SandyBrown },
                                    { "TextColor", Color.White },
                                }
                            }
                        }
                    },
                    {
                        "brown-panel",
                        new GuiControlStyle(typeof(GuiPanel))
                        {
                            { "TextureRegion", panelRegion },
                            { "Size", new Size2(400, 300) }
                        }
                    },
                    {
                        "beige-inset-panel",
                        new GuiControlStyle(typeof(GuiPanel))
                        {
                            { "TextureRegion", panelInsetRegion },
                            { "Size", new Size2(380, 280) }
                        }
                    }
                }
            };

            var serializer = new JsonSerializer();

            using (var stringWriter = new StringWriter())
            {
                serializer.Converters.Add(new TextureRegion2DConveter());
                serializer.Converters.Add(new ColorJsonConverter());
                serializer.Converters.Add(new NinePatchRegion2DConveter());
                serializer.Converters.Add(new Size2JsonConverter());
                serializer.Converters.Add(new Vector2JsonConverter());
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(stringWriter, skin);
                var json = stringWriter.ToString();
            }

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

        private GuiSkin LoadSkin(string name)
        {
            var serializer = new JsonSerializer();

            serializer.Converters.Add(new GuiSkinJsonConverter(Content));
            serializer.Converters.Add(new TextureRegion2DConveter());
            serializer.Converters.Add(new ColorJsonConverter());
            serializer.Converters.Add(new NinePatchRegion2DConveter());
            serializer.Converters.Add(new Size2JsonConverter());
            serializer.Converters.Add(new Vector2JsonConverter());

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

    public class GuiSkinJsonConverter : JsonConverter
    {
        private readonly ContentManager _contentManager;

        public GuiSkinJsonConverter(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiSkin);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var root = JToken.Load(reader);
            var fonts = LoadObject<string[]>(root, serializer, "Fonts")
                .ToDictionary(c => c, c => _contentManager.Load<BitmapFont>(c));

            var atlases = LoadObject<string[]>(root, serializer, "TextureAtlases")
                .ToDictionary(c => c, c => _contentManager.Load<TextureAtlas>(c));

            return new GuiSkin
            {
                Name = root.Value<string>("Name"),
                TextureAtlases = atlases,
                Fonts = fonts
            };
        }

        private static T LoadObject<T>(JToken root, JsonSerializer serializer, string key)
        {
            return root
                .Value<JToken>(key)
                .ToObject<T>(serializer);
        }
    }
}
