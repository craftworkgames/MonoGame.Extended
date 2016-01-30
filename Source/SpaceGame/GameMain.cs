using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Drawables;
using MonoGame.Extended.Primitives;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using SpaceGame.Entities;

namespace SpaceGame
{
    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly EntityManager _entityManager;
        private readonly ScreenManager _screenManager;
        private GuiManager _guiManager;

        private SpriteBatch _spriteBatch;
        private Texture2D _backgroundTexture;
        private Spaceship _player;
        private Camera2D _camera;
        private BitmapFont _font;
        private MouseState _previousMouseState;
        private ViewportAdapter _viewportAdapter;
        private MeteorFactory _meteorFactory;
        private SpriteSheetAnimationGroup _explosionAnimations;
        private BulletFactory _bulletFactory;
        private int _score;
        private GuiLabel _scoreLabel;

        public GameMain()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _screenManager = new ScreenManager(this);
            _entityManager = new EntityManager();

            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        //protected override void Initialize()
        //{
        //    base.Initialize();

        //    _graphicsDeviceManager.IsFullScreen = true;
        //    _graphicsDeviceManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
        //    _graphicsDeviceManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
        //    _graphicsDeviceManager.ApplyChanges();
        //}

        protected override void LoadContent()
        {
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _guiManager = new GuiManager(_viewportAdapter, GraphicsDevice);
            _font = Content.Load<BitmapFont>("Fonts/courier-new-32");

            //var textureRegion = new TextureRegion2D(Content.Load<Texture2D>("Gui/9patch-2"));
            //var dialogPatch = new GuiPatchDrawable(textureRegion, 100, 100, 122, 111, Color.White);
            //var dialogStyle = new GuiButtonStyle(dialogPatch);
            //var dialog = new GuiButton(dialogStyle)
            //{
            //    HorizontalAlignment = GuiHorizontalAlignment.Stretch,
            //    VerticalAlignment = GuiVerticalAlignment.Stretch
            //};
            //_guiManager.Layout.Children.Add(dialog);

            var checkedOn = Content.Load<Texture2D>("Gui/button-clicked").ToGuiDrawable();
            var checkedOff = Content.Load<Texture2D>("Gui/button-normal").ToGuiDrawable();
            var checkBoxStyle = new GuiCheckBoxStyle(checkedOn, checkedOff);
            var checkBox = new GuiCheckBox(checkBoxStyle) {HorizontalAlignment = GuiHorizontalAlignment.Left};
            _guiManager.Layout.Children.Add(checkBox);

            var normal = Content.Load<Texture2D>("Gui/button-normal").ToGuiDrawable();
            var pressed = Content.Load<Texture2D>("Gui/button-clicked").ToGuiDrawable();
            var hover = Content.Load<Texture2D>("Gui/button-hover").ToGuiDrawable();
            var buttonStyle = new GuiButtonStyle(normal, pressed, hover);
            var button = new GuiButton(buttonStyle) {VerticalAlignment = GuiVerticalAlignment.Bottom};
            button.Clicked += (sender, args) =>
            {
                if (_player != null)
                {
                    Explode(_player.Position, 3);
                    _player.Destroy();
                    _player = null;
                }
            };
            _guiManager.Layout.Children.Add(button);

            var labelStyle = new GuiLabelStyle(_font);
            _scoreLabel = new GuiLabel(labelStyle, "Hello")
            {
                HorizontalAlignment = GuiHorizontalAlignment.Right,
                VerticalAlignment = GuiVerticalAlignment.Top
            };
            _guiManager.Layout.Children.Add(_scoreLabel);




            _guiManager.PerformLayout();

            _camera = new Camera2D(_viewportAdapter);
            _explosionAnimations = Content.Load<SpriteSheetAnimationGroup>("explosion-animations");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundTexture = Content.Load<Texture2D>("black");

            var bulletTexture = Content.Load<Texture2D>("laserBlue03");
            var bulletRegion = new TextureRegion2D(bulletTexture);
            _bulletFactory = new BulletFactory(_entityManager, bulletRegion);

            SpawnPlayer(_bulletFactory);

            _meteorFactory = new MeteorFactory(_entityManager, Content);

            for (var i = 0; i < 13; i++)
                _meteorFactory.SpawnNewMeteor(_player.Position);
        }

        private void SpawnPlayer(BulletFactory bulletFactory)
        {
            var spaceshipTexture = Content.Load<Texture2D>("playerShip1_blue");
            var spaceshipRegion = new TextureRegion2D(spaceshipTexture);
            _player = _entityManager.AddEntity(new Spaceship(spaceshipRegion, bulletFactory));
        }


        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (_player != null && !_player.IsDestroyed)
            {
                const float acceleration = 5f;

                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                    _player.Accelerate(acceleration);

                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                    _player.Accelerate(-acceleration);

                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                    _player.Rotation -= deltaTime*3f;

                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                    _player.Rotation += deltaTime*3f;

                if (keyboardState.IsKeyDown(Keys.Space) || mouseState.LeftButton == ButtonState.Pressed)
                    _player.Fire();

                if (_previousMouseState.X != mouseState.X || _previousMouseState.Y != mouseState.Y)
                    _player.LookAt(_camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y)));

                _camera.LookAt(_player.Position + _player.Velocity * 0.2f);
                _camera.Zoom = 1.0f - (_player.Velocity.Length() / 500f);
            }

            _entityManager.Update(gameTime);

            CheckCollisions();

            _previousMouseState = mouseState;


            _scoreLabel.Text = string.Format("Score: {0}", _score);

            _guiManager.Update(gameTime);
            _guiManager.PerformLayout(); // not ideal.

            base.Update(gameTime);
        }

        private void CheckCollisions()
        {
            var meteors = _entityManager.Entities.Where(e => e is Meteor).Cast<Meteor>().ToArray();
            var lasers = _entityManager.Entities.Where(e => e is Laser).Cast<Laser>().ToArray();

            foreach (var meteor in meteors)
            {
                if (_player != null && !_player.IsDestroyed && _player.BoundingCircle.Intersects(meteor.BoundingCircle))
                {
                    Explode(meteor.Position, meteor.Size);
                    Explode(_player.Position, 3);

                    _player.Destroy();
                    _player = null;
                    meteor.Destroy();
                }

                foreach (var laser in lasers.Where(laser => meteor.Contains(laser.Position)))
                {
                    meteor.Damage(1);
                    laser.Destroy();
                    _score++;

                    Explode(laser.Position, meteor.Size);

                    if(meteor.Size >= 2)
                        _meteorFactory.SplitMeteor(meteor);
                }

                if (_player != null && _shieldHealth > 0 && meteor.BoundingCircle.Intersects(new CircleF(_player.Position, 100 + (_shieldHealth - 10) * 5)))
                {
                    _shieldHealth--;
                    Explode(meteor.Position, meteor.Size);
                    meteor.Destroy();
                }
            }
        }

        private void Explode(Vector2 position, float radius)
        {
            var explosion = new Explosion(_explosionAnimations, position, radius);
            _entityManager.AddEntity(explosion);
        }

        private int _shieldHealth = 10;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // background
            var sourceRectangle = new Rectangle(0, 0, _viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight);
            sourceRectangle.Offset(_camera.Position);
            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap);
            _spriteBatch.Draw(_backgroundTexture, Vector2.Zero, sourceRectangle, Color.White);
            _spriteBatch.End();

            // entities
            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _entityManager.Draw(_spriteBatch);
            _spriteBatch.End();

            _guiManager.Draw(gameTime);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            _spriteBatch.DrawRectangle(new Rectangle(100, 100, 200, 300), Color.CornflowerBlue);
            _spriteBatch.DrawLine(new Vector2(100, 100), new Vector2(300, 400), Color.Red,1);

            if (_player != null && _shieldHealth > 0)
            {
                _spriteBatch.DrawCircle(_player.Position, 100 + (_shieldHealth - 10) * 5, 16, Color.Green, _shieldHealth);
                _spriteBatch.DrawArc(_player.Position, 90, 16, 0, MathHelper.ToRadians(90), Color.Yellow);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
