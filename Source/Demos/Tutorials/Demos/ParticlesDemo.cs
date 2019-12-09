using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Tutorials.Demos
{
    public class ParticlesDemo : DemoBase
    {
        public override string Name => "Particles";

        public ParticlesDemo(GameMain game) 
            : base(game)
        {
        }

        private SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Transform2 _transform;
        private OrthographicCamera _camera;
        private ParticleEffect _particleEffect;
        private Texture2D _particleTexture;

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new OrthographicCamera(viewportAdapter);

            var logoTexture = Content.Load<Texture2D>("Textures/logo-square-128");
            _sprite = new Sprite(logoTexture);
            _transform = new Transform2 { Position = viewportAdapter.Center.ToVector2()};

            _particleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _particleTexture.SetData(new[] {Color.White});

            ParticleInit(new TextureRegion2D(_particleTexture));
        }

        protected override void UnloadContent()
        {
            // any content not loaded with the content manager should be disposed
            _particleTexture.Dispose();
            _particleEffect.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var p = _camera.ScreenToWorld(mouseState.X, mouseState.Y);

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _transform.Rotation += deltaTime;

            _particleEffect.Update(deltaTime);

            if (mouseState.LeftButton == ButtonState.Pressed)
                _particleEffect.Trigger(new Vector2(p.X, p.Y));

            //_particleEffect.Position = new Vector2(400, 240);
            //_particleEffect.Trigger(new Vector2(400, 240));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_particleEffect);
            _spriteBatch.Draw(_sprite, _transform.Position, _transform.Rotation, _transform.Scale);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ParticleInit(TextureRegion2D textureRegion)
        {
            _particleEffect = new ParticleEffect(autoTrigger: false)
            {
                Position = new Vector2(400, 240),
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(2.5),
                        Profile.Ring(150f, Profile.CircleRadiation.In))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 50f),
                            Quantity = 3,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(3.0f, 4.0f)
                        },
                        Modifiers = 
                        {
                            new AgeModifier
                            {
                                Interpolators = 
                                {
                                    new ColorInterpolator
                                    {
                                        StartValue = new HslColor(0.33f, 0.5f, 0.5f),
                                        EndValue = new HslColor(0.5f, 0.9f, 1.0f)
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
