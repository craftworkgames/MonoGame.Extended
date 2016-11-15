﻿using Microsoft.Xna.Framework;

namespace MonoGame.Extended.InputListeners
{
    public abstract class InputListener
    {
        protected InputListener()
        {
        }

        //public ViewportAdapter ViewportAdapter { get; set; }

        public abstract void Update(GameTime gameTime);
    }
}