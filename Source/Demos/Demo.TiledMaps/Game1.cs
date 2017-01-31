using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
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
        private TiledMapRenderer _mapRenderer;
        private ViewportAdapter _viewportAdapter;
        private KeyboardState _previousKeyboardState = Keyboard.GetState();
        private bool _showHelp;
        private TiledMap _map;

        private Queue<string> _availableMaps;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this) 
			{
				SynchronizeWithVerticalRetrace = true,
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 768
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1024, 768);
            _camera = new Camera2D(_viewportAdapter);
            _mapRenderer = new TiledMapRenderer(GraphicsDevice);

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
            _sprite = new Sprite(_texture) {Position = new Vector2(600, 240)};

            _availableMaps =
                new Queue<string>(new[] {"level01", "level02", "level03", "level04", "level05", "level06", "level07"});

            _map = LoadNextMap();
            _camera.LookAt(new Vector2(_map.WidthInPixels, _map.HeightInPixels) * 0.5f);
        }

        private TiledMap LoadNextMap()
        {
            var name = _availableMaps.Dequeue();
            _map = Content.Load<TiledMap>(name);
            _availableMaps.Enqueue(name);
            return _map;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            _mapRenderer.Update(_map, gameTime);

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            const float cameraSpeed = 500f;
            const float zoomSpeed = 0.3f;

            var moveDirection = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                moveDirection -= Vector2.UnitY;

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                moveDirection -= Vector2.UnitX;

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                moveDirection += Vector2.UnitY;

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                moveDirection += Vector2.UnitX;

            // need to normalize the direction vector incase moving diagonally, but can't normalize the zero vector
            // however, the zero vector means we didn't want to move this frame anyways so all good
            var isCameraMoving = moveDirection != Vector2.Zero;
            if (isCameraMoving)
            {
                moveDirection.Normalize();
                _camera.Move(moveDirection * cameraSpeed * deltaSeconds);
            }

            if (keyboardState.IsKeyDown(Keys.R))
                _camera.ZoomIn(zoomSpeed * deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.F))
                _camera.ZoomOut(zoomSpeed * deltaSeconds);

            if (_previousKeyboardState.IsKeyDown(Keys.Tab) && keyboardState.IsKeyUp(Keys.Tab))
            {
                _map = LoadNextMap();
                LookAtMapCenter();
            }

            if (_previousKeyboardState.IsKeyDown(Keys.H) && keyboardState.IsKeyUp(Keys.H))
                _showHelp = !_showHelp;

            if (keyboardState.IsKeyDown(Keys.Z))
                _camera.Position = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.X))
                _camera.LookAt(Vector2.Zero);

            if (keyboardState.IsKeyDown(Keys.C))
                LookAtMapCenter();

            _sprite.Rotation += MathHelper.ToRadians(5) * deltaSeconds;
            _sprite.Position = _camera.ScreenToWorld(mouseState.X, mouseState.Y);

            _previousKeyboardState = keyboardState;

            _fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        private void LookAtMapCenter()
        {
            switch (_map.Orientation)
            {
                case TiledMapOrientation.Orthogonal:
                    _camera.LookAt(new Vector2(_map.WidthInPixels, _map.HeightInPixels) * 0.5f);
                    break;
                case TiledMapOrientation.Isometric:
                    _camera.LookAt(new Vector2(0, _map.HeightInPixels + _map.TileHeight) * 0.5f);
                    break;
                case TiledMapOrientation.Staggered:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var viewMatrix = _camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            _mapRenderer.Draw(_map, ref viewMatrix, ref projectionMatrix);

            DrawText();

            _fpsCounter.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void DrawText()
        {
            var textColor = Color.Black;
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

            var baseTextPosition = new Point2(5, 0);
            var textPosition = new Point2(0, 0);

            // textPosition = base position (point) + offset (vector2)
            textPosition = baseTextPosition + new Vector2(0, _bitmapFont.LineHeight * 0);
            _spriteBatch.DrawString(_bitmapFont,
                $"Map: {_map.Name}; {_map.TileLayers.Count} tile layer(s) @ {_map.Width}x{_map.Height} tiles, {_map.ImageLayers.Count} image layer(s)",
                textPosition, textColor);
            textPosition = baseTextPosition + new Vector2(0, _bitmapFont.LineHeight * 1);
            // we can safely get the metrics without worrying about spritebatch interfering because spritebatch submits on End()
            _spriteBatch.DrawString(_bitmapFont,
                $"FPS: {_fpsCounter.FramesPerSecond:0}, Draw Calls: {GraphicsDevice.Metrics.DrawCount}, Texture Count: {GraphicsDevice.Metrics.TextureCount}, Triangle Count: {GraphicsDevice.Metrics.PrimitiveCount}",
                textPosition, textColor);
            textPosition = baseTextPosition + new Vector2(0, _bitmapFont.LineHeight * 2);
            _spriteBatch.DrawString(_bitmapFont, $"Camera Position: (x={_camera.Position.X}, y={_camera.Position.Y})",
                textPosition, textColor);

            if (!_showHelp)
            {
                _spriteBatch.DrawString(_bitmapFont, "H: Show help", new Vector2(5, _bitmapFont.LineHeight * 3),
                    textColor);
            }
            else
            {
                textPosition = baseTextPosition + new Vector2(0, _bitmapFont.LineHeight * 3);
                _spriteBatch.DrawString(_bitmapFont, "H: Hide help", textPosition, textColor);
                textPosition = baseTextPosition + new Vector2(0, _bitmapFont.LineHeight * 4);
                _spriteBatch.DrawString(_bitmapFont, "WASD/Arrows: Pan camera", textPosition, textColor);
                textPosition = baseTextPosition + new Vector2(0, _bitmapFont.LineHeight * 5);
                _spriteBatch.DrawString(_bitmapFont, "RF: Zoom camera in / out", textPosition, textColor);
                textPosition = baseTextPosition + new Vector2(0, _bitmapFont.LineHeight * 6);
                _spriteBatch.DrawString(_bitmapFont, "Z: Move camera to the origin", textPosition, textColor);
                textPosition = baseTextPosition + new Vector2(0, _bitmapFont.LineHeight * 7);
                _spriteBatch.DrawString(_bitmapFont, "X: Move camera to look at the origin", textPosition, textColor);
                textPosition = baseTextPosition + new Vector2(0, _bitmapFont.LineHeight * 8);
                _spriteBatch.DrawString(_bitmapFont, "C: Move camera to look at center of the map", textPosition,
                    textColor);
                textPosition = baseTextPosition + new Vector2(0, _bitmapFont.LineHeight * 9);
                _spriteBatch.DrawString(_bitmapFont, "Tab: Cycle through maps", textPosition, textColor);
            }

            _spriteBatch.End();
        }
    }
}