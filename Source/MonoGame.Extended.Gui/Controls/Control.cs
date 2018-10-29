using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class Control : Element<Control>, IRectangular
    {
        protected Control()
        {
            BackgroundColor = Color.White;
            TextColor = Color.White;
            IsEnabled = true;
            IsVisible = true;
            Origin = Point.Zero;
            Skin = Skin.Default;
        }

        public event EventHandler<PointerEventArgs> PointerUp;
        public event EventHandler<PointerEventArgs> PointerMoved;
        public event EventHandler<PointerEventArgs> PointerDown;
        public event EventHandler<PointerEventArgs> PointerDrag;
        public event EventHandler<PointerEventArgs> PointerEnter;
        public event EventHandler<PointerEventArgs> PointerLeave;
        public event EventHandler<KeyboardEventArgs> KeyPressed;
        public event EventHandler<KeyboardEventArgs> KeyTyped;


        private Skin _skin;
        private bool _isTopMost = false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Skin Skin
        {
            get => _skin;
            set
            {
                if (_skin != value)
                {
                    _skin = value;
                    _skin?.Apply(this);
                }
            }
        }

        public virtual IEnumerable<Control> Children { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Thickness Margin { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Thickness Padding { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Thickness ClipPadding { get; set; }

        public bool IsLayoutRequired { get; set; }

        public Rectangle ClippingRectangle
        {
            get
            {
                var r = BoundingRectangle;

                var clipRect = new Rectangle(r.Left + ClipPadding.Left, r.Top + ClipPadding.Top,
                    r.Width - ClipPadding.Width, r.Height - ClipPadding.Height);

                if (Parent != null)
                    return clipRect.Clip(Parent.ClippingRectangle);
                else
                    return clipRect;
            }
        }

        public Rectangle ContentRectangle
        {
            get
            {
                var r = BoundingRectangle;
                return new Rectangle(r.Left + Padding.Left, r.Top + Padding.Top, r.Width - Padding.Width, r.Height - Padding.Height);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsFocused { get; set; }

        private bool _isHovered;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsHovered
        {
            get => _isHovered;
            private set
            {
                if (_isHovered != value)
                {
                    _isHovered = value;
                    HoverStyle?.ApplyIf(this, _isHovered);
                }
            }
        }

        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Vector2 Offset { get; set; }
        public BitmapFont Font { get; set; }
        public Color TextColor { get; set; }
        public Vector2 TextOffset { get; set; }
        public Vector2 TextScale { get; set; } = Vector2.One;
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Centre;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Centre;
        public HorizontalAlignment HorizontalTextAlignment { get; set; } = HorizontalAlignment.Centre;
        public VerticalAlignment VerticalTextAlignment { get; set; } = VerticalAlignment.Centre;
        public HorizontalAlignment HorizontalImageAlignment { get; set; } = HorizontalAlignment.Centre;
        public VerticalAlignment VerticalImageAlignment { get; set; } = VerticalAlignment.Centre;
        public Object Tag { get; set; }
        public bool TopMost { get { return _isTopMost; } }

        public abstract Size GetContentSize(IGuiContext context);

        public virtual Size CalculateActualSize(IGuiContext context)
        {
            var fixedSize = Size;
            var desiredSize = GetContentSize(context) + Margin.Size + Padding.Size;

            if (desiredSize.Width < MinWidth)
                desiredSize.Width = MinWidth;

            if (desiredSize.Height < MinHeight)
                desiredSize.Height = MinHeight;

            if (desiredSize.Width > MaxWidth)
                desiredSize.Width = MaxWidth;

            if (desiredSize.Height > MaxWidth)
                desiredSize.Height = MaxHeight;

            var width = fixedSize.Width == 0 ? desiredSize.Width : fixedSize.Width;
            var height = fixedSize.Height == 0 ? desiredSize.Height : fixedSize.Height;
            return new Size(width, height);
        }

        public virtual void InvalidateMeasure() { }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    DisabledStyle?.ApplyIf(this, !_isEnabled);
                }
            }
        }

        public bool IsVisible { get; set; }

        private ControlStyle _hoverStyle;
        public ControlStyle HoverStyle
        {
            get => _hoverStyle;
            set
            {
                if (_hoverStyle != value)
                {
                    _hoverStyle = value;
                    HoverStyle?.ApplyIf(this, _isHovered);
                }
            }
        }

        private ControlStyle _disabledStyle;
        public ControlStyle DisabledStyle
        {
            get => _disabledStyle;
            set
            {
                _disabledStyle = value;
                DisabledStyle?.ApplyIf(this, !_isEnabled);
            }
        }

        public virtual void OnScrolled(int delta) { }

        public virtual bool OnKeyTyped(IGuiContext context, KeyboardEventArgs args) { KeyTyped?.Invoke(context, args); return true; }
        public virtual bool OnKeyPressed(IGuiContext context, KeyboardEventArgs args) { KeyPressed?.Invoke(context, args); return true; }

        public virtual bool OnFocus(IGuiContext context) { return true; }
        public virtual bool OnUnfocus(IGuiContext context) { return true; }

        public virtual bool OnPointerDown(IGuiContext context, PointerEventArgs args)
        { if (ClippingRectangle.Contains(args.Position)) PointerDown?.Invoke(context, args); return true; }
        public virtual bool OnPointerUp(IGuiContext context, PointerEventArgs args)
        { if (ClippingRectangle.Contains(args.Position)) PointerUp?.Invoke(context, args); return true; }
        public virtual bool OnPointerMoved(IGuiContext context, PointerEventArgs args)
        { if (ClippingRectangle.Contains(args.Position)) PointerMoved?.Invoke(context, args); return true; }
        public virtual bool OnPointerDrag(IGuiContext context, PointerEventArgs args)
        { if (ClippingRectangle.Contains(args.Position)) PointerDrag?.Invoke(context, args); return true; }

        public virtual bool OnPointerEnter(IGuiContext context, PointerEventArgs args)
        {
            if (IsEnabled && !IsHovered)
                IsHovered = true;

            PointerEnter?.Invoke(context, args);

            return true;
        }

        public virtual bool OnPointerLeave(IGuiContext context, PointerEventArgs args)
        {
            if (IsEnabled && IsHovered)
                IsHovered = false;

            PointerLeave?.Invoke(context, args);

            return true;
        }

        public virtual bool OnPointerMove(IGuiContext context, PointerEventArgs args)
        {
            PointerMoved?.Invoke(context, args);

            return true;
        }

        public virtual bool Contains(IGuiContext context, Point point)
        {
            return ClippingRectangle.Contains(point);
        }

        public virtual void Update(IGuiContext context, float deltaSeconds) { }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            if (BackgroundRegion == null && BackgroundColor != Color.Transparent)
                renderer.FillRectangle(BoundingRectangle, BackgroundColor);
            if (BackgroundRegion != null)
                renderer.DrawRegion(BackgroundRegion, BoundingRectangle, BackgroundColor, this.ClippingRectangle);
            if (Image != null)
                DrawImage(context, renderer, deltaSeconds, HorizontalImageAlignment, VerticalImageAlignment, null);
            if (BorderThickness != 0)
                renderer.DrawRectangle(BoundingRectangle, BorderColor, BorderThickness);

            // handy debug rectangles
            //renderer.DrawRectangle(ContentRectangle, Color.Magenta);
            //renderer.DrawRectangle(BoundingRectangle, Color.Lime);
        }

        protected virtual void DrawImage(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, Rectangle? clippingRectangle)
        {
            var rect = clippingRectangle != null ? clippingRectangle.Value.Clip(BoundingRectangle) : BoundingRectangle;
            var minSize = Math.Min(rect.Width, rect.Height);
            var imgSize = new Size((int)ImageSize.Width, (int)ImageSize.Height);
            var imageSize = imgSize == Size.Empty ? new Size(minSize, minSize) : imgSize;
            var destinationRectangle = LayoutHelper.AlignRectangle(horizontalAlignment, verticalAlignment, imageSize, rect);
            renderer.DrawRegion(Image, destinationRectangle, Color.White, clippingRectangle);
        }

        public bool HasParent(Control control)
        {
            return Parent != null && (Parent == control || Parent.HasParent(control));
        }

        protected TextInfo GetTextInfo(IGuiContext context, string text, Rectangle targetRectangle, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            var font = Font ?? context.DefaultFont;
            var textSize = (Size)font.GetStringRectangle(text ?? string.Empty, Vector2.Zero, Vector2.One).Size;
            var destinationRectangle = LayoutHelper.AlignRectangle(horizontalAlignment, verticalAlignment, textSize, targetRectangle);
            var textPosition = destinationRectangle.Location.ToVector2();
            var textInfo = new TextInfo(text, font, textPosition, textSize, TextColor, TextScale, targetRectangle);
            return textInfo;
        }

        public void SetTopMost(bool isTop)
        {
            _isTopMost = isTop;
        }

        public T FindControl<T>(string name)
            where T : Control
        {
            return FindControl<T>(this, name);
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

        public struct TextInfo
        {
            public TextInfo(string text, BitmapFont font, Vector2 position, Size size, Color color, Vector2 scale, Rectangle? clippingRectangle)
            {
                Text = text ?? string.Empty;
                Font = font;
                Size = size;
                Color = color;
                ClippingRectangle = clippingRectangle;
                Position = position;
                Scale = scale;
            }

            public string Text;
            public BitmapFont Font;
            public Size Size;
            public Color Color;
            public Vector2 Scale;
            public Rectangle? ClippingRectangle;
            public Vector2 Position;
        }

        public Dictionary<string, object> AttachedProperties { get; } = new Dictionary<string, object>();

        public object GetAttachedProperty(string name)
        {
            return AttachedProperties.TryGetValue(name, out var value) ? value : null;
        }

        public void SetAttachedProperty(string name, object value)
        {
            AttachedProperties[name] = value;
        }

        public virtual Type GetAttachedPropertyType(string propertyName)
        {
            return null;
        }
    }
}