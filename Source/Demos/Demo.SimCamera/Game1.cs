using FarseerPhysics.Common;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Text;

namespace Demo.SimCamera
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private World _world;
        private DebugViewXNA _debugView;

        private Block _blockControlled;
        private Block _block;

        private Camera2D _camera;

        private Texture2D _backgroundTexture;
        private BitmapFont _bitmapFont;

        private bool _drawScene = true;
        private bool _drawDebug = true;

        private KeyboardState _previousKBState;
        private KeyboardState _currentKBState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D textureBlock = Content.Load<Texture2D>("block");
            _backgroundTexture = Content.Load<Texture2D>("vignette");
            _bitmapFont = Content.Load<BitmapFont>("montserrat-32");

            BoxingViewportAdapter boxingViewport = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(boxingViewport);

            _world = new World(new Vector2(0, 0));

            float simWidth = ConvertSimUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width);
            float simHeight = ConvertSimUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height);

            Vertices borders = new Vertices(3);
            borders.Add(new Vector2(0, 0));
            borders.Add(new Vector2(0, simHeight));
            borders.Add(new Vector2(simWidth, simHeight));
            borders.Add(new Vector2(simWidth, 0));

            BodyFactory.CreateLoopShape(_world, borders);

            _debugView = new DebugViewXNA(_world);
            _debugView.LoadContent(GraphicsDevice, Content);

            _blockControlled = new Block(
                _world, 
                new Vector2(
                    ConvertSimUnits.ToSimUnits(410), 
                    ConvertSimUnits.ToSimUnits(110)
                    ),
                textureBlock);

            _block = new Block(
                _world, 
                new Vector2(
                    ConvertSimUnits.ToSimUnits(610), 
                    ConvertSimUnits.ToSimUnits(110)
                    ), 
                textureBlock);

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _blockControlled.Update(gameTime);

            // the camera properties of the camera can be conrolled to move, zoom and rotate
            const float movementSpeed = 2;
            const float rotationSpeed = 0.01f;
            const float zoomSpeed = 0.01f;

            _previousKBState = _currentKBState;
            _currentKBState = Keyboard.GetState();

            if (_currentKBState.IsKeyUp(Keys.D1) && _previousKBState.IsKeyDown(Keys.D1))
            {
                _drawScene = !_drawScene;
            }

            if (_currentKBState.IsKeyUp(Keys.D2) && _previousKBState.IsKeyDown(Keys.D2))
            {
                _drawDebug = !_drawDebug;
            }

            if (_currentKBState.IsKeyDown(Keys.W))
            {
                _camera.Move(new Vector2(0, -movementSpeed));
            }

            if (_currentKBState.IsKeyDown(Keys.A))
            {
                _camera.Move(new Vector2(-movementSpeed, 0));
            }

            if (_currentKBState.IsKeyDown(Keys.S))
            {
                _camera.Move(new Vector2(0, movementSpeed));
            }

            if (_currentKBState.IsKeyDown(Keys.D))
            {
                _camera.Move(new Vector2(movementSpeed, 0));
            }

            if (_currentKBState.IsKeyDown(Keys.E))
            {
                _camera.Rotation += rotationSpeed;
            }

            if (_currentKBState.IsKeyDown(Keys.Q))
            {
                _camera.Rotation -= rotationSpeed;
            }

            if (_currentKBState.IsKeyDown(Keys.R))
            {
                _camera.ZoomIn(zoomSpeed);
            }

            if (_currentKBState.IsKeyDown(Keys.F))
            {
                _camera.ZoomOut(zoomSpeed);
            }

            _world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 60f)));
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            var destinationRectangle = new Rectangle(0, 0, 800, 480);


            Matrix projection = Matrix.CreateOrthographicOffCenter(
                0f, 
                ConvertSimUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width),
                ConvertSimUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height), 
                0f, 
                0f, 
            1f);

            if (_drawScene)
            {
                _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
                _spriteBatch.Draw(_backgroundTexture, destinationRectangle, Color.White);
                _blockControlled.Draw(_spriteBatch);
                _block.Draw(_spriteBatch);
                _spriteBatch.End();
            }

            if (_drawDebug)
            {
                _debugView.RenderDebugData(projection, _camera.GetViewSimMatrix());
            }

            // not all sprite batches need to be affected by the camera
            var rectangle = _camera.GetBoundingRectangle();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("WASD: Move camera");
            stringBuilder.AppendLine("Arrows: Move box");
            stringBuilder.AppendLine("EQ: Rotate camera");
            stringBuilder.AppendLine("RF: Zoom camera");
            stringBuilder.AppendLine("1: Draw scene");
            stringBuilder.AppendLine("2: Draw debug");


            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
            _spriteBatch.DrawString(_bitmapFont, stringBuilder.ToString(), new Vector2(5, 5), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
