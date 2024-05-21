using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;

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

        private Skin _skin;

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

        public abstract IEnumerable<Control> Children { get; }

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
                return new Rectangle(r.Left + ClipPadding.Left, r.Top + ClipPadding.Top, r.Width - ClipPadding.Width, r.Height - ClipPadding.Height);
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
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Stretch;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Stretch;
        public HorizontalAlignment HorizontalTextAlignment { get; set; } = HorizontalAlignment.Centre;
        public VerticalAlignment VerticalTextAlignment { get; set; } = VerticalAlignment.Centre;

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

        public virtual bool OnKeyTyped(IGuiContext context, KeyboardEventArgs args) { return true; }
        public virtual bool OnKeyPressed(IGuiContext context, KeyboardEventArgs args) { return true; }

        public virtual bool OnFocus(IGuiContext context) { return true; }
        public virtual bool OnUnfocus(IGuiContext context) { return true; }

        public virtual bool OnPointerDown(IGuiContext context, PointerEventArgs args) { return true; }
        public virtual bool OnPointerMove(IGuiContext context, PointerEventArgs args) { return true; }
        public virtual bool OnPointerUp(IGuiContext context, PointerEventArgs args) { return true; }

        public virtual bool OnPointerEnter(IGuiContext context, PointerEventArgs args)
        {
            if (IsEnabled && !IsHovered)
                IsHovered = true;

            return true;
        }

        public virtual bool OnPointerLeave(IGuiContext context, PointerEventArgs args)
        {
            if (IsEnabled && IsHovered)
                IsHovered = false;

            return true;
        }

        public virtual bool Contains(IGuiContext context, Point point)
        {
            return BoundingRectangle.Contains(point);
        }

        public virtual void Update(IGuiContext context, float deltaSeconds) { }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            if (BackgroundRegion != null)
                renderer.DrawRegion(BackgroundRegion, BoundingRectangle, BackgroundColor);
            else if (BackgroundColor != Color.Transparent)
                renderer.FillRectangle(BoundingRectangle, BackgroundColor);

            if (BorderThickness != 0)
                renderer.DrawRectangle(BoundingRectangle, BorderColor, BorderThickness);

            // handy debug rectangles
            //renderer.DrawRectangle(ContentRectangle, Color.Magenta);
            //renderer.DrawRectangle(BoundingRectangle, Color.Lime);
        }

        public bool HasParent(Control control)
        {
            return Parent != null && (Parent == control || Parent.HasParent(control));
        }

        protected TextInfo GetTextInfo(IGuiContext context, string text, Rectangle targetRectangle, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            var font = Font ?? context.DefaultFont;
            var textSize = (Size)font.GetStringRectangle(text ?? string.Empty, Vector2.Zero).Size;
            var destinationRectangle = LayoutHelper.AlignRectangle(horizontalAlignment, verticalAlignment, textSize, targetRectangle);
            var textPosition = destinationRectangle.Location.ToVector2();
            var textInfo = new TextInfo(text, font, textPosition, textSize, TextColor, targetRectangle);
            return textInfo;
        }

        public struct TextInfo
        {
            public TextInfo(string text, BitmapFont font, Vector2 position, Size size, Color color, Rectangle? clippingRectangle)
            {
                Text = text ?? string.Empty;
                Font = font;
                Size = size;
                Color = color;
                ClippingRectangle = clippingRectangle;
                Position = position;
            }

            public string Text;
            public BitmapFont Font;
            public Size Size;
            public Color Color;
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
