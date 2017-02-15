using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Gui
{
    public class GuiSystem
    {
        private readonly IGuiRenderer _renderer;
        private readonly MouseListener _mouseListener;
        private readonly KeyboardListener _keyboardListener;

        private GuiControl _focusedControl;
        private GuiControl _hoveredControl;

        public GuiSystem(ViewportAdapter viewportAdapter, IGuiRenderer renderer)
        {
            _renderer = renderer;

            _mouseListener = new MouseListener(viewportAdapter);
            _mouseListener.MouseClicked += OnMouseClicked;
            _mouseListener.MouseMoved += OnMouseMoved;
            _mouseListener.MouseDown += (sender, args) => _hoveredControl?.OnMouseDown(args);
            _mouseListener.MouseUp += (sender, args) => _hoveredControl?.OnMouseUp(args);

            _keyboardListener = new KeyboardListener();
            _keyboardListener.KeyTyped += (sender, args) => _focusedControl?.OnKeyTyped(args);
            _keyboardListener.KeyPressed += (sender, args) => _focusedControl?.OnKeyPressed(args);
        }

        public GuiScreen Screen { get; set; }
        public Vector2 CursorPosition { get; set; }

        public void Update(GameTime gameTime)
        {
            _mouseListener.Update(gameTime);
            _keyboardListener.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            var deltaSeconds = gameTime.GetElapsedSeconds();

            _renderer.Begin();

            if (Screen != null)
            {
                DrawChildren(Screen.Controls, deltaSeconds);

                var cursor = Screen.Skin?.Cursor;

                if (cursor != null)
                    _renderer.DrawRegion(cursor.TextureRegion, CursorPosition, cursor.Color);
            }

            _renderer.End();
        }
        
        private void DrawChildren(GuiControlCollection controls, float deltaSeconds)
        {
            foreach (var control in controls.Where(c => c.IsVisible))
            {
                //_renderer.DrawRegion(null, control.BoundingRectangle.ToRectangle(), Color.Magenta);
                control.Draw(_renderer, deltaSeconds);
            }

            foreach (var childControl in controls.Where(c => c.IsVisible))
                DrawChildren(childControl.Controls, deltaSeconds);
        }

        private void OnMouseMoved(object sender, MouseEventArgs args)
        {
            CursorPosition = args.Position.ToVector2();

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

                if (control.IsVisible)
                {
                    if (topMostControl == null && control.BoundingRectangle.Contains(point))
                        topMostControl = control;

                    if (control.Controls.Any())
                    {
                        var child = FindControlAtPoint(control.Controls, point);

                        if (child != null)
                            topMostControl = child;
                    }
                }
            }

            return topMostControl;
        }

    }
}