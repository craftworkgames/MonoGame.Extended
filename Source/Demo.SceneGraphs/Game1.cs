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
        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        private SceneGraph _sceneGraph;
        
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

            _camera = new Camera2D(viewportAdapter);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var texture = Content.Load<Texture2D>("logo-square-512");
            _sceneGraph = new SceneGraph(GraphicsDevice, viewportAdapter, _camera)
            {
                RootNode = new SceneNode
                {
                    Position = viewportAdapter.Center.ToVector2(),
                    Entity = new Sprite(texture) { Scale = Vector2.One * 0.5f }
                }
            };
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _sceneGraph.RootNode.Rotation += deltaTime;
            _sceneGraph.Update(gameTime);

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
