using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.NuclexGui;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.NuclexGui.Controls.Desktop;

namespace Demo.NuclexGui
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Camera2D _camera;

        InputManager _inputManager;
        GuiManager _gui;
        int rotation = 1;
        Color background;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            background = Color.Black;

            _inputManager = new InputManager(Services);
            _gui = new GuiManager(Services);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _gui.Screen = new GuiScreen(800, 480);
            _gui.Screen.Desktop.Bounds = new UniRectangle(new UniScalar(0f, 0), new UniScalar(0f, 0), new UniScalar(1f, 0), new UniScalar(1f, 0));

            _gui.Initialize();

            GuiButtonControl button = new GuiButtonControl
            {
                Name = "button",
                Bounds = new UniRectangle(new UniScalar(0.0f, 20), new UniScalar(0.0f, 20), new UniScalar(0f, 120), new UniScalar(0f, 50)),
                Text = "Rotate logo"
            };
            GuiButtonControl button2 = new GuiButtonControl
            {
                Name = "button2",
                Bounds = new UniRectangle(new UniScalar(20), new UniScalar(80), new UniScalar(120), new UniScalar(50)),
                Text = "Open Window"
            };


            button.Pressed += Button_Pressed;
            button2.Pressed += Button2_Pressed;

            _gui.Screen.Desktop.Children.Add(button);
            _gui.Screen.Desktop.Children.Add(button2);
        }

        private void Button2_Pressed(object sender, System.EventArgs e)
        {
            GuiWindowControl window = new GuiWindowControl
            {
                Name = "window",
                Bounds = new UniRectangle(new UniVector(new UniScalar(0.5f, -100), new UniScalar(0.5f, -60)), new UniVector(new UniScalar(200), new UniScalar(120))),
                Title = "Color picker",
                EnableDragging = true
            };

            GuiChoiceControl choice1 = new GuiChoiceControl
            {
                Name = "choiceBlack",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(0.0f, 30), new UniScalar(10), new UniScalar(10)),
                Text = "Black",
                Selected = true
            };
            GuiChoiceControl choice2 = new GuiChoiceControl
            {
                Name = "choiceGray",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(0.0f, 45), new UniScalar(10), new UniScalar(10)),
                Text = "Gray",
                Selected = false
            };
            GuiChoiceControl choice3 = new GuiChoiceControl
            {
                Name = "choiceWhite",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(0.0f, 60), new UniScalar(10), new UniScalar(10)),
                Text = "White",
                Selected = false
            };
            GuiButtonControl button1 = new GuiButtonControl
            {
                Name = "confirm",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(1.0f, -40), new UniScalar(0f, 80), new UniScalar(0f, 30)),
                Text = "Confirm"
            };
            GuiButtonControl button2 = new GuiButtonControl
            {
                Name = "cancel",
                Bounds = new UniRectangle(new UniScalar(1.0f, -90), new UniScalar(1.0f, -40), new UniScalar(0f, 80), new UniScalar(0f, 30)),
                Text = "Cancel"
            };

            button1.Pressed += DialogueConfirm_Pressed;
            button2.Pressed += DialogueCancel_Pressed;

            window.Children.Add(choice1);
            window.Children.Add(choice2);
            window.Children.Add(choice3);
            window.Children.Add(button1);
            window.Children.Add(button2);

            _gui.Screen.Desktop.Children.Add(window);
        }

        private void DialogueCancel_Pressed(object sender, System.EventArgs e)
        {
            _gui.Screen.Desktop.Children.Remove(((GuiButtonControl)sender).Parent);
        }

        private void DialogueConfirm_Pressed(object sender, System.EventArgs e)
        {
            foreach (var control in ((GuiButtonControl)sender).Parent.Children)
            {
                if (control.GetType() == typeof(GuiChoiceControl))
                {
                    var radiobutton = (GuiChoiceControl)control;

                    if (radiobutton.Selected)
                        switch (radiobutton.Name)
                        {
                            case "choiceGray":
                                background = Color.Gray;
                                break;

                            case "choiceWhite":
                                background = Color.White;
                                break;

                            default:
                                background = Color.Black;
                                break;
                        }
                }
            }

            _gui.Screen.Desktop.Children.Remove(((GuiButtonControl)sender).Parent);
        }

        private void Button_Pressed(object sender, System.EventArgs e)
        {
            rotation *= -1;
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

            _inputManager.Update(gameTime);
            _gui.Update(gameTime);

            _sprite.Rotation += deltaTime * rotation;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());

            _spriteBatch.Draw(_sprite);
            _spriteBatch.End();

            _gui.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}