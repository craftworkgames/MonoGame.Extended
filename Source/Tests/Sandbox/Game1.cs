using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

// The Sandbox project is used for experiementing outside the normal demos.
// Any code found here should be considered experimental work in progress.
namespace Sandbox
{
    public class MyPanel : GuiUniformGrid
    {
        public MyPanel(GuiSkin skin) : base(skin)
        {
        }
    }

    public class MyWindow : GuiWindow
    {
        public MyWindow(GuiScreen parent)
            : base(parent)
        {
            Width = 300;
            Height = 200;

            Controls.Add(new MyPanel(Skin) { Text = "My Panel", HorizontalTextAlignment = HorizontalAlignment.Right });

            var button = new GuiButton(Skin)
            {
                Width = 100,
                Height = 32,
                Text = "Yay",
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Controls.Add(button);

            Controls.Add(new GuiStackPanel(Skin)
            {
                Controls =
                {
                    new GuiLabel(Skin, "propertyName"),
                    new GuiStackPanel
                    {
                        HorizontalAlignment = HorizontalAlignment.Centre,
                        VerticalAlignment = VerticalAlignment.Centre,
                        Orientation = GuiOrientation.Horizontal,
                        Controls =
                        {
                            new GuiLabel(Skin, "propertyName"),
                            new GuiTextBox(Skin, "Hello"),
                            new GuiTextBox(Skin, "World")
                        }
                    }
                }
            });

            var label = new GuiLabel(Skin, "This is just a boring dialog.");
            Controls.Add(label);
        }
    }

    public class MyScreen : GuiScreen
    {
        public MyScreen(GuiSkin skin)
            : base(skin)
        {
            //Controls.Add(new GuiUniformGrid(Skin)
            //{
            //    Columns = 2,
            //    HorizontalAlignment = HorizontalAlignment.Right,
            //    VerticalAlignment = VerticalAlignment.Top,
            //    Controls =
            //    {
            //        new GuiLabel(Skin, "A"),
            //        new GuiUniformGrid(Skin)
            //        {
            //            Controls =
            //            {
            //                new GuiTextBox(Skin) {Text = "Play"},
            //                new GuiTextBox(Skin) {Text = "Play"},
            //            }
            //        },
            //        new GuiLabel(Skin, "B"),
            //        new GuiTextBox(Skin, "Text")
            //    }
            //});

            //var button = new GuiButton(Skin) { Text = "Press Me" };
            //button.Clicked += OpenDialog_Clicked;
            //Controls.Add(button);


            //Controls.Add(new GuiListBox(Skin)
            //{
            //    HorizontalAlignment = HorizontalAlignment.Centre,
            //    VerticalAlignment = VerticalAlignment.Centre,
            //    Items =
            //    {
            //        "one",
            //        "two",
            //        "three"
            //    }
            //});

            Controls.Add(new GuiUniformGrid(Skin)
            {
                Controls =
                {
                    new GuiComboBox(Skin)
                    {
                        Items =
                        {
                            "one",
                            "two",
                            "three"
                        },
                        SelectedIndex = 0
                    },
                    new GuiListBox(Skin)
                    {
                        Items =
                        {
                            "one",
                            "two",
                            "three"
                        }
                    }
                }
            });
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
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private ViewportAdapter _viewportAdapter;
        private Camera2D _camera;
        private GuiSystem _guiSystem;

        private SpriteBatch _spriteBatch;
        private ParticleEffect _particleEffect;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            IsMouseVisible = false;

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, _graphicsDeviceManager.PreferredBackBufferWidth, _graphicsDeviceManager.PreferredBackBufferHeight);
            _camera = new Camera2D(_viewportAdapter);

            var skin = GuiSkin.FromFile(Content, @"Content/adventure-gui-skin.json", typeof(MyPanel));
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, _viewportAdapter.GetScaleMatrix);

            _guiSystem = new GuiSystem(_viewportAdapter, guiRenderer);
            _guiSystem.Screens.Add(new MyScreen(skin));

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var particleTexture = new Texture2D(GraphicsDevice, 1, 1);
            particleTexture.SetData(new[] { Color.White });

            var textureRegionService = new TextureRegionService();
            textureRegionService.TextureAtlases.Add(Content.Load<TextureAtlas>("adventure-gui-atlas"));

            _particleEffect = ParticleEffect.FromFile(textureRegionService, @"Content/particle-effect.json");
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var p = _camera.ScreenToWorld(mouseState.X, mouseState.Y);

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _particleEffect.Update(deltaTime);

            if (mouseState.LeftButton == ButtonState.Pressed)
                _particleEffect.Trigger(new Vector2(p.X, p.Y));

            //_particleEffect.Trigger(new Vector2(400, 240));

            _guiSystem.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_particleEffect);
            _spriteBatch.End();

            _guiSystem.Draw(gameTime);
        }
    }
}
