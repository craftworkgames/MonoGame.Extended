using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class Control : Element<Control>, IMovable, ISizable, IRectangular
    {
        protected Control()
        {
            BackgroundColor = Color.White;
            TextColor = Color.White;
            IsEnabled = true;
            IsVisible = true;
            Origin = Vector2.Zero;
        }

        private Skin _skin;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Skin Skin
        {
            get { return _skin; }
            set
            {
                if (_skin != value)
                {
                    _skin = value;
                    _skin?.Apply(this);
                }
            }
        }

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

        private bool _isHovering;

        public abstract Size2 GetContentSize(IGuiContext context);

        public virtual Size2 GetActualSize(IGuiContext context)
        {
            var fixedSize = Size;
            var desiredSize = GetContentSize(context) + Margin.Size + Padding.Size;
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return new Size2(fixedSize.Width == 0 ? desiredSize.Width : fixedSize.Width, fixedSize.Height == 0 ? desiredSize.Height : fixedSize.Height);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
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
        public ControlStyle HoverStyle { get; set; }

        private ControlStyle _disabledStyle;
        public ControlStyle DisabledStyle
        {
            get { return _disabledStyle; }
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
            if (IsEnabled && !_isHovering)
            {
                _isHovering = true;
                HoverStyle?.Apply(this);
            }
            return true;
        }

        public virtual bool OnPointerLeave(IGuiContext context, PointerEventArgs args)
        {
            if (IsEnabled && _isHovering)
            {
                _isHovering = false;
                HoverStyle?.Revert(this);
            }
            return true;
        }

        public virtual bool Contains(IGuiContext context, Point point)
        {
            return BoundingRectangle.Contains(point);
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            DrawBackground(context, renderer, deltaSeconds);
        }

        public bool HasParent(Control control)
        {
            return Parent != null && (Parent == control || Parent.HasParent(control));
        }
        
        protected virtual void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            if (BackgroundRegion != null)
                renderer.DrawRegion(BackgroundRegion, BoundingRectangle, BackgroundColor);
            else if(BackgroundColor != Color.Transparent)
                renderer.FillRectangle(BoundingRectangle, BackgroundColor);

            if(BorderThickness != 0)
                renderer.DrawRectangle(BoundingRectangle, BorderColor, BorderThickness);

            // handy debug rectangles
            //renderer.DrawRectangle(ContentRectangle, Color.Magenta);
            //renderer.DrawRectangle(BoundingRectangle, Color.Lime);
        }
        
        protected TextInfo GetTextInfo(IGuiContext context, string text, Rectangle targetRectangle, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, Rectangle? clippingRectangle = null)
        {
            var font = Font ?? context.DefaultFont;
            var textSize = font.GetStringRectangle(text ?? string.Empty, Vector2.Zero).Size;
            var destinationRectangle = LayoutHelper.AlignRectangle(horizontalAlignment, verticalAlignment, textSize, targetRectangle);
            var textPosition = destinationRectangle.Location.ToVector2();
            var textInfo = new TextInfo(text, font, textPosition, textSize, TextColor, clippingRectangle);// ?? ClippingRectangle);
            return textInfo;
        }
        
        public struct TextInfo
        {
            public TextInfo(string text, BitmapFont font, Vector2 position, Vector2 size, Color color, Rectangle? clippingRectangle)
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
            public Vector2 Size;
            public Color Color;
            public Rectangle? ClippingRectangle;
            public Vector2 Position;
        }
    }
}