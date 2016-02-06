using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.SceneGraphs
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly bool _isFullScreen;
        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        private SceneGraph _sceneGraph;
        private SceneNode _leftWheelNode;
        private SceneNode _rightWheelNode;
        private SceneNode _carNode;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _isFullScreen = false;

            Content.RootDirectory = "Content";
            Window.Title = "MonoGame.Extended Game";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (_isFullScreen)
            {
                _graphicsDeviceManager.IsFullScreen = true;
                _graphicsDeviceManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                _graphicsDeviceManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                _graphicsDeviceManager.ApplyChanges();
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter) {Zoom = 2.0f};
            _sceneGraph = new SceneGraph();

            var carHullTexture = Content.Load<Texture2D>("car-hull");
            var carHullSprite = new Sprite(carHullTexture);
            var carWheelTexture = Content.Load<Texture2D>("car-wheel");
            var carWheelSprite = new Sprite(carWheelTexture);

            _carNode = _sceneGraph.RootNode.CreateChildSceneNode(viewportAdapter.Center.ToVector2());
            _carNode.Attach(carHullSprite);

            _leftWheelNode = _carNode.CreateChildSceneNode(new Vector2(-29, 17));
            _leftWheelNode.Attach(carWheelSprite);

            _rightWheelNode = _carNode.CreateChildSceneNode(new Vector2(40, 17));
            _rightWheelNode.Attach(carWheelSprite);

            
        }

        protected override void UnloadContent()
        {
        }

        private float _speed = 0.15f;

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.W))
                _speed += deltaTime * 0.5f;

            if (keyboardState.IsKeyDown(Keys.S))
                _speed -= deltaTime * 0.5f;

            _leftWheelNode.Rotation += _speed;
            _rightWheelNode.Rotation = _leftWheelNode.Rotation;
            _carNode.Position += new Vector2(_speed * 5, 0);

            // quick and dirty collision detection
            const int maxX = 535;
            if (_carNode.Position.X >= maxX)
            {
                _speed = -_speed * 0.2f;
                _carNode.Position =  new Vector2(maxX, _carNode.Position.Y);
            }

            const int minX = 265;
            if (_carNode.Position.X <= minX)
            {
                _speed = -_speed * 0.2f;
                _carNode.Position = new Vector2(minX, _carNode.Position.Y);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

            _sceneGraph.Draw(_spriteBatch);

            _spriteBatch.FillRectangle(0, 266, 800, 240, Color.DarkOliveGreen);
            _spriteBatch.FillRectangle(200, 0, 5, 480, Color.DarkOliveGreen);
            _spriteBatch.FillRectangle(595, 0, 5, 480, Color.DarkOliveGreen);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
