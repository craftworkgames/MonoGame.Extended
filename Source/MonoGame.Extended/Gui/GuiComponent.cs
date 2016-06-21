using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class GuiComponent : DrawableGameComponent
    {
        public GuiComponent(Game game, string guiFile) 
            : base(game)
        {
            _guiFile = guiFile;
        }

        private readonly string _guiFile;
        private ContentManager _contentManager;
        private GuiContentService _contentService;
        private SpriteBatch _spriteBatch;
        private List<GuiControl> _controls;
        private Dictionary<string, GuiControlStyle> _controlStyles;
        private InputListenerManager _inputManager;
        private GuiControl _hoveredControl;
        private GuiControl _focusedControl;

        protected override void Dispose(bool disposing)
        {
            _contentManager.Dispose();
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            base.Initialize();

            var viewportAdapter = new BoxingViewportAdapter(Game.Window, GraphicsDevice, 800, 480);
            _inputManager = new InputListenerManager(viewportAdapter);

            var mouseListener = _inputManager.AddListener<MouseListener>();
            mouseListener.MouseClicked += OnMouseClicked;
            mouseListener.MouseMoved += OnMouseMoved;
            mouseListener.MouseDown += (sender, args) => _hoveredControl?.OnMouseDown(sender, args);
            mouseListener.MouseUp += (sender, args) => _hoveredControl?.OnMouseUp(sender, args);

            var keyboardListener = _inputManager.AddListener<KeyboardListener>();
            keyboardListener.KeyTyped += (sender, args) => _focusedControl?.OnKeyTyped(sender, args);
        }

        private void OnMouseMoved(object sender, MouseEventArgs args)
        {
            var hoveredControl = FindControlAtPoint(_controls, args.Position);

            if (_hoveredControl != hoveredControl)
            {
                _hoveredControl?.OnMouseLeave(this, args);
                _hoveredControl = hoveredControl;
                _hoveredControl?.OnMouseEnter(this, args);
            }

            //ForEachChildAtPoint(args.Position, c => c.OnMouseMoved(this, args));
        }

        private void OnMouseClicked(object sender, MouseEventArgs mouseEventArgs)
        {
            var focusedControl = FindControlAtPoint(_controls, mouseEventArgs.Position);

            if (_focusedControl != focusedControl)
            {
                if (_focusedControl != null)
                    _focusedControl.IsFocused = false;

                _focusedControl = focusedControl;

                if (_focusedControl != null)
                    _focusedControl.IsFocused = true;
            }
        }

        private static GuiControl FindControlAtPoint(IList<GuiControl> controls, Point point)
        {
            for (var i = controls.Count - 1; i >= 0; i--)
            {
                var child = controls[i];

                if (child.Contains(point))
                {
                    //var layoutControl = child as GuiLayoutControl;

                    //if (layoutControl != null)
                    //{
                    //    var c = FindControlAtPoint(layoutControl.Controls, point);

                    //    if (c != null)
                    //        return c;
                    //}

                    return child;
                }
            }

            return null;
        }

        public override void Update(GameTime gameTime)
        {
            _inputManager.Update(gameTime);

            foreach (var control in _controls)
                control.Update(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _contentManager = new ContentManager(Game.Services, "Content");
            _contentService = new GuiContentService(_contentManager);
            _controls = new List<GuiControl>();

            var guiPath = Path.Combine(_contentManager.RootDirectory, _guiFile);

            using (var stream = TitleContainer.OpenStream(guiPath))
            using (var streamReader = new StreamReader(stream))
            {
                var guiFile = GuiFile.Load(streamReader);
                var stylesPath = Path.Combine(_contentManager.RootDirectory, guiFile.Styles);

                _controlStyles = LoadStyles(stylesPath);
                
                foreach (var controlData in guiFile.Controls)
                {
                    var controlStyle = _controlStyles[controlData.Style];
                    var control = controlStyle.CreateControl(_contentService);

                    if (control != null)
                    {
                        control.Name = controlData.Name;
                        control.Position = new Vector2(controlData.X, controlData.Y);
                        control.Size = new Vector2(controlData.Width, controlData.Height);
                        _controls.Add(control);
                    }
                }
            }
        }

        protected override void UnloadContent()
        {
            _contentManager.Unload();
            base.UnloadContent();
        }

        private static Dictionary<string, GuiControlStyle> LoadStyles(string stylesPath)
        {
            using (var stream = TitleContainer.OpenStream(stylesPath))
            using (var textReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(textReader))
            {
                var serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Objects,
                    Converters = { new MonoGameColorJsonConverter() }
                };

                return serializer.Deserialize<GuiControlStyle[]>(reader)
                    .ToDictionary(s => s.Name);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);

            foreach (var control in _controls)
                control.Draw(_spriteBatch);

            _spriteBatch.End();
        }
    }
}