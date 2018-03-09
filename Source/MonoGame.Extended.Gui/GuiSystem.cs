using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace MonoGame.Extended.Gui
{
    public interface IGuiContext
    {
        BitmapFont DefaultFont { get; }
        Vector2 CursorPosition { get; }
        Control FocusedControl { get; }

        void SetFocus(Control focusedControl);
    }

    public class GuiSystem : IGuiContext, IRectangular
    {
        private readonly ViewportAdapter _viewportAdapter;
        private readonly IGuiRenderer _renderer;
        private readonly MouseListener _mouseListener;
        private readonly TouchListener _touchListener;
        private readonly KeyboardListener _keyboardListener;

        private Control _preFocusedControl;

        public GuiSystem(ViewportAdapter viewportAdapter, IGuiRenderer renderer, Skin defaultSkin)
        {
            _viewportAdapter = viewportAdapter;
            _renderer = renderer;

            _mouseListener = new MouseListener(viewportAdapter);
            _mouseListener.MouseDown += (s, e) => OnPointerDown(PointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseMoved += (s, e) => OnPointerMoved(PointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseUp += (s, e) => OnPointerUp(PointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseWheelMoved += (s, e) => FocusedControl?.OnScrolled(e.ScrollWheelDelta);

            _touchListener = new TouchListener(viewportAdapter);
            _touchListener.TouchStarted += (s, e) => OnPointerDown(PointerEventArgs.FromTouchArgs(e));
            _touchListener.TouchMoved += (s, e) => OnPointerMoved(PointerEventArgs.FromTouchArgs(e));
            _touchListener.TouchEnded += (s, e) => OnPointerUp(PointerEventArgs.FromTouchArgs(e));

            _keyboardListener = new KeyboardListener();
            _keyboardListener.KeyTyped += (sender, args) => PropagateDown(FocusedControl, x => x.OnKeyTyped(this, args));
            _keyboardListener.KeyPressed += (sender, args) => PropagateDown(FocusedControl, x => x.OnKeyPressed(this, args));

            DefaultSkin = defaultSkin;
        }

        public Control FocusedControl { get; private set; }
        public Control HoveredControl { get; private set; }

        private Screen _activeScreen;
        public Screen ActiveScreen
        {
            get { return _activeScreen; }
            set
            {
                if (_activeScreen != value)
                {
                    _activeScreen = value;

                    if(_activeScreen != null)
                        InitializeScreen(_activeScreen);
                }
            }
        }

        public Rectangle BoundingRectangle => _viewportAdapter.BoundingRectangle;

        public Vector2 CursorPosition { get; set; }

        public Skin DefaultSkin { get; }

        public BitmapFont DefaultFont => DefaultSkin?.DefaultFont;
        
        private void InitializeScreen(Screen screen)
        {
            screen.Layout(this, BoundingRectangle);
        }

        public void ClientSizeChanged()
        {
            ActiveScreen?.Layout(this, BoundingRectangle);
        }

        public void Update(GameTime gameTime)
        {
            if(ActiveScreen == null)
                return;

            _touchListener.Update(gameTime);
            _mouseListener.Update(gameTime);
            _keyboardListener.Update(gameTime);

            if (ActiveScreen.IsLayoutRequired)
                ActiveScreen.Layout(this, BoundingRectangle);

            ActiveScreen.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            var deltaSeconds = gameTime.GetElapsedSeconds();

            _renderer.Begin();

            if (ActiveScreen != null && ActiveScreen.IsVisible)
            {
                DrawControl(ActiveScreen.Content, deltaSeconds);
                //DrawWindows(ActiveScreen.Windows, deltaSeconds);
            }

            var cursor = DefaultSkin?.Cursor;

            if (cursor != null)
                _renderer.DrawRegion(cursor.TextureRegion, CursorPosition, cursor.Color);

            _renderer.End();
        }

        //private void DrawWindows(WindowCollection windows, float deltaSeconds)
        //{
        //    foreach (var window in windows)
        //    {
        //        window.Draw(this, _renderer, deltaSeconds);
        //        DrawChildren(window.Controls, deltaSeconds);
        //    }
        //}

        private void DrawControl(Control control, float deltaSeconds)
        {
            if(control.Skin == null)
                control.Skin = DefaultSkin;

            if (control.IsVisible)
            {
                control.Draw(this, _renderer, deltaSeconds);

                var itemsControl = control as ItemsControl;

                if (itemsControl != null)
                {
                    foreach (var childControl in itemsControl.Items)
                        DrawControl(childControl, deltaSeconds);
                }
            }
        }

        private void OnPointerDown(PointerEventArgs args)
        {
            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return;

            _preFocusedControl = FindControlAtPoint(args.Position);
            PropagateDown(HoveredControl, x => x.OnPointerDown(this, args));
        }

        private void OnPointerUp(PointerEventArgs args)
        {
            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return;

            var postFocusedControl = FindControlAtPoint(args.Position);

            if (_preFocusedControl == postFocusedControl)
            {
                SetFocus(postFocusedControl);
            }

            _preFocusedControl = null;
            PropagateDown(HoveredControl, x => x.OnPointerUp(this, args));
        }

        private void OnPointerMoved(PointerEventArgs args)
        {
            CursorPosition = args.Position.ToVector2();

            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return;

            var hoveredControl = FindControlAtPoint(args.Position);

            if (HoveredControl != hoveredControl)
            {
                if (HoveredControl != null && (hoveredControl == null || !hoveredControl.HasParent(HoveredControl)))
                    PropagateDown(HoveredControl, x => x.OnPointerLeave(this, args));

                HoveredControl = hoveredControl;
                PropagateDown(HoveredControl, x => x.OnPointerEnter(this, args));
            }
            else
            {
                PropagateDown(HoveredControl, x => x.OnPointerMove(this, args));
            }
        }

        public void SetFocus(Control focusedControl)
        {
            if (FocusedControl != focusedControl)
            {
                if (FocusedControl != null)
                {
                    FocusedControl.IsFocused = false;
                    PropagateDown(FocusedControl, x => x.OnUnfocus(this));
                }

                FocusedControl = focusedControl;

                if (FocusedControl != null)
                {
                    FocusedControl.IsFocused = true;
                    PropagateDown(FocusedControl, x => x.OnFocus(this));
                }
            }
        }

        /// <summary>
        /// Method is meant to loop down the parents control to find a suitable event control.  If the predicate returns false
        /// it will continue down the control tree.
        /// </summary>
        /// <param name="control">The control we want to check against</param>
        /// <param name="predicate">A function to check if the propagation should resume, if returns false it will continue down the tree.</param>
        private static void PropagateDown(Control control, Func<Control, bool> predicate)
        {
            while(control != null && predicate(control))
            {
                control = control.Parent;
            }
        }

        private Control FindControlAtPoint(Point point)
        {
            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return null;

            return FindControlAtPoint(ActiveScreen.Content, point);
        }

        private Control FindControlAtPoint(Control control, Point point)
        {
            var topMostControl = control != null && control.IsVisible && control.Contains(this, point) ? control : null;
            var itemsControl = topMostControl as ItemsControl;

            if (itemsControl != null)
            {
                var controls = itemsControl.Items;

                for (var i = controls.Count - 1; i >= 0; i--)
                {
                    var childControl = FindControlAtPoint(controls[i], point);

                    if (childControl != null)
                        topMostControl = childControl;
                }
            }
            
            return topMostControl;
        }
    }
}