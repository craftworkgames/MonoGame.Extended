using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Gui
{
    public interface IGuiContext
    {
        BitmapFont DefaultFont { get; }
        Vector2 CursorPosition { get; }
    }

    public class GuiSystem : IGuiContext
    {
        private readonly ViewportAdapter _viewportAdapter;
        private readonly IGuiRenderer _renderer;
        private readonly MouseListener _mouseListener;
        private readonly TouchListener _touchListener;
        private readonly KeyboardListener _keyboardListener;

        private GuiControl _preFocusedControl;
        private GuiControl _focusedControl;
        private GuiControl _hoveredControl;

        public GuiSystem(ViewportAdapter viewportAdapter, IGuiRenderer renderer)
        {
            _viewportAdapter = viewportAdapter;
            _renderer = renderer;

            _mouseListener = new MouseListener(viewportAdapter);
            _mouseListener.MouseMoved += (s, e) => OnPointerMoved(GuiPointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseDown += (s, e) => OnPointerDown(GuiPointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseUp += (s, e) => OnPointerUp(GuiPointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseWheelMoved += (s, e) => _focusedControl?.OnScrolled(e.ScrollWheelDelta);

            _touchListener = new TouchListener(viewportAdapter);
            _touchListener.TouchStarted += (s, e) => OnPointerDown(GuiPointerEventArgs.FromTouchArgs(e));
            _touchListener.TouchMoved += (s, e) => OnPointerMoved(GuiPointerEventArgs.FromTouchArgs(e));
            _touchListener.TouchEnded += (s, e) => OnPointerUp(GuiPointerEventArgs.FromTouchArgs(e));

            _keyboardListener = new KeyboardListener();
            _keyboardListener.KeyTyped += (sender, args) => _focusedControl?.OnKeyTyped(this, args);
            _keyboardListener.KeyPressed += (sender, args) => _focusedControl?.OnKeyPressed(this, args);
        }

        private GuiScreen _screen;
        public GuiScreen Screen
        {
            get { return _screen; }
            set
            {
                if (_screen != value)
                {
                    _screen = value;
                    _screen?.Layout(this, _viewportAdapter.BoundingRectangle);
                }
            }
        }
        public Vector2 CursorPosition { get; set; }
        public BitmapFont DefaultFont => Screen?.Skin?.DefaultFont;

        public void Update(GameTime gameTime)
        {
            _touchListener.Update(gameTime);
            _mouseListener.Update(gameTime);
            _keyboardListener.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if(Screen == null || !Screen.IsVisible)
                return;

            var deltaSeconds = gameTime.GetElapsedSeconds();

            _renderer.Begin();

            DrawChildren(Screen.Controls, deltaSeconds);

            var cursor = Screen.Skin?.Cursor;

            if (cursor != null)
                _renderer.DrawRegion(cursor.TextureRegion, CursorPosition, cursor.Color);

            _renderer.End();
        }
        
        private void DrawChildren(GuiControlCollection controls, float deltaSeconds)
        {
            foreach (var control in controls.Where(c => c.IsVisible))
                control.Draw(this, _renderer, deltaSeconds);

            foreach (var childControl in controls.Where(c => c.IsVisible))
                DrawChildren(childControl.Controls, deltaSeconds);
        }

        private void OnPointerDown(GuiPointerEventArgs args)
        {
            if (Screen == null || !Screen.IsVisible)
                return;

            _preFocusedControl = FindControlAtPoint(Screen.Controls, args.Position);
            _hoveredControl?.OnPointerDown(this, args);
        }

        private void OnPointerUp(GuiPointerEventArgs args)
        {
            if (Screen == null || !Screen.IsVisible)
                return;

            var postFocusedControl = FindControlAtPoint(Screen.Controls, args.Position);

            if (_preFocusedControl == postFocusedControl)
            {
                var focusedControl = postFocusedControl;

                if (_focusedControl != focusedControl)
                {
                    if (_focusedControl != null)
                        _focusedControl.IsFocused = false;

                    _focusedControl = focusedControl;

                    if (_focusedControl != null)
                        _focusedControl.IsFocused = true;
                }
            }

            _preFocusedControl = null;
            _hoveredControl?.OnPointerUp(this, args);
        }

        private void OnPointerMoved(GuiPointerEventArgs args)
        {
            CursorPosition = args.Position.ToVector2();

            if (Screen == null || !Screen.IsVisible)
                return;

            var hoveredControl = FindControlAtPoint(Screen.Controls, args.Position);

            if (_hoveredControl != hoveredControl)
            {
                _hoveredControl?.OnPointerLeave(this, args);
                _hoveredControl = hoveredControl;
                _hoveredControl?.OnPointerEnter(this, args);
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