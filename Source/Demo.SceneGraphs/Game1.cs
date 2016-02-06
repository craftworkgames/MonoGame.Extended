using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.SceneGraphs
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly bool _isFullScreen;
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
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter) {Zoom = 4.0f};
            _sceneGraph = new SceneGraph(GraphicsDevice, viewportAdapter, _camera);

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

        private float _speed = 0;

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.W))
                _speed += deltaTime * 0.01f;

            if (keyboardState.IsKeyDown(Keys.S))
                _speed -= deltaTime * 0.01f;


            //_sceneGraph.Update(gameTime);
            _leftWheelNode.Rotation += _speed;
            _rightWheelNode.Rotation = _leftWheelNode.Rotation;
            _carNode.Position += new Vector2(_speed * 5, 0);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _sceneGraph.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
