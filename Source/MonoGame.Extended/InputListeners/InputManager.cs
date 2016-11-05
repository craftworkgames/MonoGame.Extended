using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.NuclexGui.Input;

using XnaEventHandler = System.EventHandler<System.EventArgs>;

namespace MonoGame.Extended.InputListeners
{
    public class InputManager : NuclexGui.Input.IInputService, IGameComponent, IUpdateable, IDisposable
    {
        #region Events

        /// <summary>Fired when the UpdateOrder property changes its  value</summary>
        public event XnaEventHandler UpdateOrderChanged;

        /// <summary>Fired when the Enabled property changes its value</summary>
        public event XnaEventHandler EnabledChanged { add { } remove { } }

        #endregion

        #region Properties

        public KeyboardListener KeyboardListener { get { return _keyboardListener; } }
        public MouseListener MouseListener { get { return _mouseListener; } }
        public GamePadListener GamePadListener { get { return _gamePadListener; } }
        public TouchListener TouchListener { get { return _touchListener; } }

        /// <summary>Whether the component is currently enabled</summary>
        public bool Enabled { get { return true; } }

        /// <summary>
        ///   Indicates when the game component should be updated relative to other game
        ///   components. Lower values are updated first.
        /// </summary>
        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (value != _updateOrder)
                {
                    _updateOrder = value;
                    OnUpdateOrderChanged();
                }
            }
        }

        #endregion

        #region Fields

        private KeyboardListener _keyboardListener;
        private MouseListener _mouseListener;
        private GamePadListener _gamePadListener;
        private TouchListener _touchListener;

        /// <summary>
        ///   Controls the order in which this game component is updated relative
        ///   to other game components.
        /// </summary>
        private int _updateOrder = int.MinValue;
        /// <summary>Game service container, saved to unregister on dispose</summary>
        private GameServiceContainer _gameServices;

        #endregion

        #region Constructors

        /// <summary>Initializes a new input manager</summary>
        public InputManager() : this(null) { }
        

        /// <summary>Initializs a new input manager</summary>
        /// <param name="services">Game service container the manager registers to</param>
        public InputManager(GameServiceContainer services)
        {
            _keyboardListener = new KeyboardListener();
            _mouseListener = new MouseListener();
            _gamePadListener = new GamePadListener(new GamePadListenerSettings());
            _touchListener = new TouchListener();

            if (services != null)
            {
                _gameServices = services;
                _gameServices.AddService(typeof(IInputService), this);
            }
        }

        #endregion

        #region Methods

        public void Initialize()
        {

        }

        /// <summary>Fires the UpdateOrderChanged event</summary>
        protected void OnUpdateOrderChanged()
        {
            if (UpdateOrderChanged != null)
            {
                UpdateOrderChanged(this, EventArgs.Empty);
            }
        }

        public void Update()
        {

        }

        /// <summary>Updates the state of all input devices</summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            _keyboardListener.Update(gameTime);
            _mouseListener.Update(gameTime);
            _gamePadListener.Update(gameTime);
            _touchListener.Update(gameTime);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~InputManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}
