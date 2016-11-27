using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.InputListeners
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly List<string> _logLines = new List<string>();
        private SpriteBatch _spriteBatch;
        private Texture2D _backgroundTexture;
        private BitmapFont _bitmapFont;
        private string _typedString = string.Empty;
        private bool _isCursorVisible = true;
        private const float _cursorBlinkDelay = 0.5f;
        private float _cursorBlinkDelta = _cursorBlinkDelay;
        private Camera2D _camera;
        
        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;
        }

        protected override void Initialize()
        {
            var mouseListener = new MouseListener(new MouseListenerSettings());
            var keyboardListener = new KeyboardListener(new KeyboardListenerSettings());

            Components.Add(new InputListenerComponent(this, mouseListener, keyboardListener));

            keyboardListener.KeyPressed += (sender, args) =>
            {
                if (args.Key == Keys.Escape)
                    Exit();
            };

            mouseListener.MouseClicked += (sender, args) => LogMessage("{0} mouse button clicked", args.Button);
            mouseListener.MouseDoubleClicked += (sender, args) => LogMessage("{0} mouse button double-clicked", args.Button);
            mouseListener.MouseDown += (sender, args) => LogMessage("{0} mouse button down", args.Button);
            mouseListener.MouseUp += (sender, args) => LogMessage("{0} mouse button up", args.Button);
            mouseListener.MouseDrag += (sender, args) => LogMessage("Mouse dragged");
            mouseListener.MouseWheelMoved += (sender, args) => LogMessage("Mouse scroll wheel value {0}", args.ScrollWheelValue);

            keyboardListener.KeyPressed += (sender, args) => LogMessage("{0} key pressed", args.Key);
            keyboardListener.KeyReleased += (sender, args) => LogMessage("{0} key released", args.Key);
            keyboardListener.KeyTyped += (sender, args) =>
            {
                if (args.Key == Keys.Back && _typedString.Length > 0)
                {
                    _typedString = _typedString.Substring(0, _typedString.Length - 1);
                }
                else if (args.Key == Keys.Enter)
                {
                    LogMessage(_typedString);
                    _typedString = string.Empty;
                }
                else
                {
                    _typedString += args.Character?.ToString() ?? "";
                }
            };

            LogMessage("Do something with the mouse or keyboard...");

            base.Initialize();
        }

        private const int _maxLogLines = 13;

        private void LogMessage(string messageFormat, params object[] args)
        {
            var message = string.Format(messageFormat, args);

            if (_logLines.Count == _maxLogLines)
                _logLines.RemoveAt(0);

            _logLines.Add(message);
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundTexture = Content.Load<Texture2D>("vignette");
            _bitmapFont = Content.Load<BitmapFont>("montserrat-32");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            _cursorBlinkDelta -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_cursorBlinkDelta <= 0)
            {
                _isCursorVisible = !_isCursorVisible;
                _cursorBlinkDelta = _cursorBlinkDelay;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.DarkSlateGray);

            for (var i = 0; i < _logLines.Count; i++)
            {
                var logLine = _logLines[i];
                _spriteBatch.DrawString(_bitmapFont, logLine, new Vector2(4, i * _bitmapFont.LineHeight), Color.LightGray * 0.2f);
            }

            var textInputY = 14 * _bitmapFont.LineHeight - 2;
            var position = new Point(4, textInputY);
            var stringRectangle = _bitmapFont.GetStringRectangle(_typedString, position);

            _spriteBatch.DrawString(_bitmapFont, _typedString, position.ToVector2(), Color.White);

            if (_isCursorVisible)
                _spriteBatch.DrawString(_bitmapFont, "_", new Vector2(stringRectangle.Width, textInputY), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
