using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Layouts;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Gui
{
    public class GuiManager : IDraw, IUpdate
    {
        private readonly ViewportAdapter _viewportAdapter;
        private readonly InputListenerManager _inputManager;
        private readonly SpriteBatch _spriteBatch;
        private GuiControl _focusedControl;

        public GuiManager(ViewportAdapter viewportAdapter, GraphicsDevice graphicsDevice)
        {
            Layout = new GuiGridLayout();

            _viewportAdapter = viewportAdapter;
            _inputManager = new InputListenerManager(viewportAdapter);
            _spriteBatch = new SpriteBatch(graphicsDevice);

            var mouseListener = _inputManager.AddListener<MouseListener>();
            mouseListener.MouseMoved += OnMouseMoved;
            mouseListener.MouseDown += (sender, args) => Layout.OnMouseDown(this, args);
            mouseListener.MouseUp += (sender, args) => Layout.OnMouseUp(this, args);
        }

        private void OnMouseMoved(object sender, MouseEventArgs args)
        {
            if (_focusedControl != null)
                _focusedControl.OnMouseLeave(this, args);

            _focusedControl = FindControlAtPoint(Layout, args.Position);

            if (_focusedControl != null)
                _focusedControl.OnMouseEnter(this, args);

            Layout.OnMouseMoved(this, args);
        }

        private GuiLayoutControl _layout;
        public GuiLayoutControl Layout
        {
            get { return _layout; }
            set
            {
                if (value == null)
                    throw new InvalidOperationException("Layout must not be null");

                _layout = value;
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(transformMatrix: _viewportAdapter.GetScaleMatrix());
            Layout.Draw(_spriteBatch, _viewportAdapter.BoundingRectangle);
            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            _inputManager.Update(gameTime);
            Layout.Update(gameTime);
        }

        public void PerformLayout()
        {
            Layout.LayoutChildren(_viewportAdapter.BoundingRectangle);
        }

        private static GuiControl FindControlAtPoint(GuiControl control, Point point)
        {
            var layoutControl = control as GuiLayoutControl;

            if (layoutControl != null)
            {
                var children = layoutControl.Children;

                for(var i = children.Count - 1; i >= 0; i--)
                {
                    var child = children[i];

                    if (!child.Contains(point))
                        continue;

                    var c = FindControlAtPoint(child, point);

                    if (c != null)
                        return c;
                }
            }

            if (control.Contains(point))
                return control;

            return null;
        }
    }
}