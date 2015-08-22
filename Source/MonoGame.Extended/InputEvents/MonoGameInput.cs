using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.InputEvents
{
    public sealed class MonoGameInput : Input
    {
        private readonly MonoGameMouseEvents _mouseEvents;
        private readonly MonoGameKeyboardEvents _monoGameKeyboardEvents;
		private readonly MonoGameTouchEvents _monoGameTouchEvents;

        public MonoGameInput(Game game, int? doubleClickMaxTimeDelta = null,
            int? doubleClickMaxMovementDelta = null) : base(game)
        {
            if (doubleClickMaxTimeDelta == null) 
            {
                doubleClickMaxTimeDelta = 500; //Windows default
            }

            if (doubleClickMaxMovementDelta == null) 
            {
                doubleClickMaxMovementDelta = 2; //Windows default
            }

            _mouseEvents = new MonoGameMouseEvents(
                doubleClickMaxTimeDelta.Value,
                doubleClickMaxTimeDelta.Value);

            _monoGameKeyboardEvents = new MonoGameKeyboardEvents();

			_monoGameTouchEvents = new MonoGameTouchEvents();
        }

        override public void Update(GameTime gameTime) 
        {
            _mouseEvents.Update(gameTime);
            _monoGameKeyboardEvents.Update(gameTime);
			_monoGameTouchEvents.Update(gameTime);
        }


        /*####################################################################*/
        /*                          Keyboard Events                           */
        /*####################################################################*/

        public override event EventHandler<KeyboardCharacterEventArgs> CharacterTyped
        {
            add { _monoGameKeyboardEvents.CharacterTyped += value; }
            remove { _monoGameKeyboardEvents.CharacterTyped -= value; }
        }

        public override event EventHandler<KeyboardKeyEventArgs> KeyDown
        {
            add { _monoGameKeyboardEvents.KeyPressed += value; }
            remove { _monoGameKeyboardEvents.KeyPressed -= value; }
        }

        public override event EventHandler<KeyboardKeyEventArgs> KeyUp
        {
            add { _monoGameKeyboardEvents.KeyReleased += value; }
            remove { _monoGameKeyboardEvents.KeyReleased -= value; }
        }


        /*####################################################################*/
        /*                            Mouse Events                            */
        /*####################################################################*/ 

        //Movement
        public override event EventHandler<MouseEventArgs> MouseMoved
        {
            add { _mouseEvents.MouseMoved += value; }
            remove { _mouseEvents.MouseMoved -= value; }
        }

        //Buttons
        public override event EventHandler<MouseEventArgs> MouseDoubleClick
        {
            add { _mouseEvents.ButtonDoubleClicked += value; }
            remove { _mouseEvents.ButtonDoubleClicked -= value; }
        }

        public override event EventHandler<MouseEventArgs> MouseDown
        {
            add { _mouseEvents.ButtonPressed += value; }
            remove { _mouseEvents.ButtonPressed -= value; }
        }

        public override event EventHandler<MouseEventArgs> MouseUp
        {
            add { _mouseEvents.ButtonReleased += value; }
            remove { _mouseEvents.ButtonReleased -= value; }
        }                

        //Wheel
        public override event EventHandler<MouseEventArgs> MouseWheel
        {
            add { _mouseEvents.MouseWheelMoved += value; }
            remove { _mouseEvents.MouseWheelMoved -= value; }
        }

		/*####################################################################*/
		/*                            Touch Events                            */
		/*####################################################################*/

		public override event EventHandler<TouchEventArgs> TouchBegan
		{
			add { _monoGameTouchEvents.TouchBegan += value; }
			remove { _monoGameTouchEvents.TouchBegan -= value; }
		}

		public override event EventHandler<TouchEventArgs> TouchMoved
		{
			add { _monoGameTouchEvents.TouchMoved += value; }
			remove { _monoGameTouchEvents.TouchMoved -= value; }
		}

		public override event EventHandler<TouchEventArgs> TouchEnded
		{
			add { _monoGameTouchEvents.TouchEnded += value; }
			remove { _monoGameTouchEvents.TouchEnded -= value; }
		}

		public override event EventHandler<TouchEventArgs> TouchCancelled
		{
			add { _monoGameTouchEvents.TouchCancelled += value; }
			remove { _monoGameTouchEvents.TouchCancelled -= value; }
		}
    }
}
