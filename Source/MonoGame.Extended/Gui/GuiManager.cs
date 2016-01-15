using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Gui
{
    public class GuiManager : IDraw, IUpdate
    {
        private readonly ViewportAdapter _viewportAdapter;
        private readonly InputListenerManager _inputManager;
        private readonly SpriteBatch _spriteBatch;

        public GuiManager(ViewportAdapter viewportAdapter, GraphicsDevice graphicsDevice)
        {
            _viewportAdapter = viewportAdapter;
            _inputManager = new InputListenerManager(viewportAdapter);
            _spriteBatch = new SpriteBatch(graphicsDevice);

            Controls = new List<GuiControl>();

            var mouseListener = _inputManager.AddListener<MouseListener>();
            mouseListener.MouseMoved += (sender, args) => DelegateMouseEvent(args, c => c.OnMouseMoved(this, args));
            mouseListener.MouseDown += (sender, args) => DelegateMouseEvent(args, c => c.OnMouseDown(this, args));
            mouseListener.MouseUp += (sender, args) => DelegateMouseEvent(args, c => c.OnMouseUp(this, args));
        }

        public List<GuiControl> Controls { get; private set; }

        private void DelegateMouseEvent(MouseEventArgs args, Action<GuiControl> action)
        {
            foreach (var control in Controls
                .Where(control => control.Contains(args.Position)))
            {
                action(control);
            }
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(transformMatrix: _viewportAdapter.GetScaleMatrix());

            foreach (var control in Controls.OfType<GuiDrawableControl>())
                control.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            _inputManager.Update(gameTime);

            foreach (var control in Controls)
                control.Update(gameTime);
        }
    }
}