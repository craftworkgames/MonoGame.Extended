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
using OpenTK.Input;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

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
        private Sprite _sprite;
        private ViewportAdapter _viewportAdapter;
        private BitmapFont _bitmapFont;
        private TiledMap _tiledMap;
        private FramesPerSecondCounter _fpsCounter;
        private InputListenerManager _inputManager;
        private SpriteAnimator _spriteAnimator;
        private Vector2 _cameraDirection = Vector2.Zero;
        private float _cameraRotation;
        private Sprite _zombieSprite;
        private SpriteAnimator _zombieAnimator;

        public SandboxGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Position = new Point(50, 50);
            Window.Title = string.Format("MonoGame.Extended - {0}", GetType().Name);
        }

        protected override void Initialize()
        {
            _fpsCounter = new FramesPerSecondCounter();
            _viewportAdapter = new BoxingViewportAdapter(GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter)
            {
                MinimumZoom = 0.5f,
                MaximumZoom = 2.0f,
                Zoom = 0.5f,
                Origin = new Vector2(400, 240),
                Position = new Vector2(408, 270)
            };

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (s, e) => _viewportAdapter.OnClientSizeChanged();
            
            SetUpInput();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bitmapFont = Content.Load<BitmapFont>("courier-new-32");
            _tiledMap = Content.Load<TiledMap>("level01");

            var fireballTexture = Content.Load<Texture2D>("fireball");
            var spriteSheetAtlas = TextureAtlas.Create(fireballTexture, 512, 197);

            _sprite = new Sprite(spriteSheetAtlas.First())
            {
                Position = new Vector2(850, 200),
                Scale = new Vector2(0.5f)
            };
            _spriteAnimator = new SpriteAnimator(_sprite, spriteSheetAtlas, 15);

            var zombieSheet = Content.Load<TextureAtlas>("zombie-atlas");
            _zombieSprite = new Sprite(zombieSheet[0])
            {
                Position = new Vector2(300, 900),
                OriginNormalized = new Vector2(0.5f, 1.0f),
                Scale = new Vector2(0.5f)
            };
            _zombieAnimator = new SpriteAnimator(_zombieSprite, zombieSheet, 7);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.R))
                _camera.ZoomIn(deltaSeconds);

            if (Keyboard.GetState().IsKeyDown(Keys.F))
                _camera.ZoomOut(deltaSeconds);

            _inputManager.Update(gameTime);

            _camera.Move(_cameraDirection * deltaSeconds);
            _camera.Rotation += _cameraRotation * deltaSeconds;
            _sprite.Position += new Vector2(-500, 0) * deltaSeconds;

            _spriteAnimator.Update(gameTime);
            _zombieAnimator.Update(gameTime);

            if (_sprite.Position.X < 0 - _sprite.GetBoundingRectangle().Width)
                _sprite.Position = new Vector2(1900, _sprite.Position.Y);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _fpsCounter.Update(gameTime);

            GraphicsDevice.Clear(Color.Black);

            _tiledMap.Draw(_camera);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_sprite);
            _spriteBatch.Draw(_zombieSprite);
            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_bitmapFont, string.Format("FPS: {0} Zoom: {1}", 
                _fpsCounter.AverageFramesPerSecond,
                _camera.Zoom), 
                new Vector2(5, 5), new Color(0.5f, 0.5f, 0.5f));
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void SetUpInput()
        {
            _inputManager = new InputListenerManager();

            var up = new Vector2(0, -250);
            var right = new Vector2(250, 0);

            //camera movement
            var keyboardListener = _inputManager.AddListener<KeyboardListener>();
            var mouseListener = _inputManager.AddListener<MouseListener>();

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
                    case Keys.Escape:
                        Exit();
                        break;
                        
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
                _camera.Zoom += args.ScrollWheelDelta * 0.001f;
            };

            //// look at
            //mouseListener.MouseUp += (sender, args) =>
            //{
            //    if (args.Button == MouseButton.Left)
            //    {
            //        var p = _viewportAdapter.PointToScreen(args.Position);
            //        Trace.WriteLine(string.Format("{0} => {1}", args.Position, p));
            //    }
            //};

            mouseListener.MouseDown += (sender, args) => Trace.WriteLine("MouseDown");
            mouseListener.MouseUp += (sender, args) => Trace.WriteLine("MouseUp");
            mouseListener.MouseClicked += (sender, args) => Trace.WriteLine("MouseClicked");
            mouseListener.MouseDoubleClicked += (sender, args) => Trace.WriteLine("MouseDoubleClicked");
            mouseListener.MouseWheelMoved += (sender, args) => Trace.WriteLine(string.Format("MouseWheelMoved {0}", args.ScrollWheelValue));
            mouseListener.MouseDragged += (sender, args) => Trace.WriteLine("MouseDragged");
        }
    }
}
