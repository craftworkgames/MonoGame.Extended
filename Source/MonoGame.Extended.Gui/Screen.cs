using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class Screen : Element<GuiSystem>, IDisposable
    {
        public event EventHandler<KeyboardEventArgs> KeyPressed;
        public event EventHandler<PointerEventArgs> PointerDown;
        public event EventHandler<PointerEventArgs> PointerUp;
        public event EventHandler<PointerEventArgs> PointerMoved;
        public event EventHandler<PointerEventArgs> PointerDrag;
        public event EventHandler<EventArgs> VisibleChanged;

        public Screen()
        {

        }

        private Control _content;
        [JsonProperty(Order = 1)]
        public Control Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    _isLayoutRequired = true;
                }
            }
        }

        public new float Width { get; private set; }
        public new float Height { get; private set; }
        public new Size2 Size => new Size2(Width, Height);
        public bool IsVisible { get; set; } = false;

        private bool _isLayoutRequired;
        [JsonIgnore]
        public bool IsLayoutRequired => _isLayoutRequired || Content.IsLayoutRequired;
        private List<Control> _topControls;
        private bool _drawTop;

        public virtual void Show()
        {

            if (Parent != null)
                Parent.ActiveScreen = this;
            IsVisible = true;
            VisibleChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Hide()
        {
            IsVisible = false;
            if (Parent != null)
            {
                Parent.ActiveScreen = null;
                Parent.FocusedControl = null;
            }
            VisibleChanged?.Invoke(this, EventArgs.Empty);
        }

        public T FindControl<T>(string name)
            where T : Control
        {
            return FindControl<T>(Content, name);
        }

        private static T FindControl<T>(Control rootControl, string name)
            where T : Control
        {
            if (rootControl.Name == name)
                return rootControl as T;

            foreach (var childControl in rootControl.Children.ToList())
            {
                var control = FindControl<T>(childControl, name);

                if (control != null)
                    return control;
            }

            return null;
        }

        public void Layout(IGuiContext context, Rectangle rectangle)
        {
            Width = rectangle.Width;
            Height = rectangle.Height;

            LayoutHelper.PlaceControl(context, Content, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

            _isLayoutRequired = false;
            Content.IsLayoutRequired = false;
        }

        public virtual void Initialize(IGuiContext context) { }

        public virtual void Update(GameTime gameTime)
        {
            var deltaSeconds = gameTime.GetElapsedSeconds();
            UpdateControl(Content, deltaSeconds);
        }

        public virtual void Draw(GameTime gameTime)
        {
            var deltaSeconds = gameTime.GetElapsedSeconds();
            _topControls = new List<Control>();
            DrawControl(Content, deltaSeconds);
            _drawTop = true;
            foreach (Control control in _topControls.ToArray())
                DrawControl(control, deltaSeconds);
            _drawTop = false;

        }

        private void UpdateControl(Control control, float deltaSeconds)
        {
            if (Parent == null)
                return;

            if (control.IsVisible)
            {
                control.Update(Parent, deltaSeconds);

                foreach (var childControl in control.Children.ToList())
                    UpdateControl(childControl, deltaSeconds);
            }
        }

        private void DrawControl(Control control, float deltaSeconds, bool topMost = false)
        {
            if (Parent == null)
                return;

            if (control.IsVisible)
            {
                control.Draw(Parent, Parent.Renderer, deltaSeconds);
                if (control.TopMost && !_drawTop)
                    _topControls.Add(control);
                else
                    foreach (var childControl in control.Children.ToList())
                        DrawControl(childControl, deltaSeconds);
            }
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {

        }

        public virtual void OnPointerDown(PointerEventArgs args)
        {
            this.PointerDown?.Invoke(this, args);
        }

        public virtual void OnKeyPressed(KeyboardEventArgs args)
        {
            this.KeyPressed?.Invoke(this, args);
        }

        public virtual void OnPointerUp(PointerEventArgs args)
        {
            this.PointerUp?.Invoke(this, args);
        }

        public virtual void OnPointerMoved(PointerEventArgs args)
        {
            this.PointerMoved?.Invoke(this, args);
        }

        public virtual void OnPointerDrag(PointerEventArgs args)
        {
            this.PointerDrag?.Invoke(this, args);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region File Loading
        public static Screen FromStream(ContentManager contentManager, Stream stream, params Type[] customControlTypes)
        {
            return FromStream<Screen>(contentManager, stream, customControlTypes);
        }

        public static TScreen FromStream<TScreen>(ContentManager contentManager, Stream stream, params Type[] customControlTypes)
            where TScreen : Screen
        {
            var skinService = new SkinService();
            var serializer = new GuiJsonSerializer(contentManager, customControlTypes)
            {
                Converters =
                {
                    new SkinJsonConverter(contentManager, skinService, customControlTypes),
                    new ControlJsonConverter(skinService, customControlTypes)
                }
            };

            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var screen = serializer.Deserialize<TScreen>(jsonReader);
                return screen;
            }
        }

        public static Screen FromFile(ContentManager contentManager, string path, params Type[] customControlTypes)
        {
            using (var stream = TitleContainer.OpenStream(path))
            {
                return FromStream<Screen>(contentManager, stream, customControlTypes);
            }
        }

        public static TScreen FromFile<TScreen>(ContentManager contentManager, string path, params Type[] customControlTypes)
            where TScreen : Screen
        {
            using (var stream = TitleContainer.OpenStream(path))
            {
                return FromStream<TScreen>(contentManager, stream, customControlTypes);
            }
        }


        #endregion

    }
}