using System;

namespace Microsoft.Xna.Framework.Input
{
    public class KeyboardCharacterEventArgs : EventArgs
    {
        public char Character { get; private set; }

        internal KeyboardCharacterEventArgs(char character) 
        {
            Character = character;
        }
    }
}
