using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.EntityComponentSystem
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private Camera2D _camera;

        private MonoGame.Extended.Entities.EntityComponentSystem _entityComponentSystem;
        private Entity _entity;

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

            _entityComponentSystem = new MonoGame.Extended.Entities.EntityComponentSystem();
            _entityComponentSystem.RegisterSystem(new SpriteBatchSystem(GraphicsDevice, _camera));
            _entityComponentSystem.RegisterSystem(new AnimatedSpriteSystem());
            _entityComponentSystem.RegisterSystem(new ParticleEmitterSystem());

            var logoTexture = Content.Load<Texture2D>("logo-square-128");
            _entity = _entityComponentSystem.CreateEntity("logo", new Vector2(400, 240));
            _entity.AttachComponent(new Sprite(logoTexture));
            
            var motwTexture = Content.Load<Texture2D>("motw");
            var motwAtlas = TextureAtlas.Create("motw-atlas", motwTexture, 52, 72);
            var motwAnimationFactory = new SpriteSheetAnimationFactory(motwAtlas);
            motwAnimationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            motwAnimationFactory.Add("walkSouth", new SpriteSheetAnimationData(new[] { 0, 1, 2, 1 }, isLooping: false));
            motwAnimationFactory.Add("walkWest", new SpriteSheetAnimationData(new[] { 12, 13, 14, 13 }, isLooping: false));
            motwAnimationFactory.Add("walkEast", new SpriteSheetAnimationData(new[] { 24, 25, 26, 25 }, isLooping: false));
            motwAnimationFactory.Add("walkNorth", new SpriteSheetAnimationData(new[] { 36, 37, 38, 37 }, isLooping: false));

            var animatedEntity = _entityComponentSystem.CreateEntity("animated", new Vector2(50, 50));
            animatedEntity.AttachComponent(new AnimatedSprite(motwAnimationFactory, "walkSouth"));

            var particleEntity = _entityComponentSystem.CreateEntity("particles", new Vector2(500, 50));
            var particleEmitter = new ParticleEmitter(new TextureRegion2D(logoTexture), 500, TimeSpan.FromSeconds(0.5f),
                Profile.Point())
            {
                Parameters = new ParticleReleaseParameters
                {
                    Speed = new Range<float>(0f, 150f),
                    Quantity = 30,
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
                                InitialColor = new HslColor(0.8f, 0.8f, 0.8f),
                                FinalColor = new HslColor(0.5f, 0.9f, 1.0f)
                            }
                        }
                    },
                    new RotationModifier {RotationRate = -2.1f},
                    new RectangleContainerModifier {Width = 800, Height = 480},
                    new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 130f}
                }
            };
            particleEntity.AttachComponent(particleEmitter);
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

            _entity.Rotation += deltaTime;
            _entityComponentSystem.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _entityComponentSystem.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}