using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.NuclexGui.Input;
using MonoGame.Extended.NuclexGui.Visuals;
using MonoGame.Extended.NuclexGui.Visuals.Flat;
using GameEventHandler = System.EventHandler<System.EventArgs>;

namespace MonoGame.Extended.NuclexGui
{
    //   If an instance creates its own GUI visualizer (because the user didn't assign
    //   a custom one), it belongs to the instance and should be disposed. If the
    //   use does assign a custom visualizer, it shouldn't be disposed.
    //   But what if the user stores the current visualizer, then assigns a different
    //   one and assigns our's back?


    public class GuiManager : IGameComponent, IUpdateable, IDrawable, IDisposable, IGuiService
    {
        /// <summary>Draw order rank relative to other game components</summary>
        private int _drawOrder;

        /// <summary>Game service container the GUI has registered itself in</summary>
        private readonly GameServiceContainer _gameServices;

        /// <summary>Graphics device servide the GUI uses</summary>
        private IGraphicsDeviceService _graphicsDeviceService;

        /// <summary>Draws the GUI</summary>
        private IGuiVisualizer _guiVisualizer;

        /// <summary>Captures user input for the XNA game</summary>
        private IInputCapturer _inputCapturer;

        /// <summary>Input service the GUI uses</summary>
        private IGuiInputService _inputService;

        /// <summary>The GUI screen representing the desktop</summary>
        private GuiScreen _screen;

        /// <summary>The IGuiVisualizer under its IUpdateable interface, if implemented</summary>
        private IUpdateable _updateableGuiVisualizer;

        /// <summary>The IInputCapturer under its IUpdateable interface, if implemented</summary>
        private IUpdateable _updateableInputCapturer;

        /// <summary>Update order rank relative to other game components</summary>
        private int _updateOrder;

        /// <summary>Whether the GUI should be drawn by Game.Draw()</summary>
        private bool _visible = true;

        /// <summary>Initializes a new GUI manager using the XNA service container</summary>
        /// <param name="gameServices">
        ///     Game service container the GuiManager will register itself in and
        ///     to take the services it consumes from.
        /// </param>
        /// <param name="inputService">The input service used by the GUI</param>
        public GuiManager(GameServiceContainer gameServices, IGuiInputService inputService)
        {
            _gameServices = gameServices;
            _inputService = inputService;

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
        public GuiManager(IGraphicsDeviceService graphicsDeviceService, IGuiInputService inputService)
        {
            _graphicsDeviceService = graphicsDeviceService;
            _inputService = inputService;
        }

        /// <summary>Initializes a new GUI manager using explicit services</summary>
        /// <param name="gameServices">Game service container the GuiManager will register itself in</param>
        /// <param name="graphicsDeviceService">Graphics device service the GUI will be rendered with</param>
        /// <param name="inputService">Input service used to read data from the input devices</param>
        /// <remarks>
        ///     This constructor is provided for users of dependency injection frameworks
        ///     or if you just want to be more explicit in stating which manager consumes
        ///     what services.
        /// </remarks>
        public GuiManager(GameServiceContainer gameServices, IGraphicsDeviceService graphicsDeviceService, IGuiInputService inputService) 
            : this(gameServices, inputService)
        {
            _graphicsDeviceService = graphicsDeviceService;
            _inputService = inputService;
        }

        /// <summary>Input capturer that collects data from the input devices</summary>
        /// <remarks>
        ///     The GuiManager will dispose its input capturer together with itself. If you
        ///     want to keep the input capturer, unset it before disposing the GuiManager.
        ///     If you want to replace the GuiManager's input capturer after it has constructed
        ///     the default one, you should dispose the GuiManager's default input capturer
        ///     after assigning your own.
        /// </remarks>
        public IInputCapturer InputCapturer
        {
            get { return _inputCapturer; }
            set
            {
                if (!ReferenceEquals(value, _inputCapturer))
                {
                    if (_inputCapturer != null)
                        _inputCapturer.InputReceiver = null;

                    _inputCapturer = value;
                    _updateableInputCapturer = value as IUpdateable;

                    if (_inputCapturer != null)
                        _inputCapturer.InputReceiver = _screen;
                }
            }
        }

        /// <summary>Immediately releases all resources used the GUI manager</summary>
        public void Dispose()
        {
            // Unregister the service if we have registered it before
            if (_gameServices != null)
            {
                var registeredService = _gameServices.GetService(typeof(IGuiService));

                if (ReferenceEquals(registeredService, this))
                    _gameServices.RemoveService(typeof(IGuiService));
            }

            // Dispose the input capturer, if necessary
            if (_inputCapturer != null)
            {
                var disposableInputCapturer = _inputCapturer as IDisposable;

                disposableInputCapturer?.Dispose();

                _updateableInputCapturer = null;
                _inputCapturer = null;
            }

            // Dispose the GUI visualizer, if necessary
            if (_guiVisualizer != null)
            {
                var disposableguiVisualizer = _guiVisualizer as IDisposable;

                disposableguiVisualizer?.Dispose();

                _updateableGuiVisualizer = null;
                _guiVisualizer = null;
            }
        }

        /// <summary>Fired when the DrawOrder property changes</summary>
        public event GameEventHandler DrawOrderChanged;

        /// <summary>Fired when the Visible property changes</summary>
        public event GameEventHandler VisibleChanged;

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
        ///     The order in which to draw this object relative to other objects. Objects
        ///     with a lower value are drawn first.
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

        /// <summary>Called when the drawable component needs to draw itself.</summary>
        /// <param name="gameTime">Provides a snapshot of the game's timing values</param>
        public void Draw(GameTime gameTime)
        {
            if (_guiVisualizer != null)
            {
                if (_screen != null)
                    _guiVisualizer.Draw(_screen);
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
                    _inputService = GetInputService(_gameServices);

                //_inputCapturer = new Input.DefaultInputCapturer(_inputService);
                _inputCapturer = new DefaultInputCapturer(_inputService);

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
                    _graphicsDeviceService = GetGraphicsDeviceService(_gameServices);

                // Use a private service container. We know exactly what will be loaded from
                // the content manager our default GUI visualizer creates and if the user is
                // being funny, the graphics device service passed to the constructor might
                // be different from the one registered in the game service container.
                var services = new GameServiceContainer();
                services.AddService(typeof(IGraphicsDeviceService), _graphicsDeviceService);

                Visualizer = FlatGuiVisualizer.FromResource(services,
                    "MonoGame.Extended.NuclexGui.Resources.Skins.SuaveSkin.json");
            }
        }

        /// <summary>Visualizer that draws the GUI onto the screen</summary>
        /// <remarks>
        ///     The GuiManager will dispose its visualizer together with itself. If you want
        ///     to keep the visualizer, unset it before disposing the GuiManager. If you want
        ///     to replace the GuiManager's visualizer after it has constructed the default
        ///     one, you should dispose the GuiManager's default visualizer after assigning
        ///     your own.
        /// </remarks>
        public IGuiVisualizer Visualizer
        {
            get { return _guiVisualizer; }
            set
            {
                _guiVisualizer = value;
                _updateableGuiVisualizer = value as IUpdateable;
            }
        }

        /// <summary>GUI that is being rendered</summary>
        /// <remarks>
        ///     The GUI manager renders one GUI full-screen onto the primary render target
        ///     (the backbuffer). This property holds the GUI that is being managed by
        ///     the GUI manager component. You can replace it at any time, for example,
        ///     if the player opens or closes your ingame menu.
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
                    _inputCapturer.InputReceiver = _screen;
            }
        }

        /// <summary>Fired when the UpdateOrder property changes</summary>
        public event GameEventHandler UpdateOrderChanged;

        /// <summary>Fired when the enabled property changes, which is never</summary>
        event GameEventHandler IUpdateable.EnabledChanged
        {
            add { }
            remove { }
        }

        /// <summary>Whether the component should be updated during Game.Update()</summary>
        bool IUpdateable.Enabled => true;

        /// <summary>
        ///     Indicates when the game component should be updated relative to other game
        ///     components. Lower values are updated first.
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

        /// <summary>Called when the component needs to update its state.</summary>
        /// <param name="gameTime">Provides a snapshot of the Game's timing values</param>
        public void Update(GameTime gameTime)
        {
            _updateableInputCapturer?.Update(gameTime);
            _updateableGuiVisualizer?.Update(gameTime);
        }

        /// <summary>Fires the UpdateOrderChanged event</summary>
        protected void OnUpdateOrderChanged()
        {
            UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Fires the DrawOrderChanged event</summary>
        protected void OnDrawOrderChanged()
        {
            DrawOrderChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Fires the VisibleChanged event</summary>
        protected void OnVisibleChanged()
        {
            VisibleChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Retrieves the input service from a service provider</summary>
        /// <param name="serviceProvider">Service provider the input service is retrieved from</param>
        /// <returns>The retrieved input service</returns>
        private static IGuiInputService GetInputService(IServiceProvider serviceProvider)
        {
            var inputService = (IGuiInputService) serviceProvider.GetService(typeof(IGuiInputService));

            if (inputService == null)
            {
                throw new InvalidOperationException(
                    "Using the GUI with the default input capturer requires the IInputService. " +
                    "Please either add the IInputService to Game.Services by using the " +
                    "Nuclex.Input.InputManager in your game or provide a custom IInputCapturer " +
                    "implementation for the GUI and assign it before GuiManager.Initialize() " +
                    "is called."
                );
            }

            return inputService;
        }

        /// <summary>Retrieves the graphics device service from a service provider</summary>
        /// <param name="serviceProvider">Service provider the graphics device service is retrieved from</param>
        /// <returns>The retrieved graphics device service</returns>
        private static IGraphicsDeviceService GetGraphicsDeviceService(
            IServiceProvider serviceProvider
        )
        {
            var graphicsDeviceService =
                (IGraphicsDeviceService) serviceProvider.GetService(typeof(IGraphicsDeviceService));

            if (graphicsDeviceService == null)
            {
                throw new InvalidOperationException(
                    "Using the GUI with the default visualizer requires the IGraphicsDeviceService. " +
                    "Please either add an IGraphicsDeviceService to Game.Services by using " +
                    "XNA's GraphicsDeviceManager in your game or provide a custom " +
                    "IGuiVisualizer implementation for the GUI and assign it before " +
                    "GuiManager.Initialize() is called."
                );
            }

            return graphicsDeviceService;
        }
    }
}