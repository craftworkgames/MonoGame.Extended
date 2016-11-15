using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Maps.Renderers;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.TiledMaps
{
    public class Game1 : Game
    {
        private FramesPerSecondCounter _fpsCounter;
        private BitmapFont _bitmapFont;
        private Camera2D _camera;
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private Sprite _sprite;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private IMapRenderer _mapRenderer;
        private ViewportAdapter _viewportAdapter;
        private KeyboardState _oldKeyboardState = Keyboard.GetState();
        private bool _showHelp;
        private TiledMap _tiledMap;

        private Queue<string> _availableMaps;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this) { SynchronizeWithVerticalRetrace = false };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter);
            _mapRenderer = new FullMapRenderer(GraphicsDevice);

            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;

            _fpsCounter = new FramesPerSecondCounter();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texture = Content.Load<Texture2D>("monogame-extended-logo");
            _bitmapFont = Content.Load<BitmapFont>("montserrat-32");
            _sprite = new Sprite(_texture) { Position = new Vector2(600, 240) };

            _availableMaps = new Queue<string>(new[] { "level01", "level02", "level03", "level04", "level05", "very-large" });

            _tiledMap = LoadNextMap();
            _mapRenderer.SwapMap(_tiledMap);
            _camera.LookAt(new Vector2(_tiledMap.WidthInPixels, _tiledMap.HeightInPixels) * 0.5f);
        }

        private TiledMap LoadNextMap()
        {
            var name = _availableMaps.Dequeue();
            _tiledMap = Content.Load<TiledMap>(name);
            _availableMaps.Enqueue(name);
            return _tiledMap;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            _mapRenderer.Update(gameTime);
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            const float cameraSpeed = 500f;
            const float zoomSpeed = 0.3f;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                _camera.Move(new Vector2(0, -cameraSpeed) * deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                _camera.Move(new Vector2(-cameraSpeed, 0) * deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                _camera.Move(new Vector2(0, cameraSpeed) * deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                _camera.Move(new Vector2(cameraSpeed, 0) * deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.R))
                _camera.ZoomIn(zoomSpeed * deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.F))
                _camera.ZoomOut(zoomSpeed * deltaSeconds);

            if (_oldKeyboardState.IsKeyDown(Keys.Tab) && keyboardState.IsKeyUp(Keys.Tab))
            {
                _tiledMap = LoadNextMap();
                _mapRenderer.SwapMap(_tiledMap);
                _camera.LookAt(new Vector2(_tiledMap.WidthInPixels, _tiledMap.HeightInPixels) * 0.5f);
            }

            if (_oldKeyboardState.IsKeyDown(Keys.H) && keyboardState.IsKeyUp(Keys.H))
                _showHelp = !_showHelp;

            _sprite.Rotation += MathHelper.ToRadians(5) * deltaSeconds;
            _sprite.Position = _camera.ScreenToWorld(mouseState.X, mouseState.Y);

            _fpsCounter.Update(gameTime);

            _oldKeyboardState = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _mapRenderer.Draw(_camera.GetViewMatrix());

            var textColor = Color.Black;
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            _spriteBatch.DrawString(_bitmapFont, $"Map: {_tiledMap.Name}", new Vector2(5, _bitmapFont.LineHeight * 0), Color.Black);
            _spriteBatch.DrawString(_bitmapFont, $"FPS: {_fpsCounter.FramesPerSecond:0}", new Vector2(5, _bitmapFont.LineHeight * 1), Color.Black);
            _spriteBatch.DrawString(_bitmapFont, $"Camera: {_camera.Position}", new Vector2(5, _bitmapFont.LineHeight * 2), Color.Black);

            if (_showHelp)
            {
                _spriteBatch.DrawString(_bitmapFont, "H: Hide help", new Vector2(5, _bitmapFont.LineHeight * 3), textColor);
                _spriteBatch.DrawString(_bitmapFont, "WASD/Arrows: move", new Vector2(5, _bitmapFont.LineHeight * 4), textColor);
                _spriteBatch.DrawString(_bitmapFont, "RF: zoom", new Vector2(5, _bitmapFont.LineHeight * 5), textColor);
                _spriteBatch.DrawString(_bitmapFont, "Tab: Switch map", new Vector2(5, _bitmapFont.LineHeight * 6), textColor);
            }
            else
            {
                _spriteBatch.DrawString(_bitmapFont, "H: Show help", new Vector2(5, _bitmapFont.LineHeight * 3), textColor);
            }

            _spriteBatch.End();

            _fpsCounter.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}