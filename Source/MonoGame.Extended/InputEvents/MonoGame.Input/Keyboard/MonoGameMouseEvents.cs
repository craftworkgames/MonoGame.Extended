using System;
using System.Linq;

namespace Microsoft.Xna.Framework.Input
{
    internal class MonoGameKeyboardEvents
    {
        internal event EventHandler<KeyboardCharacterEventArgs> CharacterTyped;
        internal event EventHandler<KeyboardKeyEventArgs> KeyPressed;
        internal event EventHandler<KeyboardKeyEventArgs> KeyReleased;


        private static int InitialDelay { get; set; }
        private static int RepeatDelay { get; set; }

        private Keys _lastKey;
        private TimeSpan _lastPress;
        private bool _isInitial;
        
        private KeyboardState _previous;

        public MonoGameKeyboardEvents()
        {
            InitialDelay = 800;
            RepeatDelay = 50;
        }

        public void Update(GameTime gameTime)
        {
            var current = Keyboard.GetState();
            
            if (!current.IsKeyDown(Keys.LeftAlt)
                && !current.IsKeyDown(Keys.RightAlt)) 
            {
                foreach (var key in Enum.GetValues(typeof (Keys))
                    .Cast<Keys>()
                    .Where(key => current.IsKeyDown(key) && _previous.IsKeyUp(key)))
                {
                    var args = new KeyboardKeyEventArgs(key);

                    OnKeyPressed(this, args);

                    _lastKey = key;
                    _lastPress = gameTime.TotalGameTime;
                    _isInitial = true;
                }
            }
				
            foreach (var key in 
                Enum.GetValues(typeof(Keys))
                .Cast<Keys>()
                .Where(key => current.IsKeyUp(key) && _previous.IsKeyDown(key))) 
            {
                OnKeyReleased(this, new KeyboardKeyEventArgs(key));
            }

            var elapsedTime = (gameTime.TotalGameTime - _lastPress).TotalMilliseconds;

            if (current.IsKeyDown(_lastKey) && 
                ((_isInitial && elapsedTime > InitialDelay) ||
                (!_isInitial && elapsedTime > RepeatDelay)))
            {
                OnCharacterPressed(this, new KeyboardKeyEventArgs(_lastKey));
                
                _lastPress = gameTime.TotalGameTime;
                _isInitial = false;
            }

            _previous = current;
        }

        private void OnKeyPressed(object sender, KeyboardKeyEventArgs args)
        {
            if (KeyPressed != null) 
            {
                KeyPressed(sender, args);
            }

            OnCharacterPressed(sender, args);
        }

        private void OnCharacterPressed(object sender, KeyboardKeyEventArgs args)
        {
            if (CharacterTyped == null) { return; }

            var character = KeyboardUtil.ToChar(args.Key, args.Modifiers);
            if (character.HasValue)
            {
                CharacterTyped(this, new KeyboardCharacterEventArgs(character.Value));
            }
        }

        private void OnKeyReleased(object sender, KeyboardKeyEventArgs args)
        {
            if (KeyReleased != null) 
            {
                KeyReleased(sender, args);
            }
        }
    }
}