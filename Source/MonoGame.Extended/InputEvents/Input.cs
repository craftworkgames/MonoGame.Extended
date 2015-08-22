using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.InputEvents
{
    public abstract class Input 
    {
        protected Game Game { get; set; }

        protected Input(Game game) 
        {
            Game = game;
        }

        /*####################################################################*/
        /*                           Input Events                             */
        /*####################################################################*/

        public abstract void Update(GameTime gameTime);

        /*####################################################################*/
        /*                          Keyboard Events                           */
        /*####################################################################*/

        public abstract event EventHandler<KeyboardCharacterEventArgs> CharacterTyped;

        public abstract event EventHandler<KeyboardKeyEventArgs> KeyDown;
        public abstract event EventHandler<KeyboardKeyEventArgs> KeyUp;

        /*####################################################################*/
        /*                            Mouse Events                            */
        /*####################################################################*/

        public abstract event EventHandler<MouseEventArgs> MouseMoved;
        public abstract event EventHandler<MouseEventArgs> MouseDoubleClick;

        public abstract event EventHandler<MouseEventArgs> MouseDown;
        public abstract event EventHandler<MouseEventArgs> MouseUp;

        public abstract event EventHandler<MouseEventArgs> MouseWheel;

	    /*####################################################################*/
	    /*                            Touch Events                            */
	    /*####################################################################*/

	    public abstract event EventHandler<TouchEventArgs> TouchBegan;
	    public abstract event EventHandler<TouchEventArgs> TouchMoved;
	    public abstract event EventHandler<TouchEventArgs> TouchEnded;
	    public abstract event EventHandler<TouchEventArgs> TouchCancelled;
    }
}
