﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.InputListeners
{
    public class KeyboardListener : InputListener
    {
        private bool _isInitial;
        private TimeSpan _lastPressTime;

        private Keys _previousKey;
        private KeyboardState _previousState;

        public KeyboardListener()
            : this(new KeyboardListenerSettings())
        {
        }

        public KeyboardListener(KeyboardListenerSettings settings)
        {
            InitialDelay = settings.InitialDelayMilliseconds;
            RepeatDelay = settings.RepeatDelayMilliseconds;
        }

        public int InitialDelay { get; }
        public int RepeatDelay { get; }

        public event EventHandler<KeyboardEventArgs> KeyTyped;
        public event EventHandler<KeyboardEventArgs> KeyPressed;
        public event EventHandler<KeyboardEventArgs> KeyReleased;

        public override void Update(GameTime gameTime)
        {
            var currentState = Keyboard.GetState();

            RaisePressedEvents(gameTime, currentState);
            RaiseReleasedEvents(currentState);
            RaiseRepeatEvents(gameTime, currentState);

            _previousState = currentState;
        }

        private void RaisePressedEvents(GameTime gameTime, KeyboardState currentState)
        {
            if (!currentState.IsKeyDown(Keys.LeftAlt) && !currentState.IsKeyDown(Keys.RightAlt))
            {
                var pressedKeys = Enum.GetValues(typeof(Keys))
                    .Cast<Keys>()
                    .Where(key => currentState.IsKeyDown(key) && _previousState.IsKeyUp(key));

                foreach (var key in pressedKeys)
                {
                    var args = new KeyboardEventArgs(key, currentState);

                    KeyPressed?.Invoke(this, args);

                    if (args.Character.HasValue)
                        KeyTyped?.Invoke(this, args);

                    _previousKey = key;
                    _lastPressTime = gameTime.TotalGameTime;
                    _isInitial = true;
                }
            }
        }

        private void RaiseReleasedEvents(KeyboardState currentState)
        {
            var releasedKeys = Enum.GetValues(typeof(Keys))
                .Cast<Keys>()
                .Where(key => currentState.IsKeyUp(key) && _previousState.IsKeyDown(key));

            foreach (var key in releasedKeys)
                KeyReleased?.Invoke(this, new KeyboardEventArgs(key, currentState));
        }

        private void RaiseRepeatEvents(GameTime gameTime, KeyboardState currentState)
        {
            var elapsedTime = (gameTime.TotalGameTime - _lastPressTime).TotalMilliseconds;

            if (currentState.IsKeyDown(_previousKey) &&
                ((_isInitial && (elapsedTime > InitialDelay)) || (!_isInitial && (elapsedTime > RepeatDelay))))
            {
                var args = new KeyboardEventArgs(_previousKey, currentState);

                if (args.Character.HasValue)
                    KeyTyped?.Invoke(this, args);

                _lastPressTime = gameTime.TotalGameTime;
                _isInitial = false;
            }
        }
    }
}