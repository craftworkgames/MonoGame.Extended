using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Gui
{
    public class GuiManager
    {
        private readonly IGuiRenderer _renderer;
        private readonly MouseListener _mouseListener;
        private readonly KeyboardListener _keyboardListener;

        private GuiControl _focusedControl;
        private GuiControl _hoveredControl;

        public GuiManager(ViewportAdapter viewportAdapter, IGuiRenderer renderer)
        {
            _renderer = renderer;

            _mouseListener = new MouseListener(viewportAdapter);
            _mouseListener.MouseClicked += OnMouseClicked;
            _mouseListener.MouseMoved += OnMouseMoved;
            _mouseListener.MouseDown += (sender, args) => _hoveredControl?.OnMouseDown(args);
            _mouseListener.MouseUp += (sender, args) => _hoveredControl?.OnMouseUp(args);

            _keyboardListener = new KeyboardListener();
            _keyboardListener.KeyTyped += (sender, args) => _focusedControl?.OnKeyTyped(args);
        }

        public GuiScreen Screen { get; set; }

        public void Update(GameTime gameTime)
        {
            _mouseListener.Update(gameTime);
            _keyboardListener.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (Screen != null)
                _renderer.Draw(Screen);
        }

        private void OnMouseMoved(object sender, MouseEventArgs args)
        {
            if (Screen == null)
                return;

            var hoveredControl = FindControlAtPoint(Screen.Controls, args.Position);

            if (_hoveredControl != hoveredControl)
            {
                _hoveredControl?.OnMouseLeave(args);
                _hoveredControl = hoveredControl;
                _hoveredControl?.OnMouseEnter(args);
            }
        }

        private void OnMouseClicked(object sender, MouseEventArgs mouseEventArgs)
        {
            if (Screen == null)
                return;

            var focusedControl = FindControlAtPoint(Screen.Controls, mouseEventArgs.Position);

            if (_focusedControl != focusedControl)
            {
                if (_focusedControl != null)
                    _focusedControl.IsFocused = false;

                _focusedControl = focusedControl;

                if (_focusedControl != null)
                    _focusedControl.IsFocused = true;
            }
        }

        private static GuiControl FindControlAtPoint(GuiControlCollection controls, Point point)
        {
            var topMostControl = (GuiControl) null;

            for (var i = controls.Count - 1; i >= 0; i--)
            {
                var control = controls[i];

                if (topMostControl == null && control.BoundingRectangle.Contains(point))
                    topMostControl = control;
                
                if (control.Controls.Any())
                {
                    var child = FindControlAtPoint(control.Controls, point);

                    if (child != null)
                        topMostControl = child;
                }
            }

            return topMostControl;
        }

    }
}