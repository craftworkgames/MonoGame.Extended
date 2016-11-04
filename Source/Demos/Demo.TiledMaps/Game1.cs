﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
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
        private TiledMap _tiledMap;
        private ViewportAdapter _viewportAdapter;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this) {SynchronizeWithVerticalRetrace = false};
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter);

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

            _tiledMap = Content.Load<TiledMap>("level01");
            _camera.LookAt(new Vector2(_tiledMap.WidthInPixels, _tiledMap.HeightInPixels) * 0.5f);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            const float cameraSpeed = 500f;
            const float zoomSpeed = 0.3f;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                _camera.Move(new Vector2(0, -cameraSpeed)*deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                _camera.Move(new Vector2(-cameraSpeed, 0)*deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                _camera.Move(new Vector2(0, cameraSpeed)*deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                _camera.Move(new Vector2(cameraSpeed, 0)*deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.R))
                _camera.ZoomIn(zoomSpeed * deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.F))
                _camera.ZoomOut(zoomSpeed * deltaSeconds);

            _sprite.Rotation += MathHelper.ToRadians(5) * deltaSeconds;
            _sprite.Position = _camera.ScreenToWorld(mouseState.X, mouseState.Y);

            _fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // you can draw the whole map all at once
            _tiledMap.Draw(_camera.GetViewMatrix());

            // or you can have more control over drawing each individual layer
            //foreach (var layer in _tiledMap.Layers)
            //{
            //    _spriteBatch.Draw(_sprite);
            //    _spriteBatch.Draw(layer, _camera);
            //}

            var textColor = Color.Black;
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            _spriteBatch.DrawString(_bitmapFont, "WASD/Arrows: move", new Vector2(5, 32), textColor);
            _spriteBatch.DrawString(_bitmapFont, "RF: zoom", new Vector2(5, 32 + _bitmapFont.LineHeight), textColor);
            _spriteBatch.DrawString(_bitmapFont, $"FPS: {_fpsCounter.FramesPerSecond:0}", Vector2.One, Color.Black);
            _spriteBatch.DrawString(_bitmapFont, $"Camera: {_camera.Position}", new Vector2(5, 32 + _bitmapFont.LineHeight * 2), Color.Black);
            _spriteBatch.End();

            _fpsCounter.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}