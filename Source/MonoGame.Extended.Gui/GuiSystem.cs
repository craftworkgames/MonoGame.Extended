using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.ComponentModel;

namespace MonoGame.Extended.Gui
{
    public interface IGuiContext
    {
        BitmapFont DefaultFont { get; }
        Vector2 CursorPosition { get; }
        GuiControl FocusedControl { get; }

        void SetFocus(GuiControl focusedControl);
    }

    public class GuiSystem : IGuiContext, IRectangular
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
            _mouseListener.MouseDown += (s, e) => OnPointerDown(GuiPointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseMoved += (s, e) => OnPointerMoved(GuiPointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseUp += (s, e) => OnPointerUp(GuiPointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseWheelMoved += (s, e) => _focusedControl?.OnScrolled(e.ScrollWheelDelta);

            _touchListener = new TouchListener(viewportAdapter);
            _touchListener.TouchStarted += (s, e) => OnPointerDown(GuiPointerEventArgs.FromTouchArgs(e));
            _touchListener.TouchMoved += (s, e) => OnPointerMoved(GuiPointerEventArgs.FromTouchArgs(e));
            _touchListener.TouchEnded += (s, e) => OnPointerUp(GuiPointerEventArgs.FromTouchArgs(e));

            _keyboardListener = new KeyboardListener();
            _keyboardListener.KeyTyped += (sender, args) => PropagateDown(_focusedControl, x => x.OnKeyTyped(this, args));
            _keyboardListener.KeyPressed += (sender, args) => PropagateDown(_focusedControl, x => x.OnKeyPressed(this, args));

            Screens = new GuiScreenCollection(this) { ItemAdded = InitializeScreen };
        }

        public GuiScreenCollection Screens { get; }

        public GuiControl FocusedControl { get { return _focusedControl; } }

        public GuiScreen ActiveScreen => Screens.LastOrDefault();

        public Rectangle BoundingRectangle => _viewportAdapter.BoundingRectangle;

        public Vector2 CursorPosition { get; set; }

        public BitmapFont DefaultFont => ActiveScreen?.Skin?.DefaultFont;

        private void InitializeScreen(GuiScreen screen)
        {
            screen.Layout(this, BoundingRectangle);
        }

        public void Update(GameTime gameTime)
        {
            _touchListener.Update(gameTime);
            _mouseListener.Update(gameTime);
            _keyboardListener.Update(gameTime);

            foreach (var screen in Screens)
            {
                if (screen.IsLayoutRequired)
                    screen.Layout(this, BoundingRectangle);

                screen.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            var deltaSeconds = gameTime.GetElapsedSeconds();

            _renderer.Begin();

            foreach (var screen in Screens)
            {
                if (screen.IsVisible)
                {
                    DrawChildren(screen.Controls, deltaSeconds);
                    DrawWindows(screen.Windows, deltaSeconds);
                }
            }

            var cursor = ActiveScreen.Skin?.Cursor;

            if (cursor != null)
                _renderer.DrawRegion(cursor.TextureRegion, CursorPosition, cursor.Color);

            _renderer.End();
        }

        private void DrawWindows(GuiWindowCollection windows, float deltaSeconds)
        {
            foreach (var window in windows)
            {
                window.Draw(this, _renderer, deltaSeconds);
                DrawChildren(window.Controls, deltaSeconds);
            }
        }

        private void DrawChildren(GuiControlCollection controls, float deltaSeconds)
        {
            foreach (var control in controls.Where(c => c.IsVisible))
                control.Draw(this, _renderer, deltaSeconds);

            foreach (var childControl in controls.Where(c => c.IsVisible))
                DrawChildren(childControl.Controls, deltaSeconds);
        }

        #region Input Event Methods
        private void OnPointerDown(GuiPointerEventArgs args)
        {
            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return;

            _preFocusedControl = FindControlAtPoint(args.Position);
            PropagateDown(_hoveredControl, x => x.OnPointerDown(this, args));
        }

        private void OnPointerUp(GuiPointerEventArgs args)
        {
            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return;

            var postFocusedControl = FindControlAtPoint(args.Position);

            if (_preFocusedControl == postFocusedControl)
            {
                SetFocus(postFocusedControl);
            }

            _preFocusedControl = null;
            PropagateDown(_hoveredControl, x => x.OnPointerUp(this, args));
        }

        private void OnPointerMoved(GuiPointerEventArgs args)
        {
            CursorPosition = args.Position.ToVector2();

            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return;

            var hoveredControl = FindControlAtPoint(args.Position);

            if (_hoveredControl != hoveredControl)
            {
                if (_hoveredControl != null && (hoveredControl == null || !hoveredControl.HasParent(_hoveredControl)))
                    PropagateDown(_hoveredControl, x => x.OnPointerLeave(this, args));
                _hoveredControl = hoveredControl;
                PropagateDown(_hoveredControl, x => x.OnPointerEnter(this, args));
            }
            else
            {
                PropagateDown(_hoveredControl, x => x.OnPointerMove(this, args));
            }
        }

        public void SetFocus(GuiControl focusedControl)
        {
            if (_focusedControl != focusedControl)
            {
                if (_focusedControl != null)
                {
                    _focusedControl.IsFocused = false;
                    PropagateDown(_focusedControl, x => x.OnUnfocus(this));
                }

                _focusedControl = focusedControl;

                if (_focusedControl != null)
                {
                    _focusedControl.IsFocused = true;
                    PropagateDown(_focusedControl, x => x.OnFocus(this));
                }
            }
        }

        /// <summary>
        /// Method is meant to loop down the parents control to find a suitable event control.  If the predicate returns false
        /// it will continue down the control tree.
        /// </summary>
        /// <param name="control">The control we want to check against</param>
        /// <param name="predicate">A function to check if the propagation should resume, if returns false it will continue down the tree.</param>
        private void PropagateDown(GuiControl control, Func<GuiControl, bool> predicate)
        {
            while(control != null && predicate(control))
            {
                control = control.Parent;
            }
        }

        private GuiControl FindControlAtPoint(Point point)
        {
            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return null;

            //for(var i = Windows.Count - 1; i >= 0; i--)
            //{
            //    var window = Windows[i];
            //    var control = FindControlAtPoint(window.Controls, point);

            //    if (control != null)
            //        return control;
            //}

            return FindControlAtPoint(ActiveScreen.Controls, point);
        }

        private GuiControl FindControlAtPoint(GuiControlCollection controls, Point point)
        {
            var topMostControl = (GuiControl) null;

            for (var i = controls.Count - 1; i >= 0; i--)
            {
                var control = controls[i];

                if (control.IsVisible)
                {
                    if (topMostControl == null && control.Contains(this, point))
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
        #endregion
    }
}