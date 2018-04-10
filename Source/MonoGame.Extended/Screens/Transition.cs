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
        private readonly float _halfDuration;
        private float _currentValue;
        private TransitionState _state = TransitionState.Out;

        private enum TransitionState { In, Out }
        
        public Transition(GraphicsDevice graphicsDevice, Color color, float duration = 1.0f)
        {
            _graphicsDevice = graphicsDevice;
            _color = color;
            _halfDuration = duration / 2f;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public event EventHandler StateChanged;
        public event EventHandler Completed;

        public void Dispose()
        {
        }

        public void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();

            switch (_state)
            {
                case TransitionState.Out:
                    _currentValue += elapsedSeconds;

                    if (_currentValue >= _halfDuration)
                    {
                        _state = TransitionState.In;
                        StateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case TransitionState.In:
                    _currentValue -= elapsedSeconds;

                    if (_currentValue <= 0.0f)
                    {
                        Completed?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.FillRectangle(_graphicsDevice.Viewport.Bounds, _color * MathHelper.Clamp(_currentValue / _halfDuration, 0, 1));
            _spriteBatch.End();
        }
    }
}