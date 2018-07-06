﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.NuclexGui;
using MonoGame.Extended.NuclexGui.Controls;
using MonoGame.Extended.NuclexGui.Controls.Desktop;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace NuclexGui
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Transform2 _transform;
        private OrthographicCamera _camera;

        private readonly InputListenerComponent _inputManager;
        private readonly GuiManager _gui;
        private int _rotateDirection = 1;
        private Color _backgroundColor;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            _backgroundColor = Color.Black;

            // First, we create an input manager.
            _inputManager = new InputListenerComponent(this);

            // Then, we create GUI.
            var guiInputService = new GuiInputService(_inputManager);
            _gui = new GuiManager(Services, guiInputService);
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a GUI screen and attach it as a default to GuiManager.
            // That screen will also act as a root parent for every other control that we create.
            _gui.Screen = new GuiScreen(800, 480);
            _gui.Screen.Desktop.Bounds = new UniRectangle(new UniScalar(0f, 0), new UniScalar(0f, 0), new UniScalar(1f, 0), new UniScalar(1f, 0));

            // Perform second-stage initialization
            _gui.Initialize();

            // Create few controls.
            var button = new GuiButtonControl
            {
                Name = "button",
                Bounds = new UniRectangle(new UniScalar(0.0f, 20), new UniScalar(0.0f, 20), new UniScalar(0f, 120), new UniScalar(0f, 50)),
                Text = "Rotate logo"
            };
            var button2 = new GuiButtonControl
            {
                Name = "button2",
                Bounds = new UniRectangle(new UniScalar(20), new UniScalar(80), new UniScalar(120), new UniScalar(50)),
                Text = "Open Window"
            };
            var button3 = new GuiButtonControl()
            {
                Name = "button3",
                Bounds = new UniRectangle(new UniScalar(20), new UniScalar(140), new UniScalar(120), new UniScalar(50)),
                Text = "Show Labels",
            };

            // Attach relevant events
            button.Pressed += Button_Pressed;
            button2.Pressed += Button2_Pressed;
            button3.Pressed += Button3_Pressed;

            // And finally, attach controls to the parent control. In this case, desktop screen.
            _gui.Screen.Desktop.Children.Add(button);
            _gui.Screen.Desktop.Children.Add(button2);
            _gui.Screen.Desktop.Children.Add(button3);
        }

        private void Button3_Pressed(object sender, EventArgs e)
        {
            if (_gui.Screen.Desktop.Children.Any(i => i.Name == "window"))
                return;

            var window = new GuiWindowControl
            {
                Name = "window",
                Bounds = new UniRectangle(new UniVector(new UniScalar(0.5f, -100), new UniScalar(0.5f, -60)), new UniVector(new UniScalar(200), new UniScalar(160))),
                Title = "Labels with Styles",
                EnableDragging = true
            };

            var labelTitle = new GuiLabelControl
            {
                Name = "label-title",
                Bounds = new UniRectangle(new UniScalar(10), new UniScalar(45), new UniScalar(10), new UniScalar(10)),
                Text = "Title",
                Style = "label-title"
            };
            var label1 = new GuiLabelControl
            {
                Name = "label1",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(0.0f, 70), new UniScalar(10), new UniScalar(10)),
                Text = "Heading 1",
                Style = "label-h1",
            };
            var label2 = new GuiLabelControl
            {
                Name = "label2",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(0.0f, 90), new UniScalar(10), new UniScalar(10)),
                Text = "Heading 2",
                Style = "label-h2",
            };
            var label3 = new GuiLabelControl
            {
                Name = "label3",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(0.0f, 105), new UniScalar(10), new UniScalar(10)),
                Text = "Heading 3",
                Style = "label-h3"
            };

            var button1 = new GuiButtonControl
            {
                Name = "confirm",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(1.0f, -40), new UniScalar(0f, 80), new UniScalar(0f, 30)),
                Text = "Confirm"
            };
            var button2 = new GuiButtonControl
            {
                Name = "cancel",
                Bounds = new UniRectangle(new UniScalar(1.0f, -90), new UniScalar(1.0f, -40), new UniScalar(0f, 80), new UniScalar(0f, 30)),
                Text = "Cancel"
            };

            button1.Pressed += DialogueConfirm_Pressed;
            button2.Pressed += DialogueCancel_Pressed;

            window.Children.Add(labelTitle);
            window.Children.Add(label1);
            window.Children.Add(label2);
            window.Children.Add(label3);
            window.Children.Add(button1);
            window.Children.Add(button2);

            _gui.Screen.Desktop.Children.Add(window);
        }

        private void Button2_Pressed(object sender, System.EventArgs e)
        {
            if(_gui.Screen.Desktop.Children.Any(i => i.Name == "window"))
                return;

            var window = new GuiWindowControl
            {
                Name = "window",
                Bounds = new UniRectangle(new UniVector(new UniScalar(0.5f, -100), new UniScalar(0.5f, -60)), new UniVector(new UniScalar(200), new UniScalar(120))),
                Title = "Color picker",
                EnableDragging = true
            };

            var choice1 = new GuiChoiceControl
            {
                Name = "choiceBlack",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(0.0f, 30), new UniScalar(10), new UniScalar(10)),
                Text = "Black",
                Selected = true
            };
            var choice2 = new GuiChoiceControl
            {
                Name = "choiceGray",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(0.0f, 45), new UniScalar(10), new UniScalar(10)),
                Text = "Gray",
                Selected = false
            };
            var choice3 = new GuiChoiceControl
            {
                Name = "choiceWhite",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(0.0f, 60), new UniScalar(10), new UniScalar(10)),
                Text = "White",
                Selected = false
            };
            var button1 = new GuiButtonControl
            {
                Name = "confirm",
                Bounds = new UniRectangle(new UniScalar(0.0f, 10), new UniScalar(1.0f, -40), new UniScalar(0f, 80), new UniScalar(0f, 30)),
                Text = "Confirm"
            };
            var button2 = new GuiButtonControl
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
                                _backgroundColor = Color.Gray;
                                break;

                            case "choiceWhite":
                                _backgroundColor = Color.White;
                                break;

                            default:
                                _backgroundColor = Color.Black;
                                break;
                        }
                }
            }

            _gui.Screen.Desktop.Children.Remove(((GuiButtonControl)sender).Parent);
        }

        private void Button_Pressed(object sender, System.EventArgs e)
        {
            _rotateDirection *= -1;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new OrthographicCamera(viewportAdapter);

            var logoTexture = Content.Load<Texture2D>("logo-square-128");
            _sprite = new Sprite(logoTexture);
            _transform = new Transform2
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

            // Update both InputManager (which updates states of each device) and GUI
            _inputManager.Update(gameTime);
            _gui.Update(gameTime);

            _transform.Rotation += deltaTime * _rotateDirection;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());

            _spriteBatch.Draw(_sprite, _transform);
            _spriteBatch.End();

            // Draw GUI on top of everything
            _gui.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}