using FarseerPhysics.Common;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;

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
                    ConvertSimUnits.ToSimUnits(110), 
                    ConvertSimUnits.ToSimUnits(110)
                    ),
                textureBlock);

            _block = new Block(
                _world, 
                new Vector2(
                    ConvertSimUnits.ToSimUnits(410), 
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

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                _camera.Move(new Vector2(0, -movementSpeed));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _camera.Move(new Vector2(-movementSpeed, 0));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                _camera.Move(new Vector2(0, movementSpeed));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _camera.Move(new Vector2(movementSpeed, 0));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                _camera.Rotation += rotationSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                _camera.Rotation -= rotationSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                _camera.ZoomIn(zoomSpeed);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                _camera.ZoomOut(zoomSpeed);
            }

            _world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 60f)));
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Matrix projection = Matrix.CreateOrthographicOffCenter(
                0f, 
                ConvertSimUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width),
                ConvertSimUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height), 
                0f, 
                0f, 
            1f);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _blockControlled.Draw(_spriteBatch);
            _block.Draw(_spriteBatch);
            _spriteBatch.End();

            _debugView.RenderDebugData(projection, _camera.GetViewSimMatrix());

            base.Draw(gameTime);
        }
    }
}
