using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class GuiComponent : DrawableGameComponent
    {
        private ContentManager _contentManager;
        private GuiControl _focusedControl;
        private GuiControl _hoveredControl;
        private GuiLayout _layout;
        private SpriteBatch _spriteBatch;

        public GuiComponent(Game game)
            : base(game)
        {
        }

        protected override void Dispose(bool disposing)
        {
            _contentManager.Dispose();
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            base.Initialize();

            var viewportAdapter = new BoxingViewportAdapter(Game.Window, GraphicsDevice, 800, 480);

            var mouseListener = new MouseListener(viewportAdapter);
            mouseListener.MouseClicked += OnMouseClicked;
            mouseListener.MouseMoved += OnMouseMoved;
            mouseListener.MouseDown += (sender, args) => _hoveredControl?.OnMouseDown(sender, args);
            mouseListener.MouseUp += (sender, args) => _hoveredControl?.OnMouseUp(sender, args);

            var keyboardListener = new KeyboardListener();
            keyboardListener.KeyTyped += (sender, args) => _focusedControl?.OnKeyTyped(sender, args);

            Game.Components.Add(new InputListenerComponent(Game, mouseListener, keyboardListener));
        }

        private void OnMouseMoved(object sender, MouseEventArgs args)
        {
            if (_layout == null)
                return;

            var hoveredControl = FindControlAtPoint(_layout.Controls, args.Position);

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
            if (_layout == null)
                return;

            var focusedControl = FindControlAtPoint(_layout.Controls, mouseEventArgs.Position);

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
                    return child;
            }

            return null;
        }

        public override void Update(GameTime gameTime)
        {
            if (_layout == null)
                return;

            foreach (var control in _layout.Controls)
                control.Update(gameTime);
        }

        public GuiLayout LoadGui(string assetName)
        {
            var guiPath = Path.Combine(_contentManager.RootDirectory, assetName);

            using (var stream = TitleContainer.OpenStream(guiPath))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    var json = streamReader.ReadToEnd();
                    var converters = new JsonConverter[]
                    {
                        new ColorJsonConverter(),
                        new Vector2JsonConverter(),
                        new SizeFJsonConverter(),
                        new GuiThicknessConveter(),
                        new GuiLayoutConverter(_contentManager),
                        new ContentConverter<BitmapFont>(_contentManager),
                        new GuiTextureAtlasConverter(_contentManager)
                    };

                    _layout = JsonConvert.DeserializeObject<GuiLayout>(json, converters);
                    return _layout;
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _contentManager = new ContentManager(Game.Services, "Content");
        }

        protected override void UnloadContent()
        {
            _contentManager.Unload();

            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_layout == null)
                return;

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);

            foreach (var control in _layout.Controls)
                control.Draw(_spriteBatch);

            _spriteBatch.End();
        }
    }
}