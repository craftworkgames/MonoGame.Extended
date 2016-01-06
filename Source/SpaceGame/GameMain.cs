using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using SpaceGame.Entities;

namespace SpaceGame
{
    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Texture2D _backgroundTexture;
        private Spaceship _player;
        private readonly Random _random;
        private readonly EntityManager _entityManager;
        private Camera2D _camera;
        private BitmapFont _font;
        private int _score;

        public GameMain()
        {
            _random = new Random();
            _entityManager = new EntityManager();
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            //{
                //IsFullScreen = true
            //};
            //_graphicsDeviceManager.ApplyChanges();

            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

        }

        protected override void LoadContent()
        {
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter);
            _font = Content.Load<BitmapFont>("Fonts/courier-new-32");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundTexture = Content.Load<Texture2D>("black");

            var bulletTexture = Content.Load<Texture2D>("laserBlue03");
            var bulletRegion = new TextureRegion2D(bulletTexture);
            var bulletFactory = new BulletFactory(_entityManager, bulletRegion);

            var spaceshipTexture = Content.Load<Texture2D>("playerShip1_blue");
            var spaceshipRegion = new TextureRegion2D(spaceshipTexture);
            _player = _entityManager.AddEntity(new Spaceship(spaceshipRegion, bulletFactory));

            _meteorFactory = new MeteorFactory(_entityManager, Content);
            var meteorTexture = Content.Load<Texture2D>("meteorBrown_big1");
            _meteorRegion = new TextureRegion2D(meteorTexture);

            for (var i = 0; i < 13; i++)
                _meteorFactory.SpawnNewMeteor(_player.Position);
        }


        protected override void UnloadContent()
        {
        }

        private MouseState _previousMouseState;
        private ViewportAdapter _viewportAdapter;
        private TextureRegion2D _meteorRegion;
        private MeteorFactory _meteorFactory;

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            const float acceleration = 5f;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                _player.Accelerate(acceleration);

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                _player.Accelerate(-acceleration);

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                _player.Rotation -= deltaTime * 3f;

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                _player.Rotation += deltaTime * 3f;

            if (keyboardState.IsKeyDown(Keys.Space) || mouseState.LeftButton == ButtonState.Pressed)
                _player.Fire();

            if(_previousMouseState.X != mouseState.X || _previousMouseState.Y != mouseState.Y)
                _player.LookAt(_camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y)));

            _entityManager.Update(gameTime);

            CheckMeteorLaserCollisions();

            _previousMouseState = mouseState;
            _camera.LookAt(_player.Position +  _player.Velocity * 0.2f);

            base.Update(gameTime);
        }

        private void CheckMeteorLaserCollisions()
        {
            var meteors = _entityManager.Entities.Where(e => e is Meteor).Cast<Meteor>().ToArray();
            var lasers = _entityManager.Entities.Where(e => e is Laser).Cast<Laser>().ToArray();

            foreach (var laser in lasers)
            {
                foreach (var meteor in meteors)
                {
                    if (meteor.Contains(laser.Position))
                    {
                        meteor.Damage(1);
                        laser.Destroy();
                        _score++;

                        var animator = Content.Load<SpriteSheetAnimator>("explosion-animations");
                        _entityManager.AddEntity(new Explosion(animator, laser.Position));

                        if(meteor.Size >= 2)
                            _meteorFactory.SplitMeteor(meteor);
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // background
            var sourceRectangle = new Rectangle(0, 0, _viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight);
            sourceRectangle.Offset(_camera.Position);
            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_backgroundTexture, _camera.Position, sourceRectangle, Color.White);
            _spriteBatch.End();

            // entities
            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp, transformMatrix: _camera.GetViewMatrix());
            _entityManager.Draw(_spriteBatch);
            _spriteBatch.End();

            // hud
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, string.Format("Score: {0}", _score), Vector2.One, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
