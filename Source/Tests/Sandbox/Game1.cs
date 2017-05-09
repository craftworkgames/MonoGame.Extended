using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;

// The Sandbox project is used for experiementing outside the normal demos.
// Any code found here should be considered experimental work in progress.
namespace Sandbox
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private ViewportAdapter _viewportAdapter;
        private Camera2D _camera;
        private GuiSystem _guiSystem;

        private SpriteBatch _spriteBatch;
        private ParticleEffect _particleEffect;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            IsMouseVisible = false;

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter);

            var skin = GuiSkin.FromFile(Content, @"Content/adventure-gui-skin.json");
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, _camera.GetViewMatrix);

            _guiSystem = new GuiSystem(_viewportAdapter, guiRenderer)
            {
                Screen = new GuiScreen(skin)
                {
                    Controls =
                    {
                        //new GuiLabel { Text = "Hello World" }
                    }
                }
            };

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            var particleTexture = new Texture2D(GraphicsDevice, 1, 1);
            particleTexture.SetData(new[] { Color.White });

            var textureRegionService = new TextureRegionService();
            textureRegionService.TextureAtlases.Add(Content.Load<TextureAtlas>("adventure-gui-atlas"));

            _particleEffect = ParticleEffect.FromFile(textureRegionService, @"Content/particle-effect.json");

            //ParticleInit(new TextureRegion2D(particleTexture));
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

            _particleEffect.Trigger(new Vector2(400, 240));

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

        private ParticleEffect ParticleInit(TextureRegion2D textureRegion)
        {
            return _particleEffect = new ParticleEffect
            {
                Emitters = new[]
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(2.5), Profile.Ring(150f, Profile.CircleRadiation.In))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 50f),
                            Quantity = 3,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(3.0f, 4.0f)
                        },
                        Modifiers = new IModifier[]
                        {
                            new AgeModifier
                            {
                                Interpolators = new IInterpolator[]
                                {
                                    new ColorInterpolator
                                    {
                                        InitialColor = new HslColor(0.33f, 0.5f, 0.5f),
                                        FinalColor = new HslColor(0.5f, 0.9f, 1.0f)
                                    }
                                }
                            },
                            new RotationModifier {RotationRate = -2.1f},
                            new RectangleContainerModifier {Width = 800, Height = 480},
                            new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 30f}
                        }
                    }
                }
            };
        }
    }
}
