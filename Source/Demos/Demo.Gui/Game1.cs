using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;

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

            var titleScreen = LoadScreen(@"Content/title-screen.json");

            var skin = LoadSkin(@"Content/adventure-gui-skin.json");
            var renderer = new GuiSpriteBatchRenderer(GraphicsDevice, skin.DefaultFont);
            _guiManager = new GuiManager(viewportAdapter, renderer);

            

            var controlFactory = new GuiControlFactory(skin);
            //var screen = new GuiScreen
            //{
            //    Controls =
            //    {
            //        controlFactory.Create<GuiPanel>("brown-panel", c =>
            //        {
            //            c.Position = new Vector2(400, 240);
            //            c.Size = new Size2(750, 430);
            //        }),
            //        controlFactory.Create<GuiPanel>("beige-inset-panel", c =>
            //        {
            //            c.Position = new Vector2(400, 240);
            //            c.Size = new Size2(730, 410);
            //            c.Controls.Add(controlFactory.Create<GuiButton>("white-button", b =>
            //            {
            //                b.Position = (Vector2)c.Size - ((Vector2) b.Size / 2f).Round();
            //                b.Text = "Nested Button";
            //            }));
            //        }),
            //        controlFactory.Create<GuiButton>("white-button", position: new Vector2(400, 190), name: "PlayButton", text: "Play"),
            //        controlFactory.Create<GuiButton>("white-button", position: new Vector2(400, 240), name: "OptionsButton", text: "Options"),
            //        controlFactory.Create<GuiButton>("white-button", position: new Vector2(400, 290), name: "QuitButton", text: "Quit")                    
            //    }
            //};

            //titleScreen.FindControl<GuiButton>("QuitButton").Clicked += (sender, args) => Exit();
            _guiManager.Screen = titleScreen;
        }

        private GuiScreen LoadScreen(string name)
        {
            var skinService = new GuiSkinService();
            var serializer = new GuiJsonSerializer(Content)
            {
                Converters =
                {
                    new GuiSkinJsonConverter(Content, skinService),
                    new GuiControlJsonConverter(skinService)
                }
            };

            using (var stream = TitleContainer.OpenStream(name))
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var screen = serializer.Deserialize<GuiScreen>(jsonReader);
                return screen;
            }
        }

        private GuiSkin LoadSkin(string name)
        {
            var serializer = new GuiJsonSerializer(Content);

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
}
