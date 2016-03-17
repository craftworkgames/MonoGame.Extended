using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Particles
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Camera2D _camera;
        private ParticleEffect _particleEffect;
        private SpriteBatchRenderer _particleRenderer;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
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

            var particleTexture = new Texture2D(GraphicsDevice, 1, 1);
            particleTexture.SetData(new[] { Color.White });

            ParticleInit(particleTexture);
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

            _sprite.Rotation += deltaTime;

            _particleEffect.Update(deltaTime);
            _particleEffect.Trigger(new Vector(400, 240));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _particleRenderer.Draw(_spriteBatch, _particleEffect);
            //_spriteBatch.Draw(_sprite);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ParticleInit(Texture2D texture)
        {
            _particleRenderer = new SpriteBatchRenderer();
            _particleEffect = new ParticleEffect
            {
                Emitters = new[]
                {
                    new Emitter(2000, TimeSpan.FromSeconds(4), Profile.Spray(Axis.Up, 1f))
                    {
                        Texture = texture,
                        BlendMode = BlendMode.Alpha,
                        Parameters = new ReleaseParameters { Speed = new RangeF(120f, 150f), Quantity = 3 },
                        Modifiers = new IModifier[]
                        {
                            new ColourInterpolator2 { InitialColour = new Colour(0.33f, 0.5f, 0.5f), FinalColour = new Colour(0f, 0.5f, 0.5f) },
                            new RotationModifier { RotationRate = 1f },
                            new RectContainerModifier {  Width = 800, Height = 480 },
                            new LinearGravityModifier { Direction = Axis.Down, Strength = 100f }
                        }
                    }

                }
            };

        }
    }
}