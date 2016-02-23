using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.SpriteSheetAnimations
{
    public class GameMain : Game
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        private ViewportAdapter _viewportAdapter;
        private TiledMap _tiledMap;
        private FramesPerSecondCounter _fpsCounter;
        private Zombie _zombie;
        private CollisionWorld _world;

        public GameMain()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Position = new Point(50, 50);
            Window.Title = $"MonoGame.Extended - {GetType().Name}";
        }

        protected override void Initialize()
        {
            _fpsCounter = new FramesPerSecondCounter();
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter)
            {
                MinimumZoom = 0.1f,
                MaximumZoom = 2.0f,
                Zoom = 0.7833337f,
                Origin = new Vector2(400, 240),
                Position = new Vector2(408, 270)
            };

            Window.AllowUserResizing = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Content.Load<BitmapFont>("Fonts/courier-new-32");
            _tiledMap = Content.Load<TiledMap>("Tilesets/level01");

            _world = new CollisionWorld(new Vector2(0, 900));
            _world.CreateGrid(_tiledMap.GetLayer<TiledTileLayer>("Tile Layer 1"));

            var animationGroup = Content.Load<SpriteSheetAnimationGroup>("Sprites/zombie-animations");
            _zombie = new Zombie(animationGroup);
            var zombieActor = _world.CreateActor(_zombie);
            zombieActor.Position = new Vector2(462.5f, 896f);
        }

        protected override void UnloadContent()
        {
            _tiledMap.Dispose();
            _world.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            //_mousePoint = _camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y)).ToPoint();

            // camera
            if (keyboardState.IsKeyDown(Keys.R))
                _camera.ZoomIn(deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.F))
                _camera.ZoomOut(deltaSeconds);

            // zombie
            if (keyboardState.IsKeyDown(Keys.Left))
                _zombie.Walk(-1.0f);

            if (keyboardState.IsKeyDown(Keys.Right))
                _zombie.Walk(1.0f);

            if (keyboardState.IsKeyDown(Keys.Space))
                _zombie.Attack();

            if (keyboardState.IsKeyDown(Keys.Up))
                _zombie.Jump();
            
            if (keyboardState.IsKeyDown(Keys.Enter))
                _zombie.Die();

            // update must be called before collision detection
            _zombie.Update(gameTime);
            _world.Update(gameTime);
            _camera.LookAt(_zombie.Position);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _fpsCounter.Update(gameTime);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _tiledMap.Draw(_spriteBatch, _camera);
            _spriteBatch.End();

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _zombie.Draw(_spriteBatch);
            _spriteBatch.End();

            //_spriteBatch.Begin();            
            //_spriteBatch.DrawString(_bitmapFont, string.Format("FPS: {0} Zoom: {1}", _fpsCounter.AverageFramesPerSecond, _camera.Zoom), new Vector2(5, 5), new Color(0.5f, 0.5f, 0.5f));
            //_spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
