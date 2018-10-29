using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Linq;

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
        private ViewportAdapter _viewportAdapter;
        //private IGuiRenderer _renderer;
        private MouseListener _mouseListener;
        private TouchListener _touchListener;
        private KeyboardListener _keyboardListener;

        public GuiSystem(ViewportAdapter viewportAdapter, IGuiRenderer renderer)
        {
            Screens = new ScreenCollection(this) { };
            ResetViewPortAdapter(viewportAdapter, renderer);
        }

        public Control FocusedControl { get; set; }
        public Control PreFocusedControl { get; set; }
        public Control HoveredControl { get; set; }
        public IGuiRenderer Renderer { get; private set; }
        public ScreenCollection Screens { get; }

        private Screen _activeScreen;
        public Screen ActiveScreen
        {
            get
            {
                if (_activeScreen == null && Screens.Count > 0)
                    _activeScreen = Screens[0];
                return _activeScreen;
            }
            set
            {
                if (_activeScreen != value)
                {
                    _activeScreen = value;

                }
            }
        }

        public Rectangle BoundingRectangle => _viewportAdapter.BoundingRectangle;

        public Vector2 CursorPosition { get; set; }

        public BitmapFont DefaultFont => Skin.Default?.DefaultFont;

        private void InitializeScreen(Screen screen)
        {
            screen.Layout(this, BoundingRectangle);
            screen.Initialize(this);
            if (screen.IsVisible)
                screen.Show();
        }

        public void ResetViewPortAdapter(ViewportAdapter viewportAdapter, IGuiRenderer renderer)
        {
            _viewportAdapter = viewportAdapter;
            Renderer = renderer;

            _mouseListener = new MouseListener(viewportAdapter);
            _mouseListener.MouseDown += (s, e) => OnPointerDown(PointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseMoved += (s, e) => OnPointerMoved(PointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseUp += (s, e) => OnPointerUp(PointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseDrag += (s, e) => OnPointerDrag(PointerEventArgs.FromMouseArgs(e));
            _mouseListener.MouseWheelMoved += (s, e) => FocusedControl?.OnScrolled(e.ScrollWheelDelta);

            _touchListener = new TouchListener(viewportAdapter);
            _touchListener.TouchStarted += (s, e) => OnPointerDown(PointerEventArgs.FromTouchArgs(e));
            _touchListener.TouchMoved += (s, e) => OnPointerMoved(PointerEventArgs.FromTouchArgs(e));
            _touchListener.TouchEnded += (s, e) => OnPointerUp(PointerEventArgs.FromTouchArgs(e));

            _keyboardListener = new KeyboardListener();
            _keyboardListener.KeyTyped += (s, e) => FocusedControl?.OnKeyTyped(this, e);// PropagateDown(FocusedControl, x => x.OnKeyTyped(this, e));
            _keyboardListener.KeyPressed += (s, e) => OnKeyPressed(e);// PropagateDown(FocusedControl, x => x.OnKeyPressed(this, args));

            lock (Screens)
            {
                for (int i = 0; i < Screens.Count; i++)
                {
                    InitializeScreen(Screens[i]);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (ActiveScreen == null)
                return;

            _touchListener.Update(gameTime);
            _mouseListener.Update(gameTime);
            _keyboardListener.Update(gameTime);

            var deltaSeconds = gameTime.GetElapsedSeconds();

            lock (Screens)
            {
                for (int i = 0; i < Screens.Count; i++)
                {
                    var screen = Screens[i];
                    if (screen.IsVisible)
                    {
                        if (screen.IsLayoutRequired)
                            screen.Layout(this, BoundingRectangle);

                        screen.Update(gameTime);
                    }
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            var deltaSeconds = gameTime.GetElapsedSeconds();

            Renderer.Begin();

            for (int i = 0; i < Screens.Count; i++)
            {
                var screen = Screens[i];
                if (screen.IsVisible)
                    screen.Draw(gameTime);
            }

            var cursor = Skin.Default?.Cursor;

            if (cursor != null)
                Renderer.DrawRegion(cursor.TextureRegion, CursorPosition, cursor.Color);

            Renderer.End();
        }

        private void OnPointerDown(PointerEventArgs args)
        {
            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return;

            ActiveScreen.OnPointerDown(args);
            if (FocusedControl != null && FocusedControl.TopMost)
                FocusedControl?.OnPointerDown(this, args);
            else
            {
                PreFocusedControl = FindControlAtPoint(args.Position);
                PreFocusedControl?.OnPointerDown(this, args);
            }
            //PropagateDown(HoveredControl, x => x.OnPointerDown(this, args));
        }

        private void OnPointerUp(PointerEventArgs args)
        {
            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return;

            var postFocusedControl = FindControlAtPoint(args.Position);

            if (PreFocusedControl == postFocusedControl)
            {
                SetFocus(postFocusedControl);
            }
            PreFocusedControl = null;
            FocusedControl?.OnPointerUp(this, args);
            ActiveScreen.OnPointerUp(args);

            //PropagateDown(FocusedControl, x => x.OnPointerUp(this, args));
        }

        private void OnPointerMoved(PointerEventArgs args)
        {
            CursorPosition = args.Position.ToVector2();

            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return;

            ActiveScreen.OnPointerMoved(args);

            var hoveredControl = FindControlAtPoint(args.Position);

            if (HoveredControl != hoveredControl)
            {
                if (HoveredControl != null && (hoveredControl == null || !hoveredControl.HasParent(HoveredControl)))
                    HoveredControl?.OnPointerLeave(this, args);
                //PropagateDown(HoveredControl, x => x.OnPointerLeave(this, args));
                if (hoveredControl != PreFocusedControl)
                    PreFocusedControl = null;

                HoveredControl = hoveredControl;
                HoveredControl?.OnPointerEnter(this, args);
                //PropagateDown(HoveredControl, x => x.OnPointerEnter(this, args));
            }
            else
            {
                HoveredControl?.OnPointerMove(this, args);
                //PropagateDown(HoveredControl, x => x.OnPointerMove(this, args));
            }

        }

        private void OnPointerDrag(PointerEventArgs args)
        {
            CursorPosition = args.Position.ToVector2();

            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return;

            ActiveScreen.OnPointerDrag(args);

            var hoveredControl = FindControlAtPoint(args.Position);

            if (HoveredControl != hoveredControl)
            {
                //if (HoveredControl != null && (hoveredControl == null || !hoveredControl.HasParent(HoveredControl)))
                //    PropagateDown(HoveredControl, x => x.OnPointerLeave(this, args));

                //HoveredControl = hoveredControl;
                //PropagateDown(HoveredControl, x => x.OnPointerEnter(this, args));
            }
            else
            {
                HoveredControl?.OnPointerDrag(this, args);
                //PropagateDown(HoveredControl, x => x.OnPointerDrag(this, args));
            }

        }

        public void SetFocus(Control focusedControl)
        {
            if (FocusedControl != focusedControl)
            {
                if (FocusedControl != null)
                {
                    FocusedControl.IsFocused = false;
                    FocusedControl?.OnUnfocus(this);
                    //PropagateDown(FocusedControl, x => x.OnUnfocus(this));
                }

                FocusedControl = focusedControl;

                if (FocusedControl != null)
                {
                    FocusedControl.IsFocused = true;
                    FocusedControl?.OnUnfocus(this);
                    //PropagateDown(FocusedControl, x => x.OnFocus(this));
                }
            }
        }

        private void OnKeyPressed(KeyboardEventArgs args)
        {
            FocusedControl?.OnKeyPressed(this, args);
            //PropagateDown(FocusedControl, x => x.OnKeyPressed(this, args));
            ActiveScreen.OnKeyPressed(args);
        }

        ///// <summary>
        ///// Method is meant to loop down the parents control to find a suitable event control.  If the predicate returns false
        ///// it will continue down the control tree.
        ///// </summary>
        ///// <param name="control">The control we want to check against</param>
        ///// <param name="predicate">A function to check if the propagation should resume, if returns false it will continue down the tree.</param>
        //private static void PropagateDown(Control control, Func<Control, bool> predicate)
        //{
        //    //while (control != null && predicate(control))
        //    //{
        //    //    control = control.Parent;
        //    //}
        //}

        public Control FindControlAtPoint(Point point)
        {
            if (ActiveScreen == null || !ActiveScreen.IsVisible)
                return null;

            return FindControlAtPoint(ActiveScreen.Content, point);
        }

        public Control FindControlAtPoint(Control control, Point point)
        {
            foreach (var controlChild in control.Children.Reverse())
            {
                var c = FindControlAtPoint(controlChild, point);

                if (c != null)
                    return c;
            }

            if (control.IsVisible && control.Contains(this, point))
                return control;

            return null;
        }
    }
}