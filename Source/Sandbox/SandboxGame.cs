using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Sandbox
{
    /// <summary>
    /// You can use this sandbox game to test features that require manual interaction.
    /// Code in this class is typically throw away testing. It's not part of the library.
    /// </summary>
    public class SandboxGame : Game
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        private Texture2D _backgroundTexture;
        private Sprite _sprite;
        private ViewportAdapter _viewportAdapter;
        private BitmapFont _bitmapFont;
        private TiledMap _tiledMap;

        private InputListenerManager _inputManager;

        private Vector2 _cameraDirection = Vector2.Zero;
        private float _cameraRotation = 0f;

        public SandboxGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.IsBorderless = false;
            Window.Position = new Point(50, 50);
            Window.Title = string.Format("MonoGame.Extended - {0}", GetType().Name);

            _graphicsDeviceManager.PreferredBackBufferWidth = 1024;
            _graphicsDeviceManager.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            _viewportAdapter = new BoxingViewportAdapter(GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter)
            {
                //Zoom = 0.5f,
                Origin = new Vector2(400, 240),
                //Position = new Vector2(408, 270)
            };

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (s, e) => _viewportAdapter.OnClientSizeChanged();
            
            SetUpInput();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("hills");
            _bitmapFont = Content.Load<BitmapFont>("courier-new-32");

            var textureAtlas = Content.Load<TextureAtlas>("test-tileset-atlas");
            var textureRegion = textureAtlas.Regions.First();
            _textureRegion = textureAtlas.Regions.Last();
            _sprite = new Sprite(textureRegion)
            {
                Position = new Vector2(600, 240),
                Scale = Vector2.One * 2.5f,
                OriginNormalized = new Vector2(0.25f, 0.75f)
            };
            _tiledMap = Content.Load<TiledMap>("level01");
        }

        protected override void UnloadContent()
        {
        }

        private TextureRegion2D _textureRegion;

        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update(gameTime);

            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            //var mouseState = Mouse.GetState();
            //var gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();                                    
            
            _camera.Move(_cameraDirection * deltaTime);
            _camera.Rotation += _cameraRotation * deltaTime;

            _sprite.Rotation += deltaTime;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            //_spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            //_spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _tiledMap.WidthInPixels, _tiledMap.HeightInPixels), Color.White);
            //_spriteBatch.End();

            _tiledMap.Draw(_camera);

            _spriteBatch.Begin();
            //_spriteBatch.DrawString(_bitmapFont, "This is MonoGame.Extended", new Vector2(50, 50), new Color(Color.Black, 0.5f));
            //_spriteBatch.DrawString(_bitmapFont, string.Format("Camera: {0}", _camera.Position), new Vector2(50, 100), Color.Black);
            //_spriteBatch.DrawString(_bitmapFont, string.Format("Camera Direction: {0}", _cameraDirection), new Vector2(50, 150), Color.Black);
            //_spriteBatch.DrawString(_bitmapFont, 
            //    "Contrary to popular belief, Lorem Ipsum is not simply random text.\n\n" + 
            //    "It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard " + 
            //    "McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin " + 
            //    "words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, " + 
            //    "discovered the undoubtable source.", new Vector2(50, 100), new Color(Color.Black, 0.5f), _viewportAdapter.VirtualWidth - 50);
            _spriteBatch.End();

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_textureRegion, _sprite.GetBoundingRectangle(), Color.White);
            _spriteBatch.Draw(_sprite);
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void SetUpInput()
        {
            _inputManager = new InputListenerManager();

            var up = new Vector2(0, -250);
            var right = new Vector2(250, 0);

            //camera movement
            var keyboardListener = _inputManager.AddListener(new KeyboardListenerSettings());
            var mouseListener = _inputManager.AddListener(new MouseListenerSettings());
            _inputManager.Listeners.Add(keyboardListener);
            _inputManager.Listeners.Add(mouseListener);

            keyboardListener.KeyPressed += (sender, args) =>
            {
                switch (args.Key)
                {
                    case Keys.Escape:
                        Exit();
                        break;

                    case Keys.Q:                        
                        _cameraRotation += 1;
                        break;
                    case Keys.W:
                        _cameraRotation -= 1;
                        break;

                    case Keys.Up:
                        _cameraDirection += up;
                        break;
                    case Keys.Down:
                        _cameraDirection += -up;
                        break;
                    case Keys.Left:
                        _cameraDirection += -right;
                        break;
                    case Keys.Right:
                        _cameraDirection += right;
                        break;
                }
            };

            keyboardListener.KeyReleased += (sender, args) =>
            {
                switch (args.Key)
                {
                    case Keys.Q:
                        _cameraRotation -= 1;
                        break;
                    case Keys.W:
                        _cameraRotation += 1;
                        break;

                    case Keys.Up:
                        _cameraDirection -= up;
                        break;
                    case Keys.Down:
                        _cameraDirection -= -up;
                        break;
                    case Keys.Left:
                        _cameraDirection -= -right;
                        break;
                    case Keys.Right:                       
                        _cameraDirection -= right;
                        break;
                }
            };

            // zoom
            mouseListener.MouseWheelMoved += (sender, args) =>
            {
                _camera.Zoom += args.ScrollWheelDelta.Value * 0.0001f;
            };

            // look at
            mouseListener.MouseUp += (sender, args) =>
            {
                if (args.Button == MouseButton.Left)
                {
                    var p = _viewportAdapter.PointToScreen(args.Position);
                    Trace.WriteLine(string.Format("{0} => {1}", args.Position, p));
                }
            };

            mouseListener.MouseDown += (sender, args) => Trace.WriteLine("MouseDown");
            mouseListener.MouseUp += (sender, args) => Trace.WriteLine("MouseUp");
            mouseListener.MouseClicked += (sender, args) => Trace.WriteLine("MouseClicked");
            mouseListener.MouseDoubleClicked += (sender, args) => Trace.WriteLine("MouseDoubleClicked");
            mouseListener.MouseWheelMoved += (sender, args) => Trace.WriteLine("MouseWheelMoved");
            mouseListener.MouseDragged += (sender, args) => Trace.WriteLine("MouseDragged");
        }
    }
}
