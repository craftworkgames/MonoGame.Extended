using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.InputListeners
{
    public class InputListenerComponent : GameComponent, IUpdate, IInputService
    {
        public KeyboardListener GuiKeyboardListener { get { return (KeyboardListener)_guiListeners[0]; } }
        public MouseListener GuiMouseListener { get { return (MouseListener)_guiListeners[1]; } }
        public GamePadListener GuiGamePadListener { get { return (GamePadListener)_guiListeners[2]; } }
        public TouchListener GuiTouchListener { get { return (TouchListener)_guiListeners[3]; } }

        private readonly List<InputListener> _listeners;
        private List<InputListener> _guiListeners;

        public IList<InputListener> Listeners => _listeners;

        public InputListenerComponent(Game game) 
            : base(game)
        {
            _listeners = new List<InputListener>();

            RegisterGuiListeners();
            Game.Services.AddService<IInputService>(this);
        }

        public InputListenerComponent(Game game, params InputListener[] listeners)
            : base(game)
        {
            _listeners = new List<InputListener>(listeners);

            RegisterGuiListeners();
            Game.Services.AddService<IInputService>(this);
        }

        private void RegisterGuiListeners()
        {
            _guiListeners = new List<InputListener>(4);

            _guiListeners.Add(new KeyboardListener());
            _guiListeners.Add(new MouseListener());
            _guiListeners.Add(new GamePadListener(new GamePadListenerSettings()));
            _guiListeners.Add(new TouchListener());
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Game.IsActive)
            {
                foreach (var listener in _listeners)
                    listener.Update(gameTime);
                foreach (var listener in _guiListeners)
                    listener.Update(gameTime);
            }

            GamePadListener.CheckConnections();
        }
    }
}