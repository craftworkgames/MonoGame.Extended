using System;
using System.Reflection;
using System.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.InputListeners;

using GameEventHandler = System.EventHandler<System.EventArgs>;

namespace MonoGame.Extended.NuclexGui
{
    // TODO: Ownership issue with the GUI visualizer
    //   If an instance creates its own GUI visualizer (because the user didn't assign
    //   a custom one), it belongs to the instance and should be disposed. If the
    //   use does assign a custom visualizer, it shouldn't be disposed.
    //   But what if the user stores the current visualizer, then assigns a different
    //   one and assigns our's back?


    public class GuiManager : IGameComponent, IUpdateable, IDrawable, IDisposable, IGuiService
    {
        #region Events

        /// <summary>Fired when the DrawOrder property changes</summary>
        public event GameEventHandler DrawOrderChanged;

        /// <summary>Fired when the Visible property changes</summary>
        public event GameEventHandler VisibleChanged;

        /// <summary>Fired when the UpdateOrder property changes</summary>
        public event GameEventHandler UpdateOrderChanged;

        /// <summary>Fired when the enabled property changes, which is never</summary>
        event GameEventHandler IUpdateable.EnabledChanged { add { } remove { } }

        #endregion

        #region Properties

        /// <summary>Whether the component should be updated during Game.Update()</summary>
        bool IUpdateable.Enabled
        {
            get { return true; }
        }

        /// <summary>Whether the GUI should be drawn during Game.Draw()</summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (value != _visible)
                {
                    _visible = value;
                    OnVisibleChanged();
                }
            }
        }

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

        /// <summary>
        ///   The order in which to draw this object relative to other objects. Objects
        ///   with a lower value are drawn first.
        /// </summary>
        public int DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                if (value != _drawOrder)
                {
                    _drawOrder = value;
                    OnDrawOrderChanged();
                }
            }
        }

        /// <summary>Visualizer that draws the GUI onto the screen</summary>
        /// <remarks>
        ///   The GuiManager will dispose its visualizer together with itself. If you want
        ///   to keep the visualizer, unset it before disposing the GuiManager. If you want
        ///   to replace the GuiManager's visualizer after it has constructed the default
        ///   one, you should dispose the GuiManager's default visualizer after assigning
        ///   your own.
        /// </remarks>
        public Visuals.IGuiVisualizer Visualizer
        {
            get { return _guiVisualizer; }
            set
            {
                _guiVisualizer = value;
                _updateableGuiVisualizer = value as IUpdateable;
            }
        }

        /// <summary>Input capturer that collects data from the input devices</summary>
        /// <remarks>
        ///   The GuiManager will dispose its input capturer together with itself. If you
        ///   want to keep the input capturer, unset it before disposing the GuiManager.
        ///   If you want to replace the GuiManager's input capturer after it has constructed
        ///   the default one, you should dispose the GuiManager's default input capturer
        ///   after assigning your own.
        /// </remarks>
        public Input.IInputCapturer InputCapturer
        {
            get { return _inputCapturer; }
            set
            {
                if (!ReferenceEquals(value, _inputCapturer))
                {
                    if (_inputCapturer != null)
                    {
                        _inputCapturer.InputReceiver = null;
                    }

                    _inputCapturer = value;
                    _updateableInputCapturer = value as IUpdateable;

                    if (_inputCapturer != null)
                    {
                        _inputCapturer.InputReceiver = _screen;
                    }
                }
            }
        }

        /// <summary>GUI that is being rendered</summary>
        /// <remarks>
        ///   The GUI manager renders one GUI full-screen onto the primary render target
        ///   (the backbuffer). This property holds the GUI that is being managed by
        ///   the GUI manager component. You can replace it at any time, for example,
        ///   if the player opens or closes your ingame menu.
        /// </remarks>
        public GuiScreen Screen
        {
            get { return _screen; }
            set
            {
                _screen = value;

                // Someone could assign the screen before the component is initialized.
                // If that happens, do nothing here, the Initialize() method will take care
                // of assigning the screen to the input capturer once it is called.
                if (_inputCapturer != null)
                {
                    _inputCapturer.InputReceiver = _screen;
                }
            }
        }

        #endregion

        #region Fields

        /// <summary>Game service container the GUI has registered itself in</summary>
        private GameServiceContainer _gameServices;
        /// <summary>Graphics device servide the GUI uses</summary>
        private IGraphicsDeviceService _graphicsDeviceService;
        /// <summary>Input service the GUI uses</summary>
        private Input.IInputService _inputService;

        /// <summary>Update order rank relative to other game components</summary>
        private int _updateOrder;
        /// <summary>Draw order rank relative to other game components</summary>
        private int _drawOrder;
        /// <summary>Whether the GUI should be drawn by Game.Draw()</summary>
        private bool _visible = true;

        /// <summary>Captures user input for the XNA game</summary>
        private Input.IInputCapturer _inputCapturer;
        /// <summary>The IInputCapturer under its IUpdateable interface, if implemented</summary>
        private IUpdateable _updateableInputCapturer;
        /// <summary>Draws the GUI</summary>
        private Visuals.IGuiVisualizer _guiVisualizer;
        /// <summary>The IGuiVisualizer under its IUpdateable interface, if implemented</summary>
        private IUpdateable _updateableGuiVisualizer;

        /// <summary>The GUI screen representing the desktop</summary>
        private GuiScreen _screen;

        #endregion

        #region Constructors

        /// <summary>Initializes a new GUI manager using the XNA service container</summary>
        /// <param name="gameServices">
        ///   Game service container the GuiManager will register itself in and
        ///   to take the services it consumes from.
        /// </param>
        public GuiManager(GameServiceContainer gameServices)
        {
            _gameServices = gameServices;

            gameServices.AddService(typeof(IGuiService), this);

            // Do not look for the consumed services yet. XNA uses a two-stage
            // initialization where in the first stage all managers register themselves
            // before seeking out the services they use in the second stage, marked
            // by a call to the Initialize() method.
        }

        /// <summary>Initializes a new GUI manager without using the XNA service container</summary>
        /// <param name="graphicsDeviceService">Graphics device service the GUI will be rendered with</param>
        /// <param name="inputService">Input service used to read data from the input devices</param>
        /// <remarks>This constructor is provided for users of dependency injection frameworks.</remarks>
        public GuiManager(IGraphicsDeviceService graphicsDeviceService, Input.IInputService inputService)
        {
            _graphicsDeviceService = graphicsDeviceService;
            _inputService = inputService;
        }

        /// <summary>Initializes a new GUI manager using explicit services</summary>
        /// <param name="gameServices">Game service container the GuiManager will register itself in</param>
        /// <param name="graphicsDeviceService">Graphics device service the GUI will be rendered with</param>
        /// <param name="inputService">Input service used to read data from the input devices</param>
        /// <remarks>
        ///   This constructor is provided for users of dependency injection frameworks
        ///   or if you just want to be more explicit in stating which manager consumes
        ///   what services.
        /// </remarks>
        public GuiManager(GameServiceContainer gameServices, IGraphicsDeviceService graphicsDeviceService, Input.IInputService inputService) : this(gameServices)
        {

            _graphicsDeviceService = graphicsDeviceService;
            _inputService = inputService;
        }

        #endregion

        #region Methods

        /// <summary>Immediately releases all resources used the GUI manager</summary>
        public void Dispose()
        {

            // Unregister the service if we have registered it before
            if (_gameServices != null)
            {
                object registeredService = _gameServices.GetService(typeof(IGuiService));

                if (ReferenceEquals(registeredService, this))
                    _gameServices.RemoveService(typeof(IGuiService));
            }

            // Dispose the input capturer, if necessary
            if (_inputCapturer != null)
            {
                IDisposable disposableInputCapturer = _inputCapturer as IDisposable;

                if (disposableInputCapturer != null)
                    disposableInputCapturer.Dispose();

                _updateableInputCapturer = null;
                _inputCapturer = null;
            }

            // Dispose the GUI visualizer, if necessary
            if (_guiVisualizer != null)
            {
                IDisposable disposableguiVisualizer = _guiVisualizer as IDisposable;

                if (disposableguiVisualizer != null)
                    disposableguiVisualizer.Dispose();

                _updateableGuiVisualizer = null;
                _guiVisualizer = null;
            }

        }

        /// <summary>Handles second-stage initialization of the GUI manager</summary>
        public void Initialize()
        {

            // Set up a default input capturer if none was assigned by the user.
            // We only require an IInputService if the user doesn't use a custom input
            // capturer (which could be based on any other input library)
            if (_inputCapturer == null)
            {
                if (_inputService == null)
                    _inputService = getInputService(_gameServices);

                //_inputCapturer = new Input.DefaultInputCapturer(_inputService);
                _inputCapturer = new Input.MainInputCapturer(_inputService);

                // If a screen was assigned to the GUI before the input capturer was
                // created, then the input capturer hasn't been given the screen as its
                // input sink yet.
                if (_screen != null)
                    _inputCapturer.InputReceiver = _screen;
            }

            // Set up a default GUI visualizer if none was assigned by the user.
            // We only require an IGraphicsDeviceService if the user doesn't use a
            // custom visualizer (which could be using any kind of rendering)
            if (_guiVisualizer == null)
            {
                if (_graphicsDeviceService == null)
                    _graphicsDeviceService = getGraphicsDeviceService(_gameServices);

                // Use a private service container. We know exactly what will be loaded from
                // the content manager our default GUI visualizer creates and if the user is
                // being funny, the graphics device service passed to the constructor might
                // be different from the one registered in the game service container.
                var services = new GameServiceContainer();
                services.AddService(typeof(IGraphicsDeviceService), _graphicsDeviceService);
                
                Visualizer = Visuals.Flat.FlatGuiVisualizer.FromResource(services, "MonoGame.Extended.NuclexGui.Resources.Skins.SuaveSkin.json");
            }

        }

        /// <summary>Fires the UpdateOrderChanged event</summary>
        protected void OnUpdateOrderChanged()
        {
            if (UpdateOrderChanged != null)
                UpdateOrderChanged(this, EventArgs.Empty);
        }

        /// <summary>Fires the DrawOrderChanged event</summary>
        protected void OnDrawOrderChanged()
        {
            if (DrawOrderChanged != null)
                DrawOrderChanged(this, EventArgs.Empty);
        }

        /// <summary>Fires the VisibleChanged event</summary>
        protected void OnVisibleChanged()
        {
            if (VisibleChanged != null)
                VisibleChanged(this, EventArgs.Empty);
        }

        /// <summary>Retrieves the input service from a service provider</summary>
        /// <param name="serviceProvider">Service provider the input service is retrieved from</param>
        /// <returns>The retrieved input service</returns>
        private static Input.IInputService getInputService(IServiceProvider serviceProvider)
        {
            var inputService = (Input.IInputService)serviceProvider.GetService(typeof(Input.IInputService));

            if (inputService == null)
                throw new InvalidOperationException(
                  "Using the GUI with the default input capturer requires the IInputService. " +
                  "Please either add the IInputService to Game.Services by using the " +
                  "Nuclex.Input.InputManager in your game or provide a custom IInputCapturer " +
                  "implementation for the GUI and assign it before GuiManager.Initialize() " +
                  "is called."
                );

            return inputService;
        }

        /// <summary>Retrieves the graphics device service from a service provider</summary>
        /// <param name="serviceProvider">Service provider the graphics device service is retrieved from</param>
        /// <returns>The retrieved graphics device service</returns>
        private static IGraphicsDeviceService getGraphicsDeviceService(
          IServiceProvider serviceProvider
        )
        {
            var graphicsDeviceService = (IGraphicsDeviceService)serviceProvider.GetService(typeof(IGraphicsDeviceService));

            if (graphicsDeviceService == null)
                throw new InvalidOperationException(
                  "Using the GUI with the default visualizer requires the IGraphicsDeviceService. " +
                  "Please either add an IGraphicsDeviceService to Game.Services by using " +
                  "XNA's GraphicsDeviceManager in your game or provide a custom " +
                  "IGuiVisualizer implementation for the GUI and assign it before " +
                  "GuiManager.Initialize() is called."
                );

            return graphicsDeviceService;
        }

        /// <summary>Called when the component needs to update its state.</summary>
        /// <param name="gameTime">Provides a snapshot of the Game's timing values</param>
        public void Update(GameTime gameTime)
        {
            if (_updateableInputCapturer != null)
                _updateableInputCapturer.Update(gameTime);

            if (_updateableGuiVisualizer != null)
                _updateableGuiVisualizer.Update(gameTime);
        }

        /// <summary>Called when the drawable component needs to draw itself.</summary>
        /// <param name="gameTime">Provides a snapshot of the game's timing values</param>
        public void Draw(GameTime gameTime)
        {
            if (_guiVisualizer != null)
                if (_screen != null)
                    _guiVisualizer.Draw(_screen);
        }

        #endregion
    }
}
