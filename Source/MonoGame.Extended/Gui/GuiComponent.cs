using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Wip;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class GuiComponent : DrawableGameComponent
    {
        public GuiComponent(Game game) 
            : base(game)
        {
        }

        private ContentManager _contentManager;
        private IGuiContentService _contentService;
        private SpriteBatch _spriteBatch;
        private List<GuiControl> _controls;
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

        private static string ReadAllText(string assetName)
        {
            using (var stream = TitleContainer.OpenStream(assetName))
            using(var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void LoadGui(string assetName)
        {
            var guiPath = Path.Combine(_contentManager.RootDirectory, assetName);

            using (var stream = TitleContainer.OpenStream(guiPath))
            using (var streamReader = new StreamReader(stream))
            {
                var guiFile = GuiFile.Load(streamReader);
                var stylesPath = Path.Combine(_contentManager.RootDirectory, guiFile.Styles);
                //var guiDefinition = LoadGuiDefinition(stylesPath);

                var json = ReadAllText(stylesPath);
                var guiDefinition = JsonConvert.DeserializeObject<GuiDefinition>(json);
                var bitmapFonts = guiDefinition.Fonts
                    .Select(f => _contentManager.Load<BitmapFont>(f))
                    .ToArray();
                var textureAtlas = LoadTextureAtlas(guiDefinition.TextureAtlas);
                var converterService = new GuiJsonConverterService(textureAtlas, bitmapFonts);
                var jsonSerializer = new JsonSerializer();

                jsonSerializer.Converters.Add(new GuiJsonConverter(converterService));
                jsonSerializer.Converters.Add(new MonoGameColorJsonConverter());

                foreach (var controlData in guiFile.Controls)
                {
                    //_buttonTemplate = guiDefinition.Styles["BlueButton"].ToObject<GuiTemplate>(jsonSerializer);
                    //_labelTemplate = guiDefinition.Styles["BlueLabel"].ToObject<GuiTemplate>(jsonSerializer);
                    //_textBoxTemplate = guiDefinition.Styles["BlueTextBox"].ToObject<GuiTemplate>(jsonSerializer);

                    //var controlStyle = controlStyles[controlData.Style];
                    //var control = controlStyle.CreateControl(_contentService);

                    var control = new GuiButton(guiDefinition.Styles[controlData.Style].ToObject<GuiTemplate>(jsonSerializer));

                    if (control != null)
                    {
                        control.Name = controlData.Name;
                        control.Position = new Vector2(controlData.X, controlData.Y);
                        control.Size = new SizeF(controlData.Width, controlData.Height);
                        _controls.Add(control);
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _contentManager = new ContentManager(Game.Services, "Content");
            _contentService = new GuiContentService(_contentManager);
            _controls = new List<GuiControl>();
        }

        protected override void UnloadContent()
        {
            _contentManager.Unload();
            base.UnloadContent();
        }

        public TextureAtlas LoadTextureAtlas(string assetName)
        {
            using (var stream = TitleContainer.OpenStream(assetName))
            {
                return TextureAtlasReader.FromRawXml(_contentManager, stream);
            }
        }

        //private static Dictionary<string, GuiControlStyle> LoadStyles(string stylesPath)
        //{
        //    using (var stream = TitleContainer.OpenStream(stylesPath))
        //    using (var streamReader = new StreamReader(stream))
        //    {
        //        var stylesFile = GuiStyleFile.Load(streamReader);
        //        return stylesFile.Styles.ToDictionary(s => s.Name);
        //    }
        //}

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);

            foreach (var control in _controls)
                control.Draw(_spriteBatch);

            //_buttonTemplate.Draw(_spriteBatch, new RectangleF(100, 100, 300, 100));
            //_labelTemplate.Draw(_spriteBatch, new RectangleF(200, 200, 300, 100));
            //_textBoxTemplate.Draw(_spriteBatch, new RectangleF(200, 300, 300, 50));

            _spriteBatch.End();
        }
    }
}