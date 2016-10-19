using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners.Devices;
using MonoGame.Extended.NuclexGui.Input;

using XnaMouse = Microsoft.Xna.Framework.Input.Mouse;
using XnaEventHandler = System.EventHandler<System.EventArgs>;

namespace MonoGame.Extended.InputListeners
{
    public class InputManager : IInputCapturer
    {
        #region Events

        #endregion

        #region Properties

        public IInputReceiver InputReceiver
        {
            get; set;
        }

        #endregion

        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Methods

        #endregion
    }
}
