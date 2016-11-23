using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.BitmapFonts
{
    public class Game1 : Game
    {
        private Texture2D _backgroundTexture;
        private BitmapFont _bitmapFont;
        private Camera2D _camera;
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private Vector2 _labelPosition = Vector2.Zero;
        private string _labelText = "";
        private SpriteBatch _spriteBatch;
        private ViewportAdapter _viewportAdapter;
        private StringBuilder _stringBuilderText;
        private SpriteEffects _effect;
        private KeyboardState _lastKeyboardState;
        private KeyboardState _currentKeyboardState;
        private float _rotation;
        private float _layer;
        private Vector2 _scale;
        private bool _wrappedText;
        private readonly Vector2 _bodyTextPosition = new Vector2(50, 60);
        private Color _bodyTextColor = new Color(Color.Black, 0.5f);

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            //Window.Position = Point.Zero;
        }

        protected override void LoadContent()
        {
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter);
            _backgroundTexture = Content.Load<Texture2D>("vignette");
            _bitmapFont = Content.Load<BitmapFont>("montserrat-32");
            _stringBuilderText = new StringBuilder();
            _stringBuilderText.AppendLine("This text is built using lines manually wrapped and");
            _stringBuilderText.AppendLine("appended to a StringBuilder object.");
            _stringBuilderText.AppendLine("Lorem Ipsum comes from sections 1.10.32 and 1.10.33");
            _stringBuilderText.AppendLine("of \"de Finibus Bonorum et Malorum\" (The Extremes");
            _stringBuilderText.AppendLine("of Good and Evil) by Cicero, written in 45 BC. This");
            _stringBuilderText.AppendLine("book is a treatise on the theory of ethics, very");
            _stringBuilderText.AppendLine("popular during the Renaissance. The first line of");
            _stringBuilderText.AppendLine("Lorem Ipsum, \"Lorem ipsum dolor sit amet..\",");
            _stringBuilderText.AppendLine("comes from a line in section 1.10.32.");
            _effect = SpriteEffects.None;
            _layer = 0f;
            _scale = Vector2.One;
            _wrappedText = true;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            _lastKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            //if (keyboardState.IsKeyDown(Keys.Escape))
            //{
            //    Exit();
            //}

            // layers
            if (_currentKeyboardState.IsKeyDown(Keys.Down) && _lastKeyboardState.IsKeyUp(Keys.Down))
            {
                _layer -= 0.1f;
                if (_layer < 0f)
                {
                    _layer = 0f;
                }
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Up) && _lastKeyboardState.IsKeyUp(Keys.Up))
            {
                _layer += 0.1f;
                if (_layer > 1f)
                {
                    _layer = 1f;
                }
            }

            // scale
            if (_currentKeyboardState.IsKeyDown(Keys.Add) && _lastKeyboardState.IsKeyUp(Keys.Add))
            {
                _scale += new Vector2(0.1f, 0.1f);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Subtract) && _lastKeyboardState.IsKeyUp(Keys.Subtract))
            {
                _scale -= new Vector2(0.1f, 0.1f);
            }

            // rotate
            if (_currentKeyboardState.IsKeyDown(Keys.R) && _lastKeyboardState.IsKeyUp(Keys.R))
            {
                _rotation += 10f;
                if (_rotation >= 360f) _rotation = 0f;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.W) && _lastKeyboardState.IsKeyUp(Keys.W))
            {
                _wrappedText = !_wrappedText;
            }

            // flip
            if (_currentKeyboardState.IsKeyDown(Keys.F) && _lastKeyboardState.IsKeyUp(Keys.F))
            {
                switch (_effect)
                {
                    case SpriteEffects.None:
                        _effect = SpriteEffects.FlipHorizontally;
                        break;
                    case SpriteEffects.FlipHorizontally:
                        _effect = SpriteEffects.FlipVertically;
                        break;
                    case SpriteEffects.FlipVertically:
                        _effect = SpriteEffects.None;
                        break;
                }
            }

            _labelText = $"Layer={_layer:F} Angle={_rotation} Scale={_scale.X} Effect={_effect}";
            var stringRectangle = _bitmapFont.GetStringRectangle(_labelText, Vector2.Zero);
            _labelPosition = new Vector2(800/2 - stringRectangle.Width/2, 440);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            DrawBackground();
            DrawTitle();
            DrawBodyText();
            DrawLabel();
            DrawInstructions();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawBackground()
        {
            _spriteBatch.Draw(_backgroundTexture, _viewportAdapter.BoundingRectangle, Color.White);
        }

        private void DrawTitle()
        {
            _bitmapFont.LetterSpacing = 3;
            _spriteBatch.DrawString(_bitmapFont, "MonoGame.Extended BitmapFont Sample", new Vector2(50, 10), Color.White,
                MathHelper.ToRadians(_rotation), Vector2.Zero, _scale, _effect, _layer);
        }

        private void DrawBodyText()
        {
            _bitmapFont.LetterSpacing = 0;
            if (_wrappedText)
            {
                // pass it one long line of text and wrap it at the specified width
                _spriteBatch.DrawString(_bitmapFont,
                    "This text is continuous and wrapped at 750 pixels. " +
                    "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of " +
                    "classical Latin literature from 45 BC, making it over 2000 years old. Richard " +
                    "McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more " +
                    "obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of " +
                    "the word in classical literature, discovered the undoubtable source.",
                    _bodyTextPosition, _bodyTextColor, 750);
            }
            else
            {
                // pass it a StringBuilder object to draw
                _spriteBatch.DrawString(_bitmapFont, _stringBuilderText, _bodyTextPosition, _bodyTextColor);
            }
        }

        private void DrawInstructions()
        {
            const string instructions = "W = Wrap, R = Rotate, +/- = Scale, F = Flip, Up/Down = Layer";
            var textSize = _bitmapFont.MeasureString(instructions);
            var screenWidth = _viewportAdapter.VirtualWidth;
            var x = screenWidth/2 - textSize.Width/2;
            _spriteBatch.DrawString(_bitmapFont, instructions, new Vector2(x, 410), Color.Black);
        }

        private void DrawLabel()
        {
            _spriteBatch.DrawString(_bitmapFont, _labelText, _labelPosition, Color.Black);
        }
    }
}