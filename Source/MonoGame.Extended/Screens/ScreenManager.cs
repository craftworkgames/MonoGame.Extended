using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Screens
{
    public class Transition : IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly Color _color;
        private float _value;
        private TransitionState _state = TransitionState.Out;

        private enum TransitionState { In, Out }
        
        public Transition(GraphicsDevice graphicsDevice, Color color)
        {
            _graphicsDevice = graphicsDevice;
            _color = color;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public event EventHandler StateChanged;

        public void Dispose()
        {
        }

        public bool Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();

            switch (_state)
            {
                case TransitionState.Out:
                    _value += elapsedSeconds;

                    if (_value >= 1.0f)
                    {
                        _state = TransitionState.In;
                        StateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;

                case TransitionState.In:
                    _value -= elapsedSeconds;

                    if (_value <= 0.0f)
                        return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.FillRectangle(_graphicsDevice.Viewport.Bounds, _color * MathHelper.Clamp(_value, 0, 1));
            _spriteBatch.End();
        }
    }

    public class ScreenManager : SimpleDrawableGameComponent
    {
        public ScreenManager()
        {
        }

        private Screen _activeScreen;
        private bool _isInitialized;
        private bool _isLoaded;
        private Transition _activeTransition;

        public void LoadScreen(Screen screen, Transition transition)
        {
            _activeTransition = transition;
            _activeTransition.StateChanged += (sender, args) => LoadScreen(screen);
        }

        public void LoadScreen(Screen screen)
        {
            _activeScreen?.UnloadContent();
            _activeScreen?.Dispose();

            screen.ScreenManager = this;

            if (_isInitialized)
                screen.Initialize();

            if (_isLoaded)
                screen.LoadContent();

            _activeScreen = screen;
        }

        public override void Initialize()
        {
            base.Initialize();
            _activeScreen?.Initialize();
            _isInitialized = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _activeScreen?.LoadContent();
            _isLoaded = true;
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            _activeScreen?.UnloadContent();
            _isLoaded = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (_activeTransition != null)
            {
                if (!_activeTransition.Update(gameTime))
                {
                    _activeTransition.Dispose();
                    _activeTransition = null;
                }
            }
            else
            {
                _activeScreen?.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _activeScreen?.Draw(gameTime);
            _activeTransition?.Draw(gameTime);
        }
    }
}